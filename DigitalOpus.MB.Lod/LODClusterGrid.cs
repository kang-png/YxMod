using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODClusterGrid : LODClusterBase
{
	public Bounds b;

	public bool isVisible;

	public float distSquaredToPlayer = float.PositiveInfinity;

	public int lastPrePrioritizeFrame = -1;

	public LODClusterGrid(Bounds b, LODClusterManagerGrid m)
		: base(m)
	{
		this.b = b;
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

	public override string ToString()
	{
		return "LODClusterGrid " + b.ToString();
	}

	public override void UpdateSkinnedMeshApproximateBounds()
	{
		Debug.LogError("Grid clusters cannot be used for skinned meshes");
	}

	public void _TranslateCluster(Vector3 translation)
	{
		b.center += translation;
		for (int i = 0; i < combinedMeshes.Count; i++)
		{
			LODCombinedMesh lODCombinedMesh = combinedMeshes[i];
			lODCombinedMesh._TranslateLODs(translation);
		}
	}
}
