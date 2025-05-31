using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace DigitalOpus.MB.Lod;

public abstract class LODClusterManager
{
	public MB2_LogLevel _LOG_LEVEL = MB2_LogLevel.info;

	public MB2_LODManager.BakerPrototype _bakerPrototype;

	public List<LODCluster> clusters = new List<LODCluster>();

	protected List<LODCombinedMesh> recycledClusters = new List<LODCombinedMesh>();

	public MB2_LogLevel LOG_LEVEL
	{
		get
		{
			return _LOG_LEVEL;
		}
		set
		{
			_LOG_LEVEL = value;
		}
	}

	public LODClusterManager(MB2_LODManager.BakerPrototype bp)
	{
		_bakerPrototype = bp;
	}

	public virtual MB2_LODManager.BakerPrototype GetBakerPrototype()
	{
		return _bakerPrototype;
	}

	public virtual void Destroy()
	{
		for (int num = clusters.Count - 1; num >= 0; num--)
		{
			clusters[num].Destroy();
		}
		clusters.Clear();
		recycledClusters.Clear();
	}

	public virtual LODCluster GetClusterContaining(Vector3 v)
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			if (clusters[i].Contains(v))
			{
				return clusters[i];
			}
		}
		return null;
	}

	public virtual void RemoveCluster(Bounds b)
	{
		LODCluster clusterIntersecting = GetClusterIntersecting(b);
		if (clusterIntersecting != null)
		{
			clusterIntersecting.Clear();
			clusters.Remove(clusterIntersecting);
		}
	}

	public virtual void Clear()
	{
		for (int num = clusters.Count - 1; num >= 0; num--)
		{
			clusters[num].Clear();
			clusters.RemoveAt(num);
		}
	}

	public virtual void RecycleCluster(LODCombinedMesh c)
	{
		if (c != null)
		{
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.Log(MB2_LogLevel.debug, "LODClusterManagerGrid.RecycleCluster", LOG_LEVEL);
			}
			c.Clear();
			c.cluster = null;
			if (!recycledClusters.Contains(c))
			{
				recycledClusters.Add(c);
			}
			if (c.combinedMesh.resultSceneObject != null)
			{
				MB2_Version.SetActiveRecursively(c.combinedMesh.resultSceneObject, isActive: false);
			}
		}
	}

	public virtual void DrawGizmos()
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			clusters[i].DrawGizmos();
		}
	}

	public virtual void CheckIntegrity()
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			clusters[i].CheckIntegrity();
		}
		for (int j = 0; j < recycledClusters.Count; j++)
		{
			recycledClusters[j].CheckIntegrity();
		}
	}

	public virtual LODCombinedMesh GetFreshCombiner(LODCluster cell)
	{
		LODCombinedMesh lODCombinedMesh = null;
		if (recycledClusters.Count > 0)
		{
			lODCombinedMesh = recycledClusters[recycledClusters.Count - 1];
			recycledClusters.RemoveAt(recycledClusters.Count - 1);
			lODCombinedMesh.SetLODCluster(cell);
		}
		else
		{
			lODCombinedMesh = new LODCombinedMesh(_bakerPrototype.meshBaker, cell);
		}
		if (lODCombinedMesh.combinedMesh.resultSceneObject != null)
		{
			MB2_Version.SetActiveRecursively(lODCombinedMesh.combinedMesh.resultSceneObject, isActive: true);
		}
		cell.AddCombiner(lODCombinedMesh);
		lODCombinedMesh.numFramesBetweenChecks = -1;
		lODCombinedMesh.numFramesBetweenChecksOffset = -1;
		if (lODCombinedMesh.combinedMesh != null && lODCombinedMesh.combinedMesh.resultSceneObject != null)
		{
			lODCombinedMesh.combinedMesh.resultSceneObject.name = lODCombinedMesh.combinedMesh.resultSceneObject.name.Replace("-recycled", string.Empty);
		}
		cell.nextCheckFrame = Time.frameCount + 1;
		return lODCombinedMesh;
	}

	public virtual void UpdateSkinnedMeshApproximateBounds()
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			clusters[i].UpdateSkinnedMeshApproximateBounds();
		}
	}

	public abstract LODCluster GetClusterFor(Vector3 p);

	public virtual void ForceCheckIfLODsChanged()
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			clusters[i].ForceCheckIfLODsChanged();
		}
	}

	private LODCluster GetClusterIntersecting(Bounds b)
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			if (clusters[i].Intersects(b))
			{
				return clusters[i];
			}
		}
		return null;
	}
}
