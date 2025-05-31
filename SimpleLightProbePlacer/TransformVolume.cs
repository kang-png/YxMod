using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleLightProbePlacer;

[AddComponentMenu("")]
public class TransformVolume : MonoBehaviour
{
	[SerializeField]
	private Volume m_volume = new Volume(Vector3.zero, Vector3.one);

	public Volume Volume
	{
		get
		{
			return m_volume;
		}
		set
		{
			m_volume = value;
		}
	}

	public Vector3 Origin => m_volume.Origin;

	public Vector3 Size => m_volume.Size;

	public bool IsInBounds(Vector3[] points)
	{
		return GetBounds().Intersects(GetBounds(points));
	}

	public bool IsOnBorder(Vector3[] points)
	{
		if (points.All((Vector3 x) => !IsInVolume(x)))
		{
			return false;
		}
		return !points.All(IsInVolume);
	}

	public bool IsInVolume(Vector3[] points)
	{
		return points.All(IsInVolume);
	}

	public bool IsInVolume(Vector3 position)
	{
		for (int i = 0; i < 6; i++)
		{
			if (new Plane(GetSideDirection(i), GetSidePosition(i)).GetSide(position))
			{
				return false;
			}
		}
		return true;
	}

	public Vector3[] GetCorners()
	{
		Vector3[] array = new Vector3[8]
		{
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, -0.5f)
		};
		for (int i = 0; i < array.Length; i++)
		{
			array[i].x *= m_volume.Size.x;
			array[i].y *= m_volume.Size.y;
			array[i].z *= m_volume.Size.z;
			ref Vector3 reference = ref array[i];
			reference = base.transform.TransformPoint(m_volume.Origin + array[i]);
		}
		return array;
	}

	public Bounds GetBounds()
	{
		return GetBounds(GetCorners());
	}

	public Bounds GetBounds(Vector3[] points)
	{
		Vector3 center = points.Aggregate(Vector3.zero, (Vector3 result, Vector3 point) => result + point) / points.Length;
		Bounds result2 = new Bounds(center, Vector3.zero);
		for (int i = 0; i < points.Length; i++)
		{
			result2.Encapsulate(points[i]);
		}
		return result2;
	}

	public GameObject[] GetGameObjectsInBounds(LayerMask layerMask)
	{
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
		List<GameObject> list = new List<GameObject>();
		Bounds bounds = GetBounds();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].gameObject == base.transform.gameObject) && !(array[i].GetComponent<TransformVolume>() != null) && ((1 << array[i].gameObject.layer) & layerMask.value) != 0 && bounds.Intersects(array[i].bounds))
			{
				list.Add(array[i].gameObject);
			}
		}
		return list.ToArray();
	}

	public Vector3 GetSideDirection(int side)
	{
		Vector3[] array = new Vector3[6];
		Vector3 right = Vector3.right;
		Vector3 up = Vector3.up;
		Vector3 forward = Vector3.forward;
		array[0] = right;
		ref Vector3 reference = ref array[1];
		reference = -right;
		array[2] = up;
		ref Vector3 reference2 = ref array[3];
		reference2 = -up;
		array[4] = forward;
		ref Vector3 reference3 = ref array[5];
		reference3 = -forward;
		return base.transform.TransformDirection(array[side]);
	}

	public Vector3 GetSidePosition(int side)
	{
		Vector3[] array = new Vector3[6];
		Vector3 right = Vector3.right;
		Vector3 up = Vector3.up;
		Vector3 forward = Vector3.forward;
		array[0] = right;
		ref Vector3 reference = ref array[1];
		reference = -right;
		array[2] = up;
		ref Vector3 reference2 = ref array[3];
		reference2 = -up;
		array[4] = forward;
		ref Vector3 reference3 = ref array[5];
		reference3 = -forward;
		return base.transform.TransformPoint(array[side] * GetSizeAxis(side) + m_volume.Origin);
	}

	public float GetSizeAxis(int side)
	{
		switch (side)
		{
		case 0:
		case 1:
			return m_volume.Size.x * 0.5f;
		case 2:
		case 3:
			return m_volume.Size.y * 0.5f;
		default:
			return m_volume.Size.z * 0.5f;
		}
	}
}
