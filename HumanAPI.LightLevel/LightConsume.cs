using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer;
using UnityEngine;

namespace HumanAPI.LightLevel;

public class LightConsume : Node
{
	private LightFilter[] filters;

	private Vector3 lastPosition;

	private Quaternion lastRotation;

	private Dictionary<LightBase, LightHitInfo> lightHits;

	public NodeOutput Intensity;

	public bool checkIfUnderSun;

	public bool debugLog;

	[HideInInspector]
	public List<LightBase> ignoreLights;

	public Action<LightBase> lightAdded = delegate
	{
	};

	public Action<LightBase> lightRemoved = delegate
	{
	};

	private Collider col;

	public bool isLit => lightHits.Count > 0;

	public Color LitColor
	{
		get
		{
			Color result = new Color(0f, 0f, 0f, 0f);
			if (lightHits.Count == 0)
			{
				return result;
			}
			foreach (KeyValuePair<LightBase, LightHitInfo> lightHit in lightHits)
			{
				if (lightHit.Value.intensity > -1f)
				{
					result.r += lightHit.Value.intensity;
					result.r += lightHit.Value.intensity;
					result.r += lightHit.Value.intensity;
				}
				else
				{
					result.r = lightHit.Key.color.r + result.r;
					result.g = lightHit.Key.color.g + result.g;
					result.b = lightHit.Key.color.b + result.b;
				}
				result.a = Mathf.Max(lightHit.Key.color.a, result.a);
			}
			return result;
		}
	}

	protected virtual void Awake()
	{
		filters = GetComponentsInChildren<LightFilter>();
		filters = filters.OrderByDescending((LightFilter f) => f.priority).ToArray();
		LightFilter[] array = filters;
		foreach (LightFilter lightFilter in array)
		{
			lightFilter.Init(this);
		}
		lightHits = new Dictionary<LightBase, LightHitInfo>();
		ignoreLights = new List<LightBase>();
		col = GetComponent<Collider>();
	}

	private void FixedUpdate()
	{
		if (!NetGame.isClient)
		{
			if (lastPosition != base.transform.position || lastRotation != base.transform.rotation)
			{
				lastRotation = base.transform.rotation;
				lastPosition = base.transform.position;
				RecalculateAll();
			}
			if (checkIfUnderSun)
			{
				CheckIfUnderSun();
			}
			CheckOutput();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		LightBase componentInParent = other.GetComponentInParent<LightBase>();
		if (!(componentInParent == null))
		{
			if (debugLog)
			{
				Debug.Log("Enter " + other.name);
			}
			AddLightSource(componentInParent);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		LightBase componentInParent = other.GetComponentInParent<LightBase>();
		if (!(componentInParent == null))
		{
			AddLightSource(componentInParent);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		LightBase componentInParent = other.GetComponentInParent<LightBase>();
		if (!(componentInParent == null))
		{
			if (debugLog)
			{
				Debug.Log("Exit " + other.name);
			}
			RemoveLightSource(componentInParent);
			componentInParent.UpdateLight();
		}
	}

	private void AddLightSource(LightBase source)
	{
		if (!ignoreLights.Contains(source) && !lightHits.ContainsKey(source))
		{
			source.AddConsume(this);
			LightHitInfo lightHitInfo = new LightHitInfo(source);
			Recalculate(lightHitInfo);
			lightHits.Add(source, lightHitInfo);
			if (debugLog)
			{
				Debug.Log("Added new light source: " + source.name);
			}
			lightAdded(source);
			source.UpdateLight();
		}
	}

	private void Recalculate(LightHitInfo info)
	{
		info.contactPoint = info.source.ClosestPoint(base.transform.position);
		LightFilter[] array = filters;
		foreach (LightFilter lightFilter in array)
		{
			lightFilter.ApplyFilter(info);
		}
	}

	protected virtual void CheckOutput()
	{
		Color litColor = LitColor;
		float num = (litColor.r + litColor.g + litColor.b) / 3f;
		if (num != Intensity.value)
		{
			Intensity.SetValue(num);
		}
	}

	private T CreateDefaultOutput<T>(T input, Vector3 point) where T : LightBase
	{
		return LightPool.Create<T>(point, point - input.transform.position, base.transform);
	}

	private void CheckIfUnderSun()
	{
		if (Sun.instance == null)
		{
			return;
		}
		Vector3 direction = Sun.instance.Direction;
		Vector3 axis = base.transform.InverseTransformDirection(Vector3.up);
		Quaternion quaternion = Quaternion.AngleAxis(base.transform.rotation.eulerAngles.y, axis);
		Bounds bounds = col.bounds;
		Vector3[] array = new Vector3[4]
		{
			quaternion * base.transform.InverseTransformDirection(bounds.extents.x * 0.6f, 0f, 0f),
			quaternion * base.transform.InverseTransformDirection((0f - bounds.extents.x) * 0.6f, 0f, 0f),
			quaternion * base.transform.InverseTransformDirection(0f, 0f, bounds.extents.z * 0.6f),
			quaternion * base.transform.InverseTransformDirection(0f, 0f, (0f - bounds.extents.z) * 0.6f)
		};
		float num = 4f;
		Vector3[] array2 = array;
		foreach (Vector3 position in array2)
		{
			if (Physics.Raycast(base.transform.TransformPoint(position), -direction))
			{
				num -= 1f;
			}
		}
		if (num == 3f)
		{
			num = 2f;
		}
		float intensity = num / (float)array.Length;
		Vector3[] array3 = array;
		foreach (Vector3 position2 in array3)
		{
			Debug.DrawLine(base.transform.TransformPoint(position2), base.transform.TransformPoint(position2) + Vector3.up * 0.1f);
		}
		if (!lightHits.ContainsKey(Sun.instance))
		{
			LightHitInfo value = new LightHitInfo(Sun.instance, intensity);
			lightHits.Add(Sun.instance, value);
		}
		else
		{
			lightHits[Sun.instance].intensity = intensity;
		}
	}

	public override void Process()
	{
		base.Process();
	}

	public void RecalculateAll()
	{
		List<LightBase> list = new List<LightBase>(lightHits.Keys);
		foreach (LightBase item in list)
		{
			item.UpdateLight();
		}
	}

	public void Recalculate(LightBase source)
	{
		if (lightHits.ContainsKey(source))
		{
			Recalculate(lightHits[source]);
		}
	}

	public void RemoveLightSource(LightBase source)
	{
		if (!lightHits.ContainsKey(source))
		{
			return;
		}
		source.RemoveConsume(this);
		foreach (LightBase output in lightHits[source].outputs)
		{
			output.DisableLight();
		}
		lightHits.Remove(source);
		lightRemoved(source);
	}
}
