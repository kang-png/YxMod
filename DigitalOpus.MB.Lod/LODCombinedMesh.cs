using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODCombinedMesh
{
	public struct Transaction
	{
		public MB2_LODOperation action;

		public MB2_LOD lod;

		public int toIdx;

		public int inMeshGameObjectID;

		public int inMeshNumVerts;
	}

	public class LODCombinerSplitterMerger
	{
		private class LODHierarchy
		{
			public MB2_LOD rootLod;

			public List<MB2_LOD> lods = new List<MB2_LOD>();

			public int numVerts;

			public LODHierarchy(MB2_LOD root)
			{
				rootLod = root;
			}

			public void ComputeNumberOfVertices()
			{
				numVerts = 0;
				for (int i = 0; i < lods.Count; i++)
				{
					MB2_LOD mB2_LOD = lods[i];
					if (mB2_LOD != null)
					{
						if ((mB2_LOD.isInQueue && mB2_LOD.action == MB2_LODOperation.toAdd) || mB2_LOD.action == MB2_LODOperation.update)
						{
							numVerts += mB2_LOD.GetNumVerts(mB2_LOD.nextLevelIdx);
						}
						if (mB2_LOD.isInCombined && (!mB2_LOD.isInQueue || mB2_LOD.action != MB2_LODOperation.delete))
						{
							numVerts += mB2_LOD.GetNumVerts(mB2_LOD.currentLevelIdx);
						}
					}
				}
			}
		}

		private class NewCombiner
		{
			public List<LODHierarchy> lods;

			public LODCombinedMesh combiner;

			public int numVerts;

			public NewCombiner(LODCombinedMesh c)
			{
				lods = new List<LODHierarchy>();
				combiner = c;
				numVerts = c.GetNumVertsInMesh() + c.GetApproxNetVertsInQs();
			}
		}

		public static MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		public static void MergeCombiner(LODCluster cell)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (cell.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.debug || LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("=========== Merging Combiner " + cell);
			}
			List<LODCombinedMesh> combiners = cell.GetCombiners();
			if (combiners.Count < 2)
			{
				return;
			}
			combiners.Sort(new LODCombinerFullComparer());
			if (LOG_LEVEL == MB2_LogLevel.trace)
			{
				Debug.Log("ratios before" + PrintFullRatios(combiners));
			}
			LODCombinedMesh lODCombinedMesh = combiners[0];
			int i = 1;
			int num = lODCombinedMesh.GetNumVertsInMesh() + lODCombinedMesh.GetApproxNetVertsInQs();
			if (LOG_LEVEL == MB2_LogLevel.trace)
			{
				Debug.Log("============ numCombiners before " + lODCombinedMesh.cluster.GetCombiners().Count);
			}
			for (; i < combiners.Count; i++)
			{
				if (num >= lODCombinedMesh.combinedMesh.maxVertsInMesh)
				{
					break;
				}
				LODCombinedMesh lODCombinedMesh2 = combiners[i];
				if (num + lODCombinedMesh2.GetNumVertsInMesh() + lODCombinedMesh2.GetApproxNetVertsInQs() > lODCombinedMesh.combinedMesh.maxVertsInMesh)
				{
					break;
				}
				num += lODCombinedMesh2.GetNumVertsInMesh() + lODCombinedMesh2.GetApproxNetVertsInQs();
				List<MB2_LOD> list = new List<MB2_LOD>(lODCombinedMesh2.gosAssignedToMe);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].ForceRemove();
				}
				lODCombinedMesh2.ClearBake();
				for (int k = 0; k < list.Count; k++)
				{
					MB2_LOD mB2_LOD = list[k];
					lODCombinedMesh2.UnassignFromCombiner(mB2_LOD);
					mB2_LOD.SetCombiner(lODCombinedMesh);
					lODCombinedMesh.AssignToCombiner(mB2_LOD);
					mB2_LOD.ForceAdd();
				}
				cell.RemoveAndRecycleCombiner(lODCombinedMesh2);
			}
			if (LOG_LEVEL == MB2_LogLevel.trace)
			{
				Debug.Log("========= numCombiners after " + lODCombinedMesh.cluster.GetCombiners().Count);
			}
			if (i > 1)
			{
				lODCombinedMesh.BakeClusterCombiner();
			}
			float num2 = Time.realtimeSinceStartup - realtimeSinceStartup;
			MB2_LODManager.Manager().statLastMergeTime = num2;
			if (lODCombinedMesh.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.debug || LOG_LEVEL >= MB2_LogLevel.debug)
			{
				if (LOG_LEVEL == MB2_LogLevel.trace)
				{
					Debug.Log("ratios after " + PrintFullRatios(cell.GetCombiners()));
				}
				MB2_Log.LogDebug("=========== Done Merging Cluster merged {0} clusters in {1} sec", i - 1, num2);
			}
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				lODCombinedMesh.cluster.CheckIntegrity();
			}
		}

		public static void SplitCombiner(LODCombinedMesh src)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				src.GetLODCluster().CheckIntegrity();
			}
			if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.debug || LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug("=============== Splitting Combiner " + src.GetLODCluster());
				if (LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("ratios before " + PrintFullRatios(src.GetLODCluster().GetCombiners()));
				}
			}
			Dictionary<MB2_LOD, LODHierarchy> dictionary = new Dictionary<MB2_LOD, LODHierarchy>();
			foreach (MB2_LOD item in src.gosAssignedToMe)
			{
				MB2_LOD hierarchyRoot = item.GetHierarchyRoot();
				if (!dictionary.TryGetValue(hierarchyRoot, out var value))
				{
					value = new LODHierarchy(hierarchyRoot);
					dictionary.Add(hierarchyRoot, value);
				}
				value.lods.Add(item);
			}
			if (dictionary.Count == 1)
			{
				return;
			}
			if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
			{
				MB2_Log.LogDebug("Splitting Combiner found " + dictionary.Count + " hierarchies");
			}
			int num = 0;
			foreach (LODHierarchy value2 in dictionary.Values)
			{
				value2.ComputeNumberOfVertices();
				num += value2.numVerts;
			}
			int num2 = num / src.combinedMesh.maxVertsInMesh;
			if (num2 < 2)
			{
				num2 = 2;
			}
			NewCombiner[] array = new NewCombiner[num2];
			array[0] = new NewCombiner(src);
			array[0].numVerts = 0;
			List<LODCombinedMesh> combiners = src.GetLODCluster().GetCombiners();
			int i = 1;
			for (int j = 0; j < combiners.Count; j++)
			{
				if (i >= array.Length)
				{
					break;
				}
				if (combiners[j].GetFullRatio() < mergeCombinerThreshold)
				{
					array[i] = new NewCombiner(combiners[j]);
					i++;
				}
			}
			for (; i < array.Length; i++)
			{
				LODCombinedMesh freshCombiner = src.GetLODCluster().GetClusterManager().GetFreshCombiner(src.GetLODCluster());
				array[i] = new NewCombiner(freshCombiner);
			}
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				src.GetLODCluster().CheckIntegrity();
			}
			foreach (LODHierarchy value3 in dictionary.Values)
			{
				NewCombiner newCombiner = array[0];
				for (int k = 1; k < array.Length; k++)
				{
					if (array[k].numVerts < newCombiner.numVerts)
					{
						newCombiner = array[k];
					}
				}
				newCombiner.lods.Add(value3);
				newCombiner.numVerts += value3.numVerts;
			}
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				src.GetLODCluster().CheckIntegrity();
			}
			if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
			{
				for (int l = 0; l < array.Length; l++)
				{
					Debug.Log("distributed " + array[l].lods.Count + " to combiner " + l);
				}
			}
			foreach (NewCombiner newCombiner2 in array)
			{
				for (int n = 0; n < newCombiner2.lods.Count; n++)
				{
					LODHierarchy lODHierarchy = newCombiner2.lods[n];
					for (int num3 = 0; num3 < lODHierarchy.lods.Count; num3++)
					{
						MB2_LOD mB2_LOD = lODHierarchy.lods[num3];
						mB2_LOD.ForceRemove();
					}
				}
			}
			if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
			{
				MB2_Log.LogDebug("Clearing primary combiner");
			}
			array[0].combiner.ClearBake();
			for (int num4 = 0; num4 < array.Length; num4++)
			{
				NewCombiner newCombiner3 = array[num4];
				for (int num5 = 0; num5 < newCombiner3.lods.Count; num5++)
				{
					LODHierarchy lODHierarchy2 = newCombiner3.lods[num5];
					for (int num6 = 0; num6 < lODHierarchy2.lods.Count; num6++)
					{
						MB2_LOD mB2_LOD2 = lODHierarchy2.lods[num6];
						if (num4 >= 1)
						{
							mB2_LOD2.GetCombiner().UnassignFromCombiner(mB2_LOD2);
							mB2_LOD2.SetCombiner(newCombiner3.combiner);
							newCombiner3.combiner.AssignToCombiner(mB2_LOD2);
						}
					}
					lODHierarchy2.rootLod.ForceAdd();
				}
			}
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				src.GetLODCluster().CheckIntegrity();
			}
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
				{
					MB2_Log.LogDebug("Baking new combiner " + num7);
				}
				array[num7].combiner.BakeClusterCombiner();
				if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("baked " + num7 + " full ratio " + array[num7].combiner.GetFullRatio().ToString("F5"));
				}
			}
			float num8 = Time.realtimeSinceStartup - realtimeSinceStartup;
			MB2_LODManager.Manager().statLastSplitTime = num8;
			if (src.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.debug || LOG_LEVEL >= MB2_LogLevel.debug)
			{
				if (LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("ratios after " + PrintFullRatios(src.GetLODCluster().GetCombiners()));
				}
				MB2_Log.LogDebug("=================Done split combiners " + num8 + " ============");
			}
		}
	}

	private class LODCombinerFullComparer : IComparer<LODCombinedMesh>
	{
		int IComparer<LODCombinedMesh>.Compare(LODCombinedMesh a, LODCombinedMesh b)
		{
			return a.GetNumVertsInMesh() + a.GetApproxNetVertsInQs() - b.GetNumVertsInMesh() - b.GetApproxNetVertsInQs();
		}
	}

	protected static float splitCombinerThreshold = 2f;

	protected static float mergeCombinerThreshold = 0.3f;

	public MB3_MultiMeshCombiner combinedMesh;

	protected Dictionary<int, Transaction> lodTransactions;

	protected HashSet<MB2_LOD> gosInCombiner;

	protected HashSet<MB2_LOD> gosAssignedToMe;

	public int numFramesBetweenChecks;

	public int numFramesBetweenChecksOffset;

	protected int numBakeImmediately;

	public LODCluster cluster;

	public int numVertsInMesh;

	public int numApproxVertsInQ;

	protected bool wasTranslated;

	public LODCombinedMesh(MB3_MeshBaker meshBaker, LODCluster cell)
	{
		cluster = cell;
		combinedMesh = new MB3_MultiMeshCombiner();
		combinedMesh.maxVertsInMesh = cell.GetClusterManager().GetBakerPrototype().maxVerticesPerCombinedMesh;
		SetMBValues(meshBaker);
		lodTransactions = new Dictionary<int, Transaction>();
		gosInCombiner = new HashSet<MB2_LOD>();
		gosAssignedToMe = new HashSet<MB2_LOD>();
		numVertsInMesh = 0;
		numApproxVertsInQ = 0;
	}

	public virtual LODClusterManager GetClusterManager()
	{
		return cluster.GetClusterManager();
	}

	public void UpdateSkinnedMeshApproximateBounds()
	{
		if (combinedMesh.renderType != MB_RenderType.skinnedMeshRenderer)
		{
			Debug.LogWarning("Should not call UpdateSkinnedMeshApproximateBounds on a non skinned combined mesh");
			return;
		}
		for (int i = 0; i < combinedMesh.meshCombiners.Count; i++)
		{
			MB3_MeshCombiner mB3_MeshCombiner = combinedMesh.meshCombiners[i].combinedMesh;
			if (mB3_MeshCombiner != null)
			{
				combinedMesh.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBounds();
			}
		}
	}

	public virtual void ForceBakeImmediately()
	{
		if (numBakeImmediately == 0)
		{
			numBakeImmediately = 1;
		}
	}

	public virtual int GetNumVertsInMesh()
	{
		return numVertsInMesh;
	}

	public virtual int GetApproxNetVertsInQs()
	{
		return numApproxVertsInQ;
	}

	public virtual void SetLODCluster(LODCluster c)
	{
		cluster = c;
	}

	public virtual LODCluster GetLODCluster()
	{
		return cluster;
	}

	public virtual bool IsVisible()
	{
		return cluster.IsVisible();
	}

	public virtual int NumDirty()
	{
		return lodTransactions.Count;
	}

	public virtual int NumBakeImmediately()
	{
		return numBakeImmediately;
	}

	public virtual float DistSquaredToPlayer()
	{
		return cluster.DistSquaredToPlayer();
	}

	public void AssignToCombiner(MB2_LOD lod)
	{
		if (lod.GetCombiner() != this)
		{
			Debug.LogError("LOD was assigned to a different cluster.");
		}
		else
		{
			gosAssignedToMe.Add(lod);
		}
	}

	public void UnassignFromCombiner(MB2_LOD lod)
	{
		gosAssignedToMe.Remove(lod);
	}

	public bool IsAssignedToThis(MB2_LOD lod)
	{
		return gosAssignedToMe.Contains(lod);
	}

	private void _CancelTransaction(MB2_LOD lod)
	{
		if (!lod.isInQueue)
		{
			Debug.LogError("Cancel transaction should never be called for LODs not in Q");
		}
		Transaction transaction = default(Transaction);
		transaction.action = lod.action;
		transaction.toIdx = lod.nextLevelIdx;
		transaction.lod = lod;
		if (lodTransactions.TryGetValue(lod.gameObject.GetInstanceID(), out var value))
		{
			if (value.action == MB2_LODOperation.toAdd)
			{
				numApproxVertsInQ -= lod.GetNumVerts(value.toIdx);
			}
			if (value.action == MB2_LODOperation.update)
			{
				numApproxVertsInQ -= lod.GetNumVerts(value.toIdx);
				numApproxVertsInQ += lod.GetNumVerts(lod.currentLevelIdx);
			}
			if (value.action == MB2_LODOperation.delete)
			{
				numApproxVertsInQ += value.inMeshNumVerts;
			}
			lodTransactions.Remove(lod.gameObject.GetInstanceID());
			lod.OnRemoveFromQueue();
		}
		else
		{
			Debug.LogError("An LOD thought it was in the Q but it wasn't");
		}
	}

	private Transaction _AddTransaction(MB2_LOD lod)
	{
		Transaction transaction = default(Transaction);
		transaction.action = lod.action;
		transaction.toIdx = lod.nextLevelIdx;
		transaction.lod = lod;
		if (lod.isInCombined && lod.action == MB2_LODOperation.delete)
		{
			transaction.inMeshGameObjectID = lod.GetGameObjectID(lod.currentLevelIdx);
			transaction.inMeshNumVerts = lod.GetNumVerts(lod.currentLevelIdx);
		}
		if (lodTransactions.TryGetValue(lod.gameObject.GetInstanceID(), out var value))
		{
			if (value.action == MB2_LODOperation.toAdd)
			{
				numApproxVertsInQ -= lod.GetNumVerts(value.toIdx);
			}
			if (value.action == MB2_LODOperation.update)
			{
				numApproxVertsInQ -= lod.GetNumVerts(value.toIdx);
				numApproxVertsInQ += lod.GetNumVerts(lod.currentLevelIdx);
			}
			if (value.action == MB2_LODOperation.delete)
			{
				numApproxVertsInQ += value.inMeshNumVerts;
			}
		}
		if (lod.action == MB2_LODOperation.toAdd)
		{
			numApproxVertsInQ += lod.GetNumVerts(lod.nextLevelIdx);
		}
		if (lod.action == MB2_LODOperation.update)
		{
			numApproxVertsInQ -= lod.GetNumVerts(lod.currentLevelIdx);
			numApproxVertsInQ += lod.GetNumVerts(lod.nextLevelIdx);
		}
		if (lod.action == MB2_LODOperation.delete)
		{
			numApproxVertsInQ -= lod.GetNumVerts(lod.currentLevelIdx);
		}
		if (lod.action == MB2_LODOperation.delete && !lod.isInCombined)
		{
			lodTransactions.Remove(lod.gameObject.GetInstanceID());
			lod.OnRemoveFromQueue();
		}
		else
		{
			lodTransactions[lod.gameObject.GetInstanceID()] = transaction;
			lod.OnAddToQueue();
		}
		return transaction;
	}

	public void LODCancelTransaction(MB2_LOD lod)
	{
		if (lod.GetCombiner() != this)
		{
			Debug.LogError("Wrong combiner");
		}
		if (cluster.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
		{
			MB2_Log.Log(MB2_LogLevel.trace, string.Concat("LODManager.LODCancelTransaction ", lod, " action ", lod.action), MB2_LogLevel.trace);
		}
		_CancelTransaction(lod);
	}

	public void LODChanged(MB2_LOD lod, bool immediate)
	{
		if (lod.GetCombiner() != this)
		{
			Debug.LogError("Wrong combiner");
		}
		if (cluster.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
		{
			MB2_Log.Log(MB2_LogLevel.trace, string.Concat("LODManager.LODChanged ", lod, " action ", lod.action), MB2_LogLevel.trace);
		}
		_AddTransaction(lod);
		MB2_LODManager.Manager().AddDirtyCombinedMesh(this);
	}

	public void RemoveLOD(MB2_LOD lod, bool immediate = true)
	{
		if (cluster.GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
		{
			MB2_Log.Log(MB2_LogLevel.trace, "LODManager.RemoveLOD " + lod, MB2_LogLevel.trace);
		}
		if (!lod.isInQueue && !lod.isInCombined)
		{
			Debug.LogError("RemoveLOD: lod is not in combined or is in queue " + lod.isInQueue + " " + lod.isInCombined);
		}
		else if (lod.action != MB2_LODOperation.delete)
		{
			Debug.LogError("Action must be delete");
		}
		else
		{
			if (lod.isInQueue || lod.isInCombined)
			{
				_AddTransaction(lod);
			}
			MB2_LODManager.Manager().AddDirtyCombinedMesh(this);
		}
	}

	public void _TranslateLODs(Vector3 translation)
	{
		foreach (MB2_LOD item in gosAssignedToMe)
		{
			item._ResetPositionMarker();
		}
		if (combinedMesh.resultSceneObject != null)
		{
			Vector3 position = combinedMesh.resultSceneObject.transform.position;
			combinedMesh.resultSceneObject.transform.position = position + translation;
		}
		wasTranslated = true;
	}

	private float GetFullRatio()
	{
		int num = GetNumVertsInMesh() + GetApproxNetVertsInQs();
		return (float)num / (float)combinedMesh.maxVertsInMesh;
	}

	public virtual void Bake()
	{
		_BakeWithSplitAndMerge();
	}

	private void _BakeWithoutSplitAndMerge()
	{
		HashSet<LODCombinedMesh> hashSet = cluster.AdjustForMaxAllowedPerLevel();
		BakeClusterCombiner();
		if (hashSet == null)
		{
			return;
		}
		foreach (LODCombinedMesh item in hashSet)
		{
			if (item.cluster == this)
			{
				item.BakeClusterCombiner();
			}
		}
	}

	private void _BakeWithSplitAndMerge()
	{
		HashSet<LODCombinedMesh> hashSet = cluster.AdjustForMaxAllowedPerLevel();
		bool flag = false;
		float fullRatio = GetFullRatio();
		if (fullRatio > splitCombinerThreshold)
		{
			LODCombinerSplitterMerger.SplitCombiner(this);
			flag = true;
			MB2_LODManager.Manager().statNumSplit++;
		}
		else if (fullRatio < mergeCombinerThreshold)
		{
			List<LODCombinedMesh> combiners = cluster.GetCombiners();
			for (int i = 0; i < combiners.Count; i++)
			{
				if (combiners[i] != this && combiners[i].GetFullRatio() < mergeCombinerThreshold)
				{
					LODCombinerSplitterMerger.MergeCombiner(cluster);
					flag = true;
					MB2_LODManager.Manager().statNumMerge++;
					break;
				}
				fullRatio = GetFullRatio();
				if (fullRatio > mergeCombinerThreshold)
				{
					break;
				}
			}
		}
		if (!flag)
		{
			BakeClusterCombiner();
		}
		if (hashSet == null)
		{
			return;
		}
		foreach (LODCombinedMesh item in hashSet)
		{
			if (item.cluster == this)
			{
				item.BakeClusterCombiner();
			}
		}
	}

	public virtual void SetMBValues(MB3_MeshBaker mb)
	{
		MB3_MeshCombiner meshCombiner = mb.meshCombiner;
		combinedMesh.renderType = meshCombiner.renderType;
		combinedMesh.outputOption = MB2_OutputOptions.bakeIntoSceneObject;
		combinedMesh.lightmapOption = meshCombiner.lightmapOption;
		combinedMesh.textureBakeResults = meshCombiner.textureBakeResults;
		combinedMesh.doNorm = meshCombiner.doNorm;
		combinedMesh.doTan = meshCombiner.doTan;
		combinedMesh.doCol = meshCombiner.doCol;
		combinedMesh.doUV = meshCombiner.doUV;
		combinedMesh.doUV1 = meshCombiner.doUV1;
	}

	public virtual bool IsDirty()
	{
		if (lodTransactions.Count > 0)
		{
			return true;
		}
		return false;
	}

	public virtual void PrePrioritize(Plane[][] fustrum, Vector3[] cameraPositions)
	{
		if (cluster == null)
		{
			Debug.LogError("cluster is null");
		}
		if (fustrum == null)
		{
			Debug.LogError("fustrum is null");
		}
		if (cameraPositions == null)
		{
			Debug.LogError("camPositions null");
		}
		cluster.PrePrioritize(fustrum, cameraPositions);
	}

	public virtual Bounds CalcBounds()
	{
		Bounds result = new Bounds(Vector3.zero, Vector3.one);
		if (gosAssignedToMe.Count > 0)
		{
			bool flag = false;
			foreach (MB2_LOD item in gosAssignedToMe)
			{
				if (item != null && MB2_Version.GetActive(item.gameObject))
				{
					if (flag)
					{
						result.Encapsulate(item.transform.position);
						continue;
					}
					flag = true;
					float dimension = item.levels[0].dimension;
					result = new Bounds(item.transform.position, new Vector3(dimension, dimension, dimension));
				}
			}
			if (!flag && cluster.GetClusterManager()._LOG_LEVEL >= MB2_LogLevel.info)
			{
				Debug.Log("CalcBounds called on a CombinedMesh that contained no valid LODs");
			}
		}
		else if (cluster.GetClusterManager()._LOG_LEVEL >= MB2_LogLevel.info)
		{
			Debug.Log("CalcBounds called on a CombinedMesh that contained no valid LODs");
		}
		return result;
	}

	private void ClearBake()
	{
		lodTransactions.Clear();
		gosInCombiner.Clear();
		combinedMesh.AddDeleteGameObjects(null, combinedMesh.GetObjectsInCombined().ToArray());
		combinedMesh.Apply();
		numApproxVertsInQ = 0;
		numVertsInMesh = 0;
		numBakeImmediately = 0;
	}

	private void BakeClusterCombiner()
	{
		MB2_Log.Log(MB2_LogLevel.debug, $"Bake called on cluster numTransactions={lodTransactions.Count}", GetClusterManager().LOG_LEVEL);
		if (lodTransactions.Count <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		List<GameObject> list2 = new List<GameObject>();
		List<MB2_LOD> list3 = new List<MB2_LOD>();
		List<MB2_LOD> list4 = new List<MB2_LOD>();
		List<MB2_LOD> list5 = new List<MB2_LOD>();
		foreach (int key in lodTransactions.Keys)
		{
			Transaction transaction = lodTransactions[key];
			if (transaction.action == MB2_LODOperation.toAdd)
			{
				list2.Add(transaction.lod.GetRendererGameObject(transaction.toIdx));
				list3.Add(transaction.lod);
			}
			else if (transaction.action == MB2_LODOperation.update)
			{
				list.Add(transaction.lod.GetGameObjectID(transaction.lod.currentLevelIdx));
				list2.Add(transaction.lod.GetRendererGameObject(transaction.toIdx));
				list5.Add(transaction.lod);
			}
			else if (transaction.action == MB2_LODOperation.delete)
			{
				list.Add(transaction.inMeshGameObjectID);
				if (transaction.lod != null)
				{
					list4.Add(transaction.lod);
				}
			}
		}
		if (wasTranslated)
		{
			if (combinedMesh.resultSceneObject != null)
			{
				combinedMesh.resultSceneObject.transform.position = Vector3.zero;
			}
			foreach (MB2_LOD item in gosInCombiner)
			{
				if (item.isInCombined && !list3.Contains(item) && !list4.Contains(item) && !list5.Contains(item))
				{
					list2.Add(item.GetRendererGameObject(item.currentLevelIdx));
					list.Add(item.GetRendererGameObject(item.currentLevelIdx).GetInstanceID());
				}
			}
			wasTranslated = false;
		}
		combinedMesh.AddDeleteGameObjectsByID(list2.ToArray(), list.ToArray(), disableRendererInSource: true);
		combinedMesh.Apply();
		numApproxVertsInQ = 0;
		numVertsInMesh = 0;
		for (int i = 0; i < combinedMesh.meshCombiners.Count; i++)
		{
			numVertsInMesh += combinedMesh.meshCombiners[i].combinedMesh.GetMesh().vertexCount;
		}
		if (combinedMesh.resultSceneObject != null)
		{
			MB2_LODManager.BakerPrototype bakerPrototype = GetClusterManager().GetBakerPrototype();
			Transform transform = combinedMesh.resultSceneObject.transform;
			for (int j = 0; j < transform.childCount; j++)
			{
				GameObject gameObject = transform.GetChild(j).gameObject;
				gameObject.layer = bakerPrototype.layer;
				Renderer component = gameObject.GetComponent<Renderer>();
				component.shadowCastingMode = bakerPrototype.castShadow;
				component.receiveShadows = bakerPrototype.receiveShadow;
			}
		}
		MB2_LODManager mB2_LODManager = MB2_LODManager.Manager();
		mB2_LODManager.statTotalNumBakes++;
		mB2_LODManager.statLastNumBakes++;
		mB2_LODManager.statLastBakeFrame = Time.frameCount;
		if (combinedMesh.renderType == MB_RenderType.skinnedMeshRenderer)
		{
			for (int k = 0; k < combinedMesh.meshCombiners.Count; k++)
			{
				MB3_MeshCombiner mB3_MeshCombiner = combinedMesh.meshCombiners[k].combinedMesh;
				if (mB3_MeshCombiner != null)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)mB3_MeshCombiner.targetRenderer;
					bool updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
					skinnedMeshRenderer.updateWhenOffscreen = true;
					skinnedMeshRenderer.updateWhenOffscreen = updateWhenOffscreen;
				}
			}
		}
		lodTransactions.Clear();
		numBakeImmediately = 0;
		for (int l = 0; l < list4.Count; l++)
		{
			list4[l].OnBakeRemoved();
			gosInCombiner.Remove(list4[l]);
		}
		for (int m = 0; m < list3.Count; m++)
		{
			list3[m].OnBakeAdded();
			gosInCombiner.Add(list3[m]);
		}
		for (int n = 0; n < list5.Count; n++)
		{
			list5[n].OnBakeUpdated();
			gosInCombiner.Add(list5[n]);
		}
		if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.trace)
		{
			MB2_Log.Log(MB2_LogLevel.trace, "Bake complete, num in combined " + combinedMesh.GetNumObjectsInCombined() + " fullRatio:" + GetFullRatio().ToString("F5"), GetClusterManager().LOG_LEVEL);
		}
	}

	public virtual void Destroy()
	{
		combinedMesh.DestroyMesh();
		if (combinedMesh.resultSceneObject != null)
		{
			Object.Destroy(combinedMesh.resultSceneObject);
			combinedMesh.resultSceneObject = null;
		}
		cluster = null;
	}

	public virtual void GetObjectsThatWillBeInMesh(List<MB2_LOD> objsThatWillBeInMesh)
	{
		foreach (MB2_LOD item in gosInCombiner)
		{
			objsThatWillBeInMesh.Add(item);
		}
		foreach (int key in lodTransactions.Keys)
		{
			Transaction transaction = lodTransactions[key];
			if (transaction.action == MB2_LODOperation.toAdd)
			{
				objsThatWillBeInMesh.Add(transaction.lod);
			}
			if (transaction.action == MB2_LODOperation.delete)
			{
				objsThatWillBeInMesh.Remove(transaction.lod);
			}
		}
	}

	public virtual void Clear()
	{
		List<GameObject> objectsInCombined = combinedMesh.GetObjectsInCombined();
		if (GetClusterManager().LOG_LEVEL >= MB2_LogLevel.debug)
		{
			MB2_Log.Log(MB2_LogLevel.debug, "Clear called on grid cluster num in combined " + objectsInCombined.Count + " numTrans=" + lodTransactions.Count + " numAssignedToMe " + gosAssignedToMe.Count, GetClusterManager().LOG_LEVEL);
		}
		combinedMesh.AddDeleteGameObjects(null, objectsInCombined.ToArray());
		combinedMesh.Apply();
		foreach (MB2_LOD item in gosInCombiner)
		{
			if (item != null)
			{
				item.Clear();
			}
		}
		foreach (MB2_LOD item2 in gosAssignedToMe)
		{
			if (item2 != null)
			{
				item2.Clear();
			}
		}
		lodTransactions.Clear();
		gosInCombiner.Clear();
		gosAssignedToMe.Clear();
		numVertsInMesh = 0;
		numBakeImmediately = 0;
	}

	public virtual bool Contains(MB2_LOD lod)
	{
		if (lodTransactions.ContainsKey(lod.GetInstanceID()))
		{
			return true;
		}
		if (gosInCombiner.Contains(lod))
		{
			return true;
		}
		return false;
	}

	public virtual void CheckIntegrity()
	{
		foreach (MB2_LOD item in gosAssignedToMe)
		{
			if (item.GetCombiner() != this)
			{
				Debug.LogError(string.Concat("LOD ", item, " thinks it is in a different ", item.GetCombiner(), " than it is \n log dump", item.myLog.Dump()));
			}
		}
		foreach (MB2_LOD item2 in gosInCombiner)
		{
			if (!item2.isInCombined)
			{
				Debug.LogError(string.Concat(item2, "LOD thought it was in combined but wasn't\n log dump", item2.myLog.Dump()));
			}
			if (item2.action == MB2_LODOperation.toAdd)
			{
				Debug.LogError("bad lod action\n log dump" + item2.myLog.Dump());
			}
			if (!gosAssignedToMe.Contains(item2))
			{
				Debug.LogError(string.Concat("in combiner was not in assigned ", item2.GetCombiner(), " an it is \n log dump", item2.myLog.Dump()));
			}
		}
		List<GameObject> objectsInCombined = combinedMesh.GetObjectsInCombined();
		for (int i = 0; i < objectsInCombined.Count; i++)
		{
			if (objectsInCombined[i] != null)
			{
				MB2_LOD componentInAncestor = MB2_LODManager.GetComponentInAncestor<MB2_LOD>(objectsInCombined[i].transform);
				if (componentInAncestor == null)
				{
					Debug.LogError("Couldn't find LOD for obj in combined mesh");
				}
				else if (!gosInCombiner.Contains(componentInAncestor))
				{
					Debug.LogError("lod was in combined mesh that is not in list of lods in cluster.");
				}
			}
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (int key in lodTransactions.Keys)
		{
			Transaction transaction = lodTransactions[key];
			if (transaction.action == MB2_LODOperation.toAdd)
			{
				if (transaction.lod.isInCombined)
				{
					Debug.LogError("Bad action");
				}
				num += transaction.lod.GetNumVerts(transaction.toIdx);
			}
			if (transaction.action == MB2_LODOperation.update)
			{
				if (!transaction.lod.isInCombined)
				{
					Debug.LogError("Bad action");
				}
				num += transaction.lod.GetNumVerts(transaction.toIdx);
				num2 += transaction.lod.GetNumVerts(transaction.lod.currentLevelIdx);
			}
			if (transaction.action == MB2_LODOperation.delete)
			{
				if (!transaction.lod.isInCombined)
				{
					Debug.LogError("Bad action");
				}
				num2 += transaction.lod.GetNumVerts(transaction.lod.currentLevelIdx);
			}
		}
		for (int j = 0; j < combinedMesh.meshCombiners.Count; j++)
		{
			num3 += combinedMesh.meshCombiners[j].combinedMesh.GetMesh().vertexCount;
		}
		if (num3 != numVertsInMesh)
		{
			Debug.LogError("Num verts in mesh don't match measured " + num3 + " thought " + numVertsInMesh);
		}
		if (num - num2 != numApproxVertsInQ)
		{
			Debug.LogError("Num verts in Q don't match measured " + (num - num2) + " thought " + numApproxVertsInQ);
		}
	}

	public void Update()
	{
		List<MB2_LOD> list = null;
		foreach (MB2_LOD item in gosAssignedToMe)
		{
			if (item == null)
			{
				if (list == null)
				{
					list = new List<MB2_LOD>();
				}
				list.Add(item);
			}
			else if (item.enabled && MB2_Version.GetActive(item.gameObject))
			{
				item.CheckIfLODsNeedToChange();
			}
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				gosAssignedToMe.Remove(list[i]);
			}
		}
	}

	public static string PrintFullRatios(List<LODCombinedMesh> cls)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < cls.Count; i++)
		{
			stringBuilder.AppendFormat("{0} full ratio {1} numObjs {2} numMeshes {3}\n", i, cls[i].GetFullRatio().ToString("F5"), cls[i].gosInCombiner.Count, cls[i].combinedMesh.meshCombiners.Count);
		}
		return stringBuilder.ToString();
	}
}
