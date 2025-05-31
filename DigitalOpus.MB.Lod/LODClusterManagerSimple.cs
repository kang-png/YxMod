using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODClusterManagerSimple : LODClusterManager
{
	public LODClusterManagerSimple(MB2_LODManager.BakerPrototype bp)
		: base(bp)
	{
		clusters.Add(new LODClusterSimple(this));
	}

	public override LODCluster GetClusterFor(Vector3 p)
	{
		return clusters[0];
	}

	public override void RemoveCluster(Bounds c)
	{
		Debug.LogWarning("Cannot remove clusters from ClusterManagerSimple");
	}

	public override void DrawGizmos()
	{
	}
}
