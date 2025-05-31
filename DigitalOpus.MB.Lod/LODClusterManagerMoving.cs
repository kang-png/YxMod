using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODClusterManagerMoving : LODClusterManager
{
	public LODClusterManagerMoving(MB2_LODManager.BakerPrototype bp)
		: base(bp)
	{
		clusters.Add(new LODClusterMoving(this));
	}

	public override LODCluster GetClusterFor(Vector3 p)
	{
		return clusters[0];
	}

	public override void RemoveCluster(Bounds c)
	{
		Debug.LogWarning("Cannot remove clusters from ClusterManagerMoving");
	}

	public virtual void UpdateBounds()
	{
		Debug.LogWarning("Updating bounds.");
		((LODClusterMoving)clusters[0]).UpdateBounds();
	}
}
