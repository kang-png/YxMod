using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODClusterSimple : LODClusterBase
{
	public LODClusterSimple(LODClusterManagerSimple m)
		: base(m)
	{
	}

	public override bool Contains(Vector3 v)
	{
		return true;
	}

	public override bool Intersects(Bounds b)
	{
		return true;
	}

	public override bool Intersects(Plane[][] fustrum)
	{
		return true;
	}

	public override bool IsVisible()
	{
		return true;
	}

	public override float DistSquaredToPlayer()
	{
		return 0f;
	}

	public override Vector3 Center()
	{
		return Vector3.zero;
	}

	public override void DrawGizmos()
	{
	}

	public override void UpdateSkinnedMeshApproximateBounds()
	{
		for (int i = 0; i < combinedMeshes.Count; i++)
		{
			combinedMeshes[i].UpdateSkinnedMeshApproximateBounds();
		}
	}
}
