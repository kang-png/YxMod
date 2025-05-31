using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace DigitalOpus.MB.Lod;

public abstract class LODClusterBase : LODCluster
{
	public LODClusterManager manager;

	private int _nextCheckFrame;

	private int _lastAdjustForMaxAllowedFrame;

	protected List<LODCombinedMesh> combinedMeshes = new List<LODCombinedMesh>();

	public int nextCheckFrame
	{
		get
		{
			return _nextCheckFrame;
		}
		set
		{
			_nextCheckFrame = value;
		}
	}

	public LODClusterBase(LODClusterManager m)
	{
		manager = m;
		_lastAdjustForMaxAllowedFrame = -1;
		manager.GetFreshCombiner(this);
	}

	public List<LODCombinedMesh> GetCombiners()
	{
		return new List<LODCombinedMesh>(combinedMeshes);
	}

	public abstract bool Contains(Vector3 v);

	public abstract bool Intersects(Bounds b);

	public abstract bool Intersects(Plane[][] fustrum);

	public abstract Vector3 Center();

	public abstract void DrawGizmos();

	public abstract bool IsVisible();

	public abstract float DistSquaredToPlayer();

	public virtual void Destroy()
	{
		for (int num = combinedMeshes.Count - 1; num >= 0; num--)
		{
			combinedMeshes[num].Destroy();
		}
	}

	public virtual void Clear()
	{
		for (int num = combinedMeshes.Count - 1; num >= 0; num--)
		{
			combinedMeshes[num].Clear();
			combinedMeshes[num].combinedMesh.resultSceneObject.name = combinedMeshes[num].combinedMesh.resultSceneObject.name + "-recycled";
			manager.RecycleCluster(combinedMeshes[num]);
		}
	}

	public virtual void CheckIntegrity()
	{
		for (int i = 0; i < combinedMeshes.Count; i++)
		{
			combinedMeshes[i].CheckIntegrity();
			if (combinedMeshes[i].GetLODCluster() != this)
			{
				Debug.LogError("Cluster was a child of this cell " + i + " but its parent was another cell. num " + combinedMeshes.Count + " " + combinedMeshes[i].GetLODCluster());
			}
			for (int j = 0; j < combinedMeshes.Count; j++)
			{
				if (i != j && combinedMeshes[i] == combinedMeshes[j])
				{
					Debug.LogError("same cluster has been added twice.");
				}
			}
		}
	}

	public virtual LODClusterManager GetClusterManager()
	{
		return manager;
	}

	public virtual void RemoveAndRecycleCombiner(LODCombinedMesh cl)
	{
		combinedMeshes.Remove(cl);
		if (combinedMeshes.Contains(cl))
		{
			Debug.LogError("removed but still contains.");
		}
		manager.RecycleCluster(cl);
	}

	public virtual void AddCombiner(LODCombinedMesh cl)
	{
		if (!combinedMeshes.Contains(cl))
		{
			combinedMeshes.Add(cl);
		}
		else
		{
			Debug.LogError("error in AddCombiner");
		}
	}

	public virtual LODCombinedMesh SuggestCombiner()
	{
		LODCombinedMesh lODCombinedMesh = combinedMeshes[0];
		int num = lODCombinedMesh.GetNumVertsInMesh() + lODCombinedMesh.GetApproxNetVertsInQs();
		for (int i = 1; i < combinedMeshes.Count; i++)
		{
			int num2 = combinedMeshes[i].GetNumVertsInMesh() + combinedMeshes[i].GetApproxNetVertsInQs();
			if (num > num2)
			{
				num = num2;
				lODCombinedMesh = combinedMeshes[i];
			}
		}
		return lODCombinedMesh;
	}

	public virtual void AssignLODToCombiner(MB2_LOD l)
	{
		if (MB2_LODManager.CHECK_INTEGRITY && !combinedMeshes.Contains(l.GetCombiner()))
		{
			Debug.LogError(string.Concat("Error in AssignLODToCombiner ", l, " combiner ", l.GetCombiner(), " is not in this LODCluster this=", this, " other=", l.GetCombiner().GetLODCluster()));
		}
		l.GetCombiner().AssignToCombiner(l);
	}

	public virtual void UpdateSkinnedMeshApproximateBounds()
	{
		Debug.LogError("Grid combinedMeshes cannot be used for skinned meshes");
	}

	public virtual void PrePrioritize(Plane[][] fustrum, Vector3[] cameraPositions)
	{
	}

	public virtual HashSet<LODCombinedMesh> AdjustForMaxAllowedPerLevel()
	{
		if (_lastAdjustForMaxAllowedFrame == Time.frameCount)
		{
			return null;
		}
		int[] maxNumberPerLevel = manager.GetBakerPrototype().maxNumberPerLevel;
		if (maxNumberPerLevel == null || maxNumberPerLevel.Length == 0)
		{
			return null;
		}
		HashSet<LODCombinedMesh> hashSet = new HashSet<LODCombinedMesh>();
		List<MB2_LOD> list = new List<MB2_LOD>();
		for (int i = 0; i < combinedMeshes.Count; i++)
		{
			combinedMeshes[i].GetObjectsThatWillBeInMesh(list);
		}
		list.Sort(new MB2_LOD.MB2_LODDistToCamComparer());
		HashSet<MB2_LOD>[] array = new HashSet<MB2_LOD>[maxNumberPerLevel.Length];
		int num = 0;
		for (int j = 0; j < array.Length; j++)
		{
			num += maxNumberPerLevel[j];
			array[j] = new HashSet<MB2_LOD>();
		}
		HashSet<MB2_LOD> hashSet2 = new HashSet<MB2_LOD>();
		int num2 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			int nextLevelIdx = list[k].nextLevelIdx;
			bool flag = false;
			if (nextLevelIdx < array.Length && num2 < num)
			{
				for (int l = nextLevelIdx; l < array.Length; l++)
				{
					if (array[l].Count < maxNumberPerLevel[l])
					{
						array[l].Add(list[k]);
						num2++;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				hashSet2.Add(list[k]);
			}
		}
		if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.debug)
		{
			string text = $"AdjustForMaxAllowedPerLevel objsThatWillBeInMesh={list.Count}\n";
			for (int m = 0; m < array.Length; m++)
			{
				text += $"b{m} capacity={maxNumberPerLevel[m]} contains={array[m].Count}\n";
			}
			text += $"b[leftovers] contains={hashSet2.Count}\n";
			MB2_Log.Log(MB2_LogLevel.info, text, GetClusterManager().LOG_LEVEL);
		}
		for (int n = 1; n < array.Length; n++)
		{
			foreach (MB2_LOD item in array[n])
			{
				if (item.nextLevelIdx == n)
				{
					continue;
				}
				if (n >= item.levels.Length)
				{
					if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
					{
						MB2_Log.Log(MB2_LogLevel.trace, $"A Demoting obj in bucket={n} obj={item}", GetClusterManager().LOG_LEVEL);
					}
					item.AdjustNextLevelIndex(n);
					hashSet.Add(item.GetCombiner());
				}
				else
				{
					if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
					{
						MB2_Log.Log(MB2_LogLevel.trace, $"B Demoting obj in bucket={n} obj={item}", GetClusterManager().LOG_LEVEL);
					}
					item.AdjustNextLevelIndex(n);
					hashSet.Add(item.GetCombiner());
				}
			}
		}
		int num3 = array.Length - 1;
		foreach (MB2_LOD item2 in hashSet2)
		{
			if (item2.nextLevelIdx > num3)
			{
				continue;
			}
			if (num3 >= item2.levels.Length - 1)
			{
				if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
				{
					MB2_Log.Log(MB2_LogLevel.trace, $"C Demoting obj in bucket={num3 + 1} obj={item2}", GetClusterManager().LOG_LEVEL);
				}
				item2.AdjustNextLevelIndex(num3 + 1);
				hashSet.Add(item2.GetCombiner());
			}
			else
			{
				if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
				{
					MB2_Log.Log(MB2_LogLevel.trace, $"D Demoting obj in bucket={num3 + 1} obj={item2}", GetClusterManager().LOG_LEVEL);
				}
				item2.AdjustNextLevelIndex(num3 + 1);
				hashSet.Add(item2.GetCombiner());
			}
		}
		_lastAdjustForMaxAllowedFrame = Time.frameCount;
		return hashSet;
	}

	public virtual void ForceCheckIfLODsChanged()
	{
		for (int i = 0; i < combinedMeshes.Count; i++)
		{
			combinedMeshes[i].Update();
		}
	}
}
