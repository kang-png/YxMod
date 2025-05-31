using DigitalOpus.MB.Core;
using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODClusterManagerGrid : LODClusterManager
{
	public int _gridSize = 250;

	public int gridSize
	{
		get
		{
			return _gridSize;
		}
		set
		{
			if (clusters.Count > 0)
			{
				MB2_Log.Log(MB2_LogLevel.error, "Can't change the gridSize once clusters exist.", base.LOG_LEVEL);
			}
			else
			{
				_gridSize = value;
			}
		}
	}

	public LODClusterManagerGrid(MB2_LODManager.BakerPrototype bp)
		: base(bp)
	{
	}

	public override LODCluster GetClusterFor(Vector3 p)
	{
		LODCluster lODCluster = GetClusterContaining(p);
		if (lODCluster == null)
		{
			lODCluster = new LODClusterGrid(new Bounds(new Vector3((float)_gridSize * Mathf.Round(p.x / (float)_gridSize), (float)_gridSize * Mathf.Round(p.y / (float)_gridSize), (float)_gridSize * Mathf.Round(p.z / (float)_gridSize)), new Vector3(_gridSize, _gridSize, _gridSize)), this);
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				lODCluster.CheckIntegrity();
			}
			clusters.Add(lODCluster);
			if (base.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.Log(MB2_LogLevel.debug, string.Concat("Created new cell ", lODCluster, " to contain point ", p), base.LOG_LEVEL);
			}
		}
		return lODCluster;
	}

	public void TranslateAllClusters(Vector3 translation)
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			LODClusterGrid lODClusterGrid = (LODClusterGrid)clusters[i];
			lODClusterGrid._TranslateCluster(translation);
		}
	}
}
