using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("UPGEN Lighting/UPGEN RayTraced GI")]
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public sealed class UL_RayTracedGI : MonoBehaviour
{
	[Range(0f, 5f)]
	public float intensity = 1f;

	[Range(2f, 15f)]
	public int raysMatrixSize = 7;

	[Range(0.1f, 10f)]
	public float raysMatrixScale = 1f;

	private const float BOUNCED_LIGHT_RANGE = 8f;

	private const float BOUNCED_LIGHT_BOOST = 5f;

	private const float SUN_BOUNCED_LIGHT_BOOST = 3f;

	private const float SUN_FAR_OFFSET = 100f;

	private const float SUN_FAR_OFFSET_DBL = 200f;

	public static readonly List<UL_RayTracedGI> all = new List<UL_RayTracedGI>();

	private Light _light;

	private float _lastUpdateTime;

	private float _lastTime;

	private UL_Rays.Ray[] _rays;

	private Vector2[] _rayMatrix2D;

	private Vector3[] _rayMatrix3D;

	public Light BaseLight
	{
		[CompilerGenerated]
		get
		{
			return (!(_light == null)) ? _light : (_light = GetComponent<Light>());
		}
	}

	private void OnEnable()
	{
		all.Add(this);
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	internal void GenerateRenderData()
	{
		if (_light == null)
		{
			_light = GetComponent<Light>();
			if (_light == null)
			{
				return;
			}
		}
		if (!_light.enabled)
		{
			return;
		}
		float unscaledTime = Time.unscaledTime;
		float num = unscaledTime - _lastTime;
		_lastTime = unscaledTime;
		if (unscaledTime - _lastUpdateTime > 0.2f)
		{
			_lastUpdateTime = unscaledTime;
			UpdateRaysMatrix();
			if (_rays == UL_Rays.EMPTY_RAYS)
			{
				return;
			}
			float num2 = _light.intensity * intensity;
			switch (_light.type)
			{
			case LightType.Spot:
			case LightType.Point:
				num2 *= 5f / (float)(raysMatrixSize * raysMatrixSize);
				break;
			case LightType.Directional:
				num2 *= 3f;
				break;
			}
			Color lightColor = _light.color.linear * num2;
			if (lightColor.maxColorComponent < 0.001f)
			{
				for (int num3 = _rays.Length - 1; num3 >= 0; num3--)
				{
					_rays[num3].hit = false;
				}
				return;
			}
			Vector3 position = base.transform.position;
			float range = _light.range;
			int layersToHit = ((!UL_Manager.instance) ? (-5) : ((int)UL_Manager.instance.layersToRayTrace));
			switch (_light.type)
			{
			case LightType.Directional:
			{
				float num5 = (float)raysMatrixSize * raysMatrixScale;
				Vector3 vector = base.transform.right * num5;
				Vector3 vector2 = base.transform.up * num5;
				Vector3 forward = base.transform.forward;
				Vector3 vector3 = base.transform.position - forward * 100f;
				for (int num6 = _rayMatrix2D.Length - 1; num6 >= 0; num6--)
				{
					Vector2 vector4 = _rayMatrix2D[num6];
					_rays[num6].Trace(vector3 + vector4.x * vector + vector4.y * vector2, forward, 200f, lightColor, layersToHit);
				}
				break;
			}
			case LightType.Spot:
			{
				float num7 = Mathf.Tan((float)Math.PI / 180f * _light.spotAngle * 0.4f) * _light.range;
				Quaternion rotation = base.transform.rotation;
				Vector3 forward2 = base.transform.forward;
				for (int num8 = _rayMatrix2D.Length - 1; num8 >= 0; num8--)
				{
					_rays[num8].Trace(position, (forward2 * range + rotation * (_rayMatrix2D[num8] * num7)).normalized, range, lightColor, layersToHit);
				}
				break;
			}
			case LightType.Point:
			{
				for (int num4 = _rayMatrix3D.Length - 1; num4 >= 0; num4--)
				{
					_rays[num4].Trace(position, _rayMatrix3D[num4], range, lightColor, layersToHit);
				}
				break;
			}
			}
		}
		for (int num9 = _rays.Length - 1; num9 >= 0; num9--)
		{
			UL_Rays.Ray ray = _rays[num9];
			if ((ray.interpolatedPosition - ray.position).sqrMagnitude > 9f)
			{
				if (ray.interpolatedColor.maxColorComponent > 0.01f)
				{
					ray.interpolatedColor = Color.Lerp(ray.interpolatedColor, Color.black, num * 10f);
				}
				else
				{
					ray.interpolatedColor = Color.Lerp(ray.interpolatedColor, (!ray.hit) ? Color.black : ray.color, num * 10f);
					ray.interpolatedPosition = ray.position;
				}
				UL_Renderer.Add(ray.interpolatedPosition, 8f, ray.interpolatedColor);
			}
			else
			{
				ray.interpolatedColor = Color.Lerp(ray.interpolatedColor, (!ray.hit) ? Color.black : ray.color, num * 5f);
				ray.interpolatedPosition = Vector3.Lerp(ray.interpolatedPosition, ray.position, num * 10f);
				UL_Renderer.Add(ray.interpolatedPosition, 8f, ray.interpolatedColor);
			}
		}
	}

	private void UpdateRaysMatrix()
	{
		switch (_light.type)
		{
		case LightType.Spot:
		case LightType.Directional:
		{
			Vector2[] array2 = UL_RayMatrices.GRID[raysMatrixSize - 2];
			if (_rayMatrix2D != array2)
			{
				_rayMatrix2D = array2;
				_rays = UL_Rays.GenerateRays(array2.Length);
			}
			break;
		}
		case LightType.Point:
		{
			Vector3[] array = UL_RayMatrices.SPHERE[raysMatrixSize - 2];
			if (_rayMatrix3D != array)
			{
				_rayMatrix3D = array;
				_rays = UL_Rays.GenerateRays(array.Length);
			}
			break;
		}
		default:
			_rays = UL_Rays.EMPTY_RAYS;
			break;
		}
	}

	private void BuildFastLight(UL_Rays.Ray ray)
	{
		if (ray.hit)
		{
			GameObject gameObject = new GameObject("Fast Light");
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject.transform.position = ray.position;
			UL_FastLight uL_FastLight = gameObject.AddComponent<UL_FastLight>();
			uL_FastLight.intensity = 1f;
			uL_FastLight.range = 8f;
			uL_FastLight.color = ray.color.gamma;
		}
	}

	public void CreateFastLights()
	{
		if (base.enabled)
		{
			for (int num = _rays.Length - 1; num >= 0; num--)
			{
				BuildFastLight(_rays[num]);
			}
			base.enabled = false;
		}
	}

	public void DestroyFastLights()
	{
		if (base.enabled)
		{
			return;
		}
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = base.transform.GetChild(num);
			if ((bool)child.GetComponent<UL_FastLight>())
			{
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
		base.enabled = true;
	}
}
