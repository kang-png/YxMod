using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODClusterMoving : LODClusterBase
{
	public Bounds b;

	public bool isVisible;

	public float distSquaredToPlayer = float.PositiveInfinity;

	public int lastPrePrioritizeFrame = -1;

	public LODClusterMoving(LODClusterManagerMoving m)
		: base(m)
	{
	}

	public override bool Contains(Vector3 v)
	{
		return b.Contains(v);
	}

	public override bool Intersects(Bounds b)
	{
		return b.Intersects(b);
	}

	public override bool Intersects(Plane[][] fustrum)
	{
		for (int i = 0; i < fustrum.Length; i++)
		{
			if (GeometryUtility.TestPlanesAABB(fustrum[i], b))
			{
				return true;
			}
		}
		return false;
	}

	public override Vector3 Center()
	{
		return b.center;
	}

	public override bool IsVisible()
	{
		return isVisible;
	}

	public override float DistSquaredToPlayer()
	{
		return distSquaredToPlayer;
	}

	public override void PrePrioritize(Plane[][] fustrum, Vector3[] cameraPositions)
	{
		if (lastPrePrioritizeFrame == Time.frameCount)
		{
			return;
		}
		isVisible = false;
		distSquaredToPlayer = float.PositiveInfinity;
		for (int i = 0; i < cameraPositions.Length; i++)
		{
			float sqrMagnitude = (cameraPositions[i] - b.center).sqrMagnitude;
			if (distSquaredToPlayer > sqrMagnitude)
			{
				distSquaredToPlayer = sqrMagnitude;
			}
		}
		lastPrePrioritizeFrame = Time.frameCount;
	}

	public override void DrawGizmos()
	{
		Gizmos.DrawWireCube(b.center, b.size);
	}

	public virtual void UpdateBounds()
	{
		b = combinedMeshes[0].CalcBounds();
		for (int i = 1; i < combinedMeshes.Count; i++)
		{
			b.Encapsulate(combinedMeshes[i].CalcBounds());
		}
	}

	public override void UpdateSkinnedMeshApproximateBounds()
	{
		for (int i = 0; i < combinedMeshes.Count; i++)
		{
			combinedMeshes[i].UpdateSkinnedMeshApproximateBounds();
		}
	}
}
