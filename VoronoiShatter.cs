using System;
using System.Collections.Generic;
using HumanAPI;
using Multiplayer;
using UnityEngine;
using Voronoi2;

public class VoronoiShatter : ShatterBase
{
	public GameObject shardPrefab;

	public PhysicMaterial physicsMaterial;

	public ShatterAxis thicknessLocalAxis;

	public float densityPerSqMeter;

	public float totalMass = 100f;

	public int shardLayer = 10;

	public Vector3 adjustColliderSize;

	public float minExplodeImpulse;

	public float maxExplodeImpulse = float.PositiveInfinity;

	public float perShardImpulseFraction = 0.25f;

	public float maxShardVelocity = float.PositiveInfinity;

	public float cellInset;

	private List<GameObject> cells = new List<GameObject>();

	private float scale = 1f;

	private Rigidbody body;

	private Material material;

	[Tooltip("Optional - set the parent object for the shards")]
	public Transform parentObject;

	public bool resetOnReload = true;

	private NetScope shardParent;

	protected override void OnEnable()
	{
		base.OnEnable();
		body = GetComponent<Rigidbody>();
		material = renderer.sharedMaterial;
		collider = GetComponent<BoxCollider>();
		scale = base.transform.lossyScale.x;
	}

	private Vector3 To3D(Vector3 v)
	{
		return thicknessLocalAxis switch
		{
			ShatterAxis.X => new Vector3(v.z, v.x, v.y) / scale, 
			ShatterAxis.Y => new Vector3(v.y, v.z, v.x) / scale, 
			ShatterAxis.Z => new Vector3(v.x, v.y, v.z) / scale, 
			_ => throw new InvalidOperationException(), 
		};
	}

	private Vector3 To2D(Vector3 v)
	{
		return thicknessLocalAxis switch
		{
			ShatterAxis.X => new Vector3(v.y, v.z, v.x) * scale, 
			ShatterAxis.Y => new Vector3(v.z, v.x, v.y) * scale, 
			ShatterAxis.Z => new Vector3(v.x, v.y, v.z) * scale, 
			_ => throw new InvalidOperationException(), 
		};
	}

	protected override void Shatter(Vector3 contactPoint, Vector3 adjustedImpulse, float impactMagnitude, uint seed, uint netId)
	{
		base.Shatter(contactPoint, adjustedImpulse, impactMagnitude, seed, netId);
		GameObject gameObject = new GameObject(base.name + "shards");
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		shardParent = gameObject.AddComponent<NetScope>();
		shardParent.AssignNetId(netId);
		shardParent.suppressThrottling = 3f;
		BoxCollider boxCollider = collider as BoxCollider;
		Vector2 vector = To2D(base.transform.InverseTransformPoint(contactPoint) - boxCollider.center);
		Vector3 vector2 = To2D(boxCollider.center);
		Vector3 vector3 = To2D(boxCollider.size + adjustColliderSize);
		float x = vector3.x;
		float y = vector3.y;
		float z = vector3.z;
		float num = (0f - vector3.x) / 2f;
		float num2 = vector3.x / 2f;
		float num3 = (0f - vector3.y) / 2f;
		float num4 = vector3.y / 2f;
		float z2 = (0f - vector3.z) / 2f;
		float z3 = vector3.z / 2f;
		float num5 = x * y;
		if (densityPerSqMeter == 0f)
		{
			densityPerSqMeter = totalMass / num5;
		}
		float num6 = Mathf.Min(x, y) / 4f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)seed);
		int num7 = (int)Mathf.Clamp(num5 * 10f, 5f, 50f);
		if (NetGame.isNetStarted)
		{
			num7 /= 2;
		}
		int num8 = num7 / 2;
		Voronoi voronoi = new Voronoi(0.1f);
		float[] array = new float[num7];
		float[] array2 = new float[num7];
		for (int i = 0; i < num8; i++)
		{
			array[i] = UnityEngine.Random.Range(num, num2);
			array2[i] = UnityEngine.Random.Range(num3, num4);
		}
		for (int j = num8; j < num7; j++)
		{
			int num9 = 0;
			Vector2 vector4;
			do
			{
				if (num9++ > 1000)
				{
					return;
				}
				vector4 = UnityEngine.Random.insideUnitCircle * num6 + vector;
			}
			while (vector4.x < num || vector4.y < num3 || vector4.x > num2 || vector4.y > num4);
			array[j] = vector4.x;
			array2[j] = vector4.y;
		}
		UnityEngine.Random.state = state;
		List<GraphEdge> list = voronoi.generateVoronoi(array, array2, num, num2, num3, num4);
		List<Vector2>[] array3 = new List<Vector2>[num7];
		for (int k = 0; k < num7; k++)
		{
			array3[k] = new List<Vector2>();
		}
		int count = list.Count;
		for (int l = 0; l < count; l++)
		{
			GraphEdge graphEdge = list[l];
			Vector2 vector5 = new Vector2(graphEdge.x1, graphEdge.y1);
			Vector2 vector6 = new Vector2(graphEdge.x2, graphEdge.y2);
			if (!(vector5 == vector6))
			{
				if (!array3[graphEdge.site1].Contains(vector5))
				{
					array3[graphEdge.site1].Add(vector5);
				}
				if (!array3[graphEdge.site2].Contains(vector5))
				{
					array3[graphEdge.site2].Add(vector5);
				}
				if (!array3[graphEdge.site1].Contains(vector6))
				{
					array3[graphEdge.site1].Add(vector6);
				}
				if (!array3[graphEdge.site2].Contains(vector6))
				{
					array3[graphEdge.site2].Add(vector6);
				}
			}
		}
		float num10 = float.MaxValue;
		int num11 = 0;
		Vector2 vector7 = new Vector2(num, num3);
		float num12 = float.MaxValue;
		int num13 = 0;
		Vector2 vector8 = new Vector2(num, num4);
		float num14 = float.MaxValue;
		int num15 = 0;
		Vector2 vector9 = new Vector2(num2, num3);
		float num16 = float.MaxValue;
		int num17 = 0;
		Vector2 vector10 = new Vector2(num2, num4);
		for (int m = 0; m < num7; m++)
		{
			Vector2 vector11 = new Vector2(array[m], array2[m]);
			float sqrMagnitude = (vector7 - vector11).sqrMagnitude;
			if (sqrMagnitude < num10)
			{
				num10 = sqrMagnitude;
				num11 = m;
			}
			float sqrMagnitude2 = (vector8 - vector11).sqrMagnitude;
			if (sqrMagnitude2 < num12)
			{
				num12 = sqrMagnitude2;
				num13 = m;
			}
			float sqrMagnitude3 = (vector9 - vector11).sqrMagnitude;
			if (sqrMagnitude3 < num14)
			{
				num14 = sqrMagnitude3;
				num15 = m;
			}
			float sqrMagnitude4 = (vector10 - vector11).sqrMagnitude;
			if (sqrMagnitude4 < num16)
			{
				num16 = sqrMagnitude4;
				num17 = m;
			}
		}
		array3[num11].Add(vector7);
		array3[num13].Add(vector8);
		array3[num15].Add(vector9);
		array3[num17].Add(vector10);
		Vector3 normalized = adjustedImpulse.normalized;
		float value = Mathf.Clamp(adjustedImpulse.magnitude, minExplodeImpulse, maxExplodeImpulse) * perShardImpulseFraction;
		List<Vector2> list2 = new List<Vector2>();
		List<Vector2> list3 = new List<Vector2>();
		List<float> list4 = new List<float>();
		for (int n = 0; n < num7; n++)
		{
			Vector2 vector12 = new Vector2(array[n], array2[n]);
			List<Vector2> list5 = array3[n];
			if (list5.Count < 3)
			{
				continue;
			}
			list4.Clear();
			list3.Clear();
			list2.Clear();
			int count2 = list5.Count;
			Vector2 zero = Vector2.zero;
			for (int num18 = 0; num18 < count2; num18++)
			{
				zero += list5[num18];
			}
			zero /= (float)list5.Count;
			for (int num19 = 0; num19 < count2; num19++)
			{
				Vector2 item = list5[num19] - zero;
				float num20 = Mathf.Atan2(item.x, item.y);
				int num21;
				for (num21 = 0; num21 < list4.Count && num20 < list4[num21]; num21++)
				{
				}
				list3.Insert(num21, item);
				list4.Insert(num21, num20);
			}
			if (cellInset > 0f)
			{
				for (int num22 = 0; num22 < count2; num22++)
				{
					Vector2 vector13 = list3[num22];
					Vector2 vector14 = list3[(num22 + count2 - 1) % count2];
					Vector2 vector15 = list3[(num22 + count2 - 1) % count2];
					Vector2 normalized2 = (vector14 - vector13 + vector15 - vector13).normalized;
					list2.Add(normalized2);
				}
				for (int num23 = 0; num23 < count2; num23++)
				{
					list3[num23] += list2[num23] * cellInset;
				}
			}
			Vector3[] array4 = new Vector3[count2 * 6];
			int[] array5 = new int[(count2 * 2 + (count2 - 2) * 2) * 3];
			int num24 = count2 * 2 * 3;
			int num25 = num24 + (count2 - 2) * 3;
			Vector3 zero2 = Vector3.zero;
			for (int num26 = 0; num26 < count2; num26++)
			{
				Vector2 vector16 = list3[num26];
				ref Vector3 reference = ref array4[num26 * 6];
				ref Vector3 reference2 = ref array4[num26 * 6 + 1];
				ref Vector3 reference3 = ref array4[num26 * 6 + 2];
				reference = (reference2 = (reference3 = To3D(new Vector3(vector16.x, vector16.y, z2) - zero2)));
				ref Vector3 reference4 = ref array4[num26 * 6 + 3];
				ref Vector3 reference5 = ref array4[num26 * 6 + 4];
				ref Vector3 reference6 = ref array4[num26 * 6 + 5];
				reference4 = (reference5 = (reference6 = To3D(new Vector3(vector16.x, vector16.y, z3) - zero2)));
				int num27 = (num26 + 1) % count2;
				int num28 = num26 * 6 + 3;
				int num29 = num26 * 6;
				int num30 = num27 * 6 + 4;
				int num31 = num27 * 6 + 1;
				array5[num26 * 6] = num28;
				array5[num26 * 6 + 1] = num29;
				array5[num26 * 6 + 2] = num30;
				array5[num26 * 6 + 3] = num30;
				array5[num26 * 6 + 4] = num29;
				array5[num26 * 6 + 5] = num31;
				if (num26 >= 2)
				{
					array5[num24 + (num26 - 2) * 3] = 2;
					array5[num24 + (num26 - 2) * 3 + 1] = num26 * 6 + 2;
					array5[num24 + (num26 - 2) * 3 + 2] = (num26 - 1) * 6 + 2;
					array5[num25 + (num26 - 2) * 3] = 5;
					array5[num25 + (num26 - 2) * 3 + 1] = (num26 - 1) * 6 + 5;
					array5[num25 + (num26 - 2) * 3 + 2] = num26 * 6 + 5;
				}
			}
			Mesh mesh = new Mesh();
			mesh.name = "cell" + n;
			mesh.vertices = array4;
			mesh.triangles = array5;
			mesh.RecalculateNormals();
			float num32 = 0f;
			for (int num33 = 0; num33 < count2; num33++)
			{
				Vector2 vector17 = list3[num33];
				Vector2 vector18 = list3[(num33 + 1) % count2];
				num32 += vector17.x * vector18.y - vector17.y * vector18.x;
			}
			num32 /= 2f;
			MeshFilter meshFilter = null;
			MeshRenderer meshRenderer = null;
			MeshCollider meshCollider = null;
			CollisionAudioSensor collisionAudioSensor = null;
			GameObject gameObject2;
			if (shardPrefab == null)
			{
				gameObject2 = new GameObject();
				gameObject2.layer = shardLayer;
			}
			else
			{
				gameObject2 = UnityEngine.Object.Instantiate(shardPrefab);
				meshFilter = gameObject2.GetComponent<MeshFilter>();
				meshRenderer = gameObject2.GetComponent<MeshRenderer>();
				meshCollider = gameObject2.GetComponent<MeshCollider>();
				collisionAudioSensor = gameObject2.GetComponent<CollisionAudioSensor>();
			}
			gameObject2.SetActive(value: false);
			gameObject2.name = "cell" + n;
			if (meshFilter == null)
			{
				meshFilter = gameObject2.AddComponent<MeshFilter>();
			}
			meshFilter.mesh = mesh;
			if (meshRenderer == null)
			{
				meshRenderer = gameObject2.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = material;
			}
			if (meshCollider == null)
			{
				meshCollider = gameObject2.AddComponent<MeshCollider>();
			}
			Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
			rigidbody.mass = Mathf.Max(4f, ((!NetGame.isNetStarted) ? 1f : 0.6f) * num32 * densityPerSqMeter);
			meshCollider.convex = true;
			meshCollider.sharedMesh = mesh;
			meshCollider.sharedMaterial = physicsMaterial;
			if (collisionAudioSensor == null)
			{
				collisionAudioSensor = gameObject2.AddComponent<CollisionAudioSensor>();
			}
			collisionAudioSensor.pitch = Mathf.Clamp(10f / rigidbody.mass, 0.9f, 1.1f);
			Vector3 vector19 = (vector12 - vector) * 10f;
			vector19.z = Mathf.Lerp((vector12 - vector).sqrMagnitude, 100f, 0f);
			float num34 = Mathf.Clamp(value, 0f, maxShardVelocity * rigidbody.mass);
			gameObject2.transform.SetParent(shardParent.transform, worldPositionStays: false);
			gameObject2.transform.localPosition = To3D(zero) + boxCollider.center;
			gameObject2.SetActive(value: true);
			gameObject2.GetComponent<Rigidbody>().SafeAddForceAtPosition(-normalized * num34, (3f * contactPoint + To3D(vector12)) / 4f, ForceMode.Impulse);
			NetIdentity netIdentity = gameObject2.AddComponent<NetIdentity>();
			netIdentity.sceneId = (uint)n;
			gameObject2.AddComponent<NetBody>().Start();
			shardParent.AddIdentity(netIdentity);
			cells.Add(gameObject2);
		}
		shardParent.StartNetwork(repopulate: false);
		if (parentObject != null)
		{
			gameObject.transform.SetParent(parentObject);
		}
	}

	public override void ResetState(int checkpoint, int subObjectives)
	{
		if (resetOnReload)
		{
			base.ResetState(checkpoint, subObjectives);
			for (int i = 0; i < cells.Count; i++)
			{
				shardParent.RemoveIdentity(cells[i].GetComponent<NetIdentity>());
				UnityEngine.Object.Destroy(cells[i]);
			}
			cells.Clear();
			if (shardParent != null)
			{
				UnityEngine.Object.Destroy(shardParent.gameObject);
			}
		}
	}
}
