using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace DigitalOpus.MB.Lod;

public class LODCheckScheduler
{
	public bool FORCE_CHECK_EVERY_FRAME;

	private Vector3[] lastCameraPositions;

	private bool containsMultipleCells;

	private bool containsMovingClusters;

	private float sqrDistThreashold;

	private float minGridSize;

	private MB2_LODManager manager;

	private int nextFrameCheckOffset;

	private float lastSheduleUpdateTime;

	public int GetNextFrameCheckOffset()
	{
		if (nextFrameCheckOffset >= 1000)
		{
			nextFrameCheckOffset = 0;
		}
		return nextFrameCheckOffset++;
	}

	public void Init(MB2_LODManager m)
	{
		manager = m;
		if (manager.LOG_LEVEL >= MB2_LogLevel.debug)
		{
			Debug.Log("Init called for LODCheckScheduler.");
		}
		containsMultipleCells = false;
		containsMovingClusters = false;
		minGridSize = float.PositiveInfinity;
		for (int i = 0; i < manager.bakers.Length; i++)
		{
			if (manager.bakers[i].clusterType != MB2_LODManager.BakerPrototype.CombinerType.simple)
			{
				containsMultipleCells = true;
				if (manager.bakers[i].gridSize < minGridSize)
				{
					minGridSize = manager.bakers[i].gridSize;
				}
			}
			if (manager.bakers[i].clusterType == MB2_LODManager.BakerPrototype.CombinerType.moving)
			{
				containsMovingClusters = true;
			}
		}
		if (containsMultipleCells)
		{
			sqrDistThreashold = minGridSize / 1.5f * (minGridSize / 1.5f);
			InitializeLastCameraPositions(manager.GetCameras());
		}
	}

	private void InitializeLastCameraPositions(MB2_LODCamera[] cams)
	{
		lastCameraPositions = new Vector3[cams.Length];
		for (int i = 0; i < lastCameraPositions.Length; i++)
		{
			ref Vector3 reference = ref lastCameraPositions[i];
			reference = new Vector3(1E+16f, 1E+16f, 1E+16f);
		}
	}

	private void UpdateClusterSchedules()
	{
		if (manager.LOG_LEVEL >= MB2_LogLevel.debug)
		{
			Debug.Log("Updating cluster lodcheck schedules.");
		}
		for (int i = 0; i < manager.bakers.Length; i++)
		{
			LODClusterManager baker = manager.bakers[i].baker;
			for (int j = 0; j < baker.clusters.Count; j++)
			{
				LODCluster lODCluster = baker.clusters[j];
				int numFramesBetweenChecks = GetNumFramesBetweenChecks(lODCluster);
				int num = int.MaxValue;
				List<LODCombinedMesh> combiners = lODCluster.GetCombiners();
				for (int k = 0; k < combiners.Count; k++)
				{
					if (combiners[k].numFramesBetweenChecksOffset == -1)
					{
						if (manager.LOG_LEVEL >= MB2_LogLevel.trace)
						{
							Debug.Log("fm=" + Time.frameCount + " calling cluster Update");
						}
						combiners[k].Update();
					}
					combiners[k].numFramesBetweenChecks = numFramesBetweenChecks;
					combiners[k].numFramesBetweenChecksOffset = GetNextFrameCheckOffset();
					int num2 = combiners[k].numFramesBetweenChecks - (Time.frameCount + combiners[k].numFramesBetweenChecksOffset) % combiners[k].numFramesBetweenChecks;
					if (num2 < num)
					{
						num = num2;
					}
				}
				lODCluster.nextCheckFrame = Time.frameCount + num;
			}
		}
		lastSheduleUpdateTime = Time.time;
	}

	public int GetNumFramesBetweenChecks(LODCluster cell)
	{
		int result = -1;
		if (cell is LODClusterGrid || cell is LODClusterMoving)
		{
			float num = Mathf.Sqrt(manager.GetDistanceSqrToClosestPerspectiveCamera(cell.Center()));
			MB2_LODManager.BakerPrototype bakerPrototype = cell.GetClusterManager().GetBakerPrototype();
			result = bakerPrototype.numFramesBetweenLODChecks;
			int num2 = Mathf.FloorToInt(num / (bakerPrototype.gridSize * 0.5f)) + 1;
			result *= num2;
		}
		else if (cell is LODClusterSimple)
		{
			MB2_LODManager.BakerPrototype bakerPrototype2 = cell.GetClusterManager().GetBakerPrototype();
			result = bakerPrototype2.numFramesBetweenLODChecks;
		}
		else
		{
			Debug.LogError("Should never get here.");
		}
		return result;
	}

	private void _UpdateClusterSchedulesIfCameraHasMoved()
	{
		MB2_LODCamera[] cameras = manager.GetCameras();
		if (cameras.Length != lastCameraPositions.Length)
		{
			InitializeLastCameraPositions(cameras);
		}
		bool flag = false;
		for (int i = 0; i < lastCameraPositions.Length; i++)
		{
			Vector3 position = cameras[i].transform.position;
			Vector3 vector = position - lastCameraPositions[i];
			if (Vector3.Dot(vector, vector) > sqrDistThreashold)
			{
				UpdateClusterSchedules();
				flag = true;
				for (int j = 0; j < cameras.Length; j++)
				{
					ref Vector3 reference = ref lastCameraPositions[j];
					reference = cameras[j].transform.position;
				}
				break;
			}
		}
		if (containsMovingClusters && !flag && Time.time - lastSheduleUpdateTime > 1f)
		{
			UpdateClusterSchedules();
			flag = true;
			for (int k = 0; k < cameras.Length; k++)
			{
				ref Vector3 reference2 = ref lastCameraPositions[k];
				reference2 = cameras[k].transform.position;
			}
		}
	}

	public void CheckIfLODsNeedToChange()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (containsMultipleCells)
		{
			_UpdateClusterSchedulesIfCameraHasMoved();
		}
		for (int i = 0; i < manager.bakers.Length; i++)
		{
			LODClusterManager baker = manager.bakers[i].baker;
			if (!(baker is LODClusterManagerSimple))
			{
				containsMultipleCells = true;
			}
			if (baker is LODClusterManagerMoving)
			{
				containsMovingClusters = true;
			}
			for (int j = 0; j < baker.clusters.Count; j++)
			{
				LODCluster lODCluster = baker.clusters[j];
				if (FORCE_CHECK_EVERY_FRAME || lODCluster.nextCheckFrame == Time.frameCount)
				{
					if (lODCluster is LODClusterMoving)
					{
						((LODClusterMoving)lODCluster).UpdateBounds();
					}
					int num = int.MaxValue;
					List<LODCombinedMesh> combiners = lODCluster.GetCombiners();
					for (int k = 0; k < combiners.Count; k++)
					{
						LODCombinedMesh lODCombinedMesh = combiners[k];
						bool flag = false;
						if (lODCombinedMesh.numFramesBetweenChecks == -1)
						{
							lODCombinedMesh.numFramesBetweenChecks = GetNumFramesBetweenChecks(lODCluster);
							lODCombinedMesh.numFramesBetweenChecksOffset = GetNextFrameCheckOffset();
							flag = true;
						}
						int num2 = lODCombinedMesh.numFramesBetweenChecks - (Time.frameCount + lODCombinedMesh.numFramesBetweenChecksOffset) % lODCombinedMesh.numFramesBetweenChecks;
						if (FORCE_CHECK_EVERY_FRAME || flag || num2 == lODCombinedMesh.numFramesBetweenChecks)
						{
							if (manager.LOG_LEVEL >= MB2_LogLevel.trace)
							{
								Debug.Log("fm=" + Time.frameCount + " calling cluster Update");
							}
							lODCombinedMesh.Update();
						}
						if (num2 < num)
						{
							num = num2;
						}
					}
					lODCluster.nextCheckFrame = Time.frameCount + num;
				}
				if (lODCluster.nextCheckFrame < Time.frameCount)
				{
					Debug.LogError(Time.frameCount + " Error somehow bypassed a frame when checking. " + lODCluster.nextCheckFrame);
				}
			}
		}
		manager.statLastCheckLODNeedToChangeTime = Time.realtimeSinceStartup - realtimeSinceStartup;
		manager.statTotalCheckLODNeedToChangeTime += manager.statLastCheckLODNeedToChangeTime;
	}

	public void ForceCheckIfLODsNeedToChange()
	{
		if (containsMultipleCells)
		{
			_UpdateClusterSchedulesIfCameraHasMoved();
		}
		for (int i = 0; i < manager.bakers.Length; i++)
		{
			LODClusterManager baker = manager.bakers[i].baker;
			for (int j = 0; j < baker.clusters.Count; j++)
			{
				LODCluster lODCluster = baker.clusters[j];
				List<LODCombinedMesh> combiners = lODCluster.GetCombiners();
				for (int k = 0; k < combiners.Count; k++)
				{
					LODCombinedMesh lODCombinedMesh = combiners[k];
					lODCombinedMesh.Update();
				}
			}
		}
	}
}
