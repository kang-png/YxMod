using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using DigitalOpus.MB.Lod;
using UnityEngine;
using UnityEngine.Rendering;

public class MB2_LODManager : MonoBehaviour
{
	public enum ChangeType
	{
		changeAdd,
		changeRemove,
		changeUpdate
	}

	public struct BakeDiagnostic
	{
		public int frame;

		public int deltaTime;

		public int bakeTime;

		public int checkLODNeedToChangeTime;

		public int gcTime;

		public int numCombinedMeshsBaked;

		public int numCombinedMeshsInQ;

		public BakeDiagnostic(MB2_LODManager manager)
		{
			frame = Time.frameCount;
			numCombinedMeshsInQ = manager.dirtyCombinedMeshes.Count;
			deltaTime = 0;
			if (manager.statLastBakeFrame == frame)
			{
				bakeTime = (int)(manager.statLastCombinedMeshBakeTime * 1000f);
				numCombinedMeshsBaked = manager.statLastNumBakes;
			}
			else
			{
				bakeTime = 0;
				numCombinedMeshsBaked = 0;
			}
			checkLODNeedToChangeTime = (int)(manager.statLastCheckLODNeedToChangeTime * 1000f);
			if (manager.statLastGCFrame == frame)
			{
				gcTime = (int)(manager.statLastGarbageCollectionTime * 1000f);
			}
			else
			{
				gcTime = 0;
			}
		}

		public static string PrettyPrint(BakeDiagnostic[] data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("--------------------------------------------");
			stringBuilder.AppendLine("Frame  deltaTime  numBakes  bakeTime  gcTime  checkTime  numInQ");
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].numCombinedMeshsBaked > 0)
				{
					stringBuilder.AppendFormat("{0}:     {1}       {2}       {3}       {4}       {5}       {6}\n", data[i].frame.ToString().PadLeft(4), data[i].deltaTime.ToString().PadLeft(4), data[i].numCombinedMeshsBaked.ToString().PadLeft(4), data[i].bakeTime.ToString().PadLeft(4), data[i].gcTime.ToString().PadLeft(4), data[i].checkLODNeedToChangeTime.ToString().PadLeft(4), data[i].numCombinedMeshsInQ.ToString().PadLeft(4));
				}
				else
				{
					stringBuilder.AppendFormat("{0}:     {1}       {2}        -        {3}       {4}       {5}\n", data[i].frame.ToString().PadLeft(4), data[i].deltaTime.ToString().PadLeft(4), data[i].bakeTime.ToString().PadLeft(4), data[i].gcTime.ToString().PadLeft(4), data[i].checkLODNeedToChangeTime.ToString().PadLeft(4), data[i].numCombinedMeshsInQ.ToString().PadLeft(4));
				}
			}
			stringBuilder.AppendLine("--------------------------------------------");
			return stringBuilder.ToString();
		}
	}

	[Serializable]
	public class BakerPrototype
	{
		public enum CombinerType
		{
			grid,
			simple,
			moving
		}

		public MB3_MeshBaker meshBaker;

		public int lightMapIndex = -1;

		public int layer = 1;

		public ShadowCastingMode castShadow = ShadowCastingMode.On;

		public bool receiveShadow = true;

		public string label = string.Empty;

		public CombinerType clusterType;

		public int maxVerticesPerCombinedMesh = 32000;

		public float gridSize = 250f;

		public HashSet<Material> materials = new HashSet<Material>();

		public int[] maxNumberPerLevel = new int[0];

		public bool updateSkinnedMeshApproximateBounds;

		public int numFramesBetweenLODChecks = 20;

		[NonSerialized]
		public LODClusterManager baker;

		public bool Initialize(BakerPrototype[] bakers)
		{
			if (meshBaker == null)
			{
				Debug.LogError("Baker does not have a MeshBaker assigned. Create a 'Mesh and Material Baker'.and assign it to this baker.");
				return false;
			}
			if (maxVerticesPerCombinedMesh < 3)
			{
				Debug.LogError("Baker maxVerticesPerCombinedMesh must be greater than 3.");
				return false;
			}
			if (gridSize <= 0f && clusterType == CombinerType.grid)
			{
				Debug.LogError("Baker gridSize must be greater than zero.");
				return false;
			}
			if (meshBaker.textureBakeResults == null || meshBaker.textureBakeResults.materialsAndUVRects == null || meshBaker.textureBakeResults.materialsAndUVRects.Length == 0)
			{
				Debug.LogError("Baker does not have a texture bake result or the texture bake result contains no materials. Assign a texture bake result.");
				return false;
			}
			if (meshBaker.meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer && clusterType == CombinerType.simple && !updateSkinnedMeshApproximateBounds)
			{
				Debug.Log("You are combining skinned meshes but Update Skinned Mesh Approximate bounds is not checked. You should check this setting if your meshes can move outside the fixed bounds or your meshes may vanish unexpectedly.");
			}
			if (numFramesBetweenLODChecks < 1)
			{
				Debug.LogError("'Num Frames Between LOD Checks' must be greater than zero.");
				return false;
			}
			if (materials == null)
			{
				materials = new HashSet<Material>();
			}
			MB2_TextureBakeResults textureBakeResults = meshBaker.textureBakeResults;
			for (int i = 0; i < textureBakeResults.materialsAndUVRects.Length; i++)
			{
				if (textureBakeResults.materialsAndUVRects[i].material != null && textureBakeResults.materialsAndUVRects[i].material.shader != null)
				{
					materials.Add(textureBakeResults.materialsAndUVRects[i].material);
					continue;
				}
				Debug.LogError("One of the materials or shaders is null in prototype ");
				return false;
			}
			int[] array = maxNumberPerLevel;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] < 0)
				{
					Debug.LogError("Max Number Per Level values must be positive");
					array[j] = 1000;
				}
			}
			for (int k = 0; k < bakers.Length; k++)
			{
				if (bakers[k] != this && bakers[k].label.Length > 0 && label.Equals(bakers[k].label))
				{
					Debug.LogError("Bakers have duplicate label" + bakers[k].label);
					return false;
				}
				if (bakers[k] != this && (materials.Overlaps(bakers[k].materials) & (bakers[k].label.Length == 0) & (label.Length == 0)) && bakers[k].materials.Count != 1)
				{
					Debug.LogWarning("Bakers " + k + " share materials with another baker. Assigning LOD objects to bakers may be ambiguous. Try setting labels to resolve conflicts.");
				}
			}
			CreateClusterManager();
			return true;
		}

		public void CreateClusterManager()
		{
			LODClusterManager lODClusterManager = null;
			if (clusterType == CombinerType.grid)
			{
				lODClusterManager = new LODClusterManagerGrid(this);
				((LODClusterManagerGrid)lODClusterManager).gridSize = (int)gridSize;
			}
			else if (clusterType == CombinerType.simple)
			{
				lODClusterManager = new LODClusterManagerSimple(this);
			}
			else if (clusterType == CombinerType.moving)
			{
				lODClusterManager = new LODClusterManagerMoving(this);
			}
			baker = lODClusterManager;
		}

		public void Clear()
		{
			if (baker != null)
			{
				baker.Clear();
			}
		}
	}

	private static MB2_LODManager _manager;

	private static int destroyedFrameCount = -1;

	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	public static bool ENABLED = true;

	public static bool CHECK_INTEGRITY;

	public bool baking_enabled = true;

	public float maxCombineTimePerFrame = 0.03f;

	public bool ignoreLightmapping = true;

	public BakerPrototype[] bakers;

	public LODCheckScheduler checkScheduler;

	private Dictionary<LODCombinedMesh, LODCombinedMesh> dirtyCombinedMeshes = new Dictionary<LODCombinedMesh, LODCombinedMesh>();

	private MB2_LODCamera[] lodCameras;

	public IComparer<LODCombinedMesh> combinedMeshPriorityComparer = new MB2_LODClusterComparer();

	public List<MB2_LOD> limbo = new List<MB2_LOD>();

	public int numBakersPerGC = 2;

	private int bakesSinceLastGC;

	private int GCcollectionCount;

	private bool isSetup;

	public BakeDiagnostic[] frameInfo;

	[HideInInspector]
	public int statTotalNumBakes;

	[HideInInspector]
	public float statAveCombinedMeshBakeTime = 0.03f;

	[HideInInspector]
	public float statMaxCombinedMeshBakeTime;

	[HideInInspector]
	public float statMinCombinedMeshBakeTime = 100f;

	[HideInInspector]
	public float statTotalCombinedMeshBakeTime = 0.03f;

	[HideInInspector]
	public float statLastCombinedMeshBakeTime;

	[HideInInspector]
	public int statLastNumBakes;

	[HideInInspector]
	public int statLastGCFrame;

	[HideInInspector]
	public float statLastGarbageCollectionTime;

	[HideInInspector]
	public float statTotalGarbageCollectionTime;

	[HideInInspector]
	public int statLastBakeFrame;

	[HideInInspector]
	public int statNumDirty;

	[HideInInspector]
	public int statNumSplit;

	[HideInInspector]
	public int statNumMerge;

	[HideInInspector]
	public float statLastMergeTime;

	[HideInInspector]
	public float statLastSplitTime;

	[HideInInspector]
	public float statLastCheckLODNeedToChangeTime;

	[HideInInspector]
	public float statTotalCheckLODNeedToChangeTime;

	public static MB2_LODManager Manager()
	{
		if (destroyedFrameCount == Time.frameCount)
		{
			return null;
		}
		if (_manager == null)
		{
			MB2_LODManager[] array = (MB2_LODManager[])UnityEngine.Object.FindObjectsOfType(typeof(MB2_LODManager));
			if (array == null || array.Length == 0)
			{
				Debug.LogError("There were MB2_LOD scripts in the scene that couldn't find an MB2_LODManager in the scene. Try dragging the LODManager prefab into the scene and configuring some bakers.");
			}
			else if (array.Length > 1)
			{
				Debug.LogError("There was more than one LODManager object found in the scene.");
				_manager = null;
			}
			else
			{
				_manager = array[0];
			}
		}
		return _manager;
	}

	private void Awake()
	{
		destroyedFrameCount = -1;
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			frameInfo = new BakeDiagnostic[30];
		}
		_Setup();
	}

	private void _Setup()
	{
		if (isSetup)
		{
			return;
		}
		checkScheduler = new LODCheckScheduler();
		checkScheduler.Init(this);
		if (bakers.Length == 0)
		{
			Debug.LogWarning("LOD Manager has no bakers. LOD objects will not be added to any combined meshes.");
		}
		if (numBakersPerGC <= 0)
		{
			MB2_Log.Log(MB2_LogLevel.info, "LOD Manager Number of Bakes before gargage collection is less than one. Garbage collector will never be run by the LOD Manager.", LOG_LEVEL);
		}
		if (maxCombineTimePerFrame <= 0f)
		{
			Debug.LogError("Combine Time Per Frame must be greater than zero.");
		}
		dirtyCombinedMeshes.Clear();
		for (int i = 0; i < bakers.Length; i++)
		{
			if (!bakers[i].Initialize(bakers))
			{
				ENABLED = false;
				return;
			}
		}
		MB2_Log.Log(MB2_LogLevel.info, "LODManager.Start called initialized " + bakers.Length + " bakers", LOG_LEVEL);
		UpdateMeshesThatNeedToChange();
		isSetup = true;
	}

	public void SetupHierarchy(MB2_LOD lod)
	{
		if (!isSetup)
		{
			_Setup();
		}
		if (isSetup)
		{
			lod.SetupHierarchy(bakers, ignoreLightmapping);
		}
	}

	public void RemoveClustersIntersecting(Bounds bnds)
	{
		if (ENABLED)
		{
			MB2_Log.Log(MB2_LogLevel.debug, "MB2_LODManager.RemoveClustersIntersecting " + bnds, LOG_LEVEL);
			for (int i = 0; i < bakers.Length; i++)
			{
				bakers[i].baker.RemoveCluster(bnds);
			}
		}
	}

	public void AddDirtyCombinedMesh(LODCombinedMesh c)
	{
		if (!dirtyCombinedMeshes.ContainsKey(c))
		{
			dirtyCombinedMeshes.Add(c, c);
		}
	}

	private void Update()
	{
		checkScheduler.CheckIfLODsNeedToChange();
	}

	private void LateUpdate()
	{
		if (GC.CollectionCount(0) > GCcollectionCount)
		{
			GCcollectionCount = GC.CollectionCount(0);
		}
		UpdateMeshesThatNeedToChange();
		DestroyObjectsInLimbo();
		UpdateSkinnedMeshApproximateBoundsIfNecessary();
		if (LOG_LEVEL == MB2_LogLevel.debug)
		{
			if (frameInfo == null)
			{
				frameInfo = new BakeDiagnostic[30];
			}
			int num = Time.frameCount % 30;
			ref BakeDiagnostic reference = ref frameInfo[num];
			reference = new BakeDiagnostic(this);
			if (num == 0)
			{
				frameInfo[29].deltaTime = (int)(Time.deltaTime * 1000f);
			}
			else
			{
				frameInfo[num - 1].deltaTime = (int)(Time.deltaTime * 1000f);
			}
			if (num == 29)
			{
				Debug.Log(BakeDiagnostic.PrettyPrint(frameInfo));
			}
		}
	}

	private void UpdateMeshesThatNeedToChange()
	{
		if (destroyedFrameCount == Time.frameCount || !ENABLED || !baking_enabled || lodCameras == null || lodCameras.Length == 0)
		{
			return;
		}
		statNumDirty = dirtyCombinedMeshes.Count;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (bakesSinceLastGC > numBakersPerGC)
		{
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			GC.Collect();
			statLastGarbageCollectionTime = Time.realtimeSinceStartup - realtimeSinceStartup2;
			statTotalGarbageCollectionTime += statLastGarbageCollectionTime;
			statLastGCFrame = Time.frameCount;
			bakesSinceLastGC = 0;
		}
		float num = 0f;
		if (statNumDirty > 0)
		{
			List<LODCombinedMesh> list = PrioritizeCombinedMeshs();
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				MB2_Log.Log(MB2_LogLevel.trace, $"LODManager.UpdateMeshesThatNeedToChange called. dirty clusters= {list.Count}", LOG_LEVEL);
			}
			statLastNumBakes = 0;
			int num2 = list.Count - 1;
			while (num2 >= 0 && list[num2].NumBakeImmediately() > 0)
			{
				float realtimeSinceStartup3 = Time.realtimeSinceStartup;
				LODCombinedMesh lODCombinedMesh = list[num2];
				if (lODCombinedMesh.cluster == null)
				{
					list.RemoveAt(num2);
				}
				else
				{
					lODCombinedMesh.Bake();
					if (!lODCombinedMesh.IsDirty())
					{
						dirtyCombinedMeshes.Remove(lODCombinedMesh);
					}
					float num3 = Time.realtimeSinceStartup - realtimeSinceStartup3;
					if (num3 > statMaxCombinedMeshBakeTime)
					{
						statMaxCombinedMeshBakeTime = num3;
					}
					if (num3 < statMinCombinedMeshBakeTime)
					{
						statMinCombinedMeshBakeTime = num3;
					}
					statAveCombinedMeshBakeTime = statAveCombinedMeshBakeTime * ((float)statTotalNumBakes - 1f) / (float)statTotalNumBakes + num3 / (float)statTotalNumBakes;
					num += num3;
				}
				num2--;
			}
			while (Time.realtimeSinceStartup - realtimeSinceStartup < maxCombineTimePerFrame && num2 >= 0)
			{
				float realtimeSinceStartup4 = Time.realtimeSinceStartup;
				if (list[num2].cluster == null)
				{
					list.RemoveAt(num2);
				}
				else
				{
					list[num2].Bake();
					if (!list[num2].IsDirty())
					{
						dirtyCombinedMeshes.Remove(list[num2]);
					}
					float num4 = Time.realtimeSinceStartup - realtimeSinceStartup4;
					if (num4 > statMaxCombinedMeshBakeTime)
					{
						statMaxCombinedMeshBakeTime = num4;
					}
					if (num4 < statMinCombinedMeshBakeTime)
					{
						statMinCombinedMeshBakeTime = num4;
					}
					statAveCombinedMeshBakeTime = statAveCombinedMeshBakeTime * ((float)statTotalNumBakes - 1f) / (float)statTotalNumBakes + num4 / (float)statTotalNumBakes;
					num += num4;
				}
				num2--;
			}
			bakesSinceLastGC += statLastNumBakes;
			statLastCombinedMeshBakeTime = num;
			statTotalCombinedMeshBakeTime += statLastCombinedMeshBakeTime;
		}
		if (CHECK_INTEGRITY)
		{
			checkIntegrity();
		}
	}

	private List<LODCombinedMesh> PrioritizeCombinedMeshs()
	{
		Plane[][] array = new Plane[lodCameras.Length][];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = GeometryUtility.CalculateFrustumPlanes(lodCameras[i].GetComponent<Camera>());
		}
		Vector3[] array2 = new Vector3[lodCameras.Length];
		for (int j = 0; j < array2.Length; j++)
		{
			ref Vector3 reference = ref array2[j];
			reference = lodCameras[j].transform.position;
		}
		List<LODCombinedMesh> list = new List<LODCombinedMesh>(dirtyCombinedMeshes.Keys);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].cluster == null)
			{
				list.RemoveAt(num);
			}
			else
			{
				list[num].PrePrioritize(array, array2);
			}
		}
		list.Sort(combinedMeshPriorityComparer);
		return list;
	}

	private void checkIntegrity()
	{
		for (int i = 0; i < bakers.Length; i++)
		{
			bakers[i].baker.CheckIntegrity();
		}
	}

	private void printSet(HashSet<Material> s)
	{
		IEnumerator enumerator = s.GetEnumerator();
		Debug.Log("== Set =====");
		while (enumerator.MoveNext())
		{
			Debug.Log(enumerator.Current);
		}
	}

	private void OnDestroy()
	{
		destroyedFrameCount = Time.frameCount;
		MB2_Log.Log(MB2_LogLevel.debug, "Destroying LODManager", LOG_LEVEL);
		for (int i = 0; i < bakers.Length; i++)
		{
			if (bakers[i].baker != null)
			{
				bakers[i].baker.Destroy();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!ENABLED || bakers == null)
		{
			return;
		}
		for (int i = 0; i < bakers.Length; i++)
		{
			if (bakers[i].baker != null)
			{
				bakers[i].baker.DrawGizmos();
			}
		}
	}

	public string GetStats()
	{
		string empty = string.Empty;
		string text = empty;
		empty = text + "statTotalNumBakes=" + statTotalNumBakes + "\n";
		text = empty;
		empty = text + "statTotalNumSplit=" + statNumSplit + "\n";
		text = empty;
		empty = text + "statTotalNumMerge=" + statNumMerge + "\n";
		text = empty;
		empty = text + "statAveCombinedMeshBakeTime=" + statAveCombinedMeshBakeTime + "\n";
		text = empty;
		empty = text + "statMaxCombinedMeshBakeTime=" + statMaxCombinedMeshBakeTime + "\n";
		text = empty;
		empty = text + "statMinCombinedMeshBakeTime=" + statMinCombinedMeshBakeTime + "\n";
		text = empty;
		empty = text + "statTotalGarbageCollectionTime=" + statTotalGarbageCollectionTime + "\n";
		text = empty;
		empty = text + "statTotalCombinedMeshBakeTime=" + statTotalCombinedMeshBakeTime + "\n";
		text = empty;
		empty = text + "statTotalCheckLODNeedToChangeTime=" + statTotalCheckLODNeedToChangeTime + "\n";
		text = empty;
		empty = text + "statLastSplitTime=" + statLastSplitTime + "\n";
		text = empty;
		empty = text + "statLastMergeTime=" + statLastMergeTime + "\n";
		text = empty;
		empty = text + "statLastGarbageCollectionTime=" + statLastGarbageCollectionTime + "\n";
		text = empty;
		empty = text + "statLastBakeFrame=" + statLastBakeFrame + "\n";
		text = empty;
		return text + "statNumDirty=" + statNumDirty + "\n";
	}

	public static T GetComponentInAncestor<T>(Transform tt, bool highest = false) where T : Component
	{
		Transform transform = tt;
		if (highest)
		{
			T result = (T)null;
			while (transform != null)
			{
				T component = transform.GetComponent<T>();
				if (component != null)
				{
					result = component;
				}
				if (transform == transform.root)
				{
					break;
				}
				transform = transform.parent;
			}
			return result;
		}
		while (transform != null && transform.parent != transform)
		{
			T component2 = transform.GetComponent<T>();
			if (component2 != null)
			{
				return component2;
			}
			transform = transform.parent;
		}
		return (T)null;
	}

	public MB2_LODCamera[] GetCameras()
	{
		if (lodCameras == null)
		{
			MB2_LODCamera[] array = (MB2_LODCamera[])MB2_Version.FindSceneObjectsOfType(typeof(MB2_LODCamera));
			if (array.Length == 0)
			{
				MB2_Log.Log(MB2_LogLevel.error, "There was no cameras in the scene with an MB2_LOD camera script attached", LOG_LEVEL);
			}
			else
			{
				lodCameras = array;
			}
		}
		return lodCameras;
	}

	public void AddBaker(BakerPrototype bp)
	{
		if (bp.Initialize(bakers))
		{
			BakerPrototype[] array = new BakerPrototype[bakers.Length + 1];
			Array.Copy(bakers, array, bakers.Length);
			array[array.Length - 1] = bp;
			bakers = array;
			Debug.Log((bakers[0] == bp) + " a " + bakers[0].Equals(bp));
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.Log(MB2_LogLevel.debug, "Adding Baker to LODManager.", LOG_LEVEL);
			}
		}
	}

	public void RemoveBaker(BakerPrototype bp)
	{
		List<BakerPrototype> list = new List<BakerPrototype>();
		list.AddRange(bakers);
		Debug.Log((bakers[0] == bp) + " " + bakers[0].Equals(bp));
		if (list.Contains(bp))
		{
			bp.Clear();
			list.Remove(bp);
			bakers = list.ToArray();
			if (LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.Log(MB2_LogLevel.debug, "Found BP and removed", LOG_LEVEL);
			}
		}
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			MB2_Log.Log(MB2_LogLevel.debug, "Remving Baker from LODManager.", LOG_LEVEL);
		}
	}

	public void AddCamera(MB2_LODCamera cam)
	{
		MB2_LODCamera[] cameras = GetCameras();
		for (int i = 0; i < cameras.Length; i++)
		{
			if (cam == cameras[i])
			{
				return;
			}
		}
		MB2_LODCamera[] array = new MB2_LODCamera[cameras.Length + 1];
		Array.Copy(cameras, array, cameras.Length);
		array[array.Length - 1] = cam;
		lodCameras = array;
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			MB2_Log.Log(MB2_LogLevel.debug, "MB2_LODManager.AddCamera added a camera length is now " + lodCameras.Length, LOG_LEVEL);
		}
	}

	public void RemoveCamera(MB2_LODCamera cam)
	{
		MB2_LODCamera[] cameras = GetCameras();
		List<MB2_LODCamera> list = new List<MB2_LODCamera>();
		list.AddRange(cameras);
		list.Remove(cam);
		lodCameras = list.ToArray();
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			MB2_Log.Log(MB2_LogLevel.debug, "MB2_LODManager.RemovedCamera removed a camera length is now " + lodCameras.Length, LOG_LEVEL);
		}
	}

	public void LODDestroy(MB2_LOD lodComponent)
	{
		lodComponent.SetWasDestroyedFlag();
		MB2_LOD[] components = lodComponent.GetComponents<MB2_LOD>();
		for (int i = 0; i < components.Length; i++)
		{
			if (lodComponent != components[i])
			{
				LODDestroy(components[i]);
			}
		}
		bool flag = false;
		if (!lodComponent.isInCombined)
		{
			UnityEngine.Object.Destroy(lodComponent.gameObject);
		}
		else
		{
			MB2_Version.SetActiveRecursively(lodComponent.gameObject, isActive: false);
			limbo.Add(lodComponent);
			flag = true;
		}
		if (LOG_LEVEL == MB2_LogLevel.trace)
		{
			MB2_Log.Log(MB2_LogLevel.trace, string.Concat("MB2_LODManager.LODDestroy ", lodComponent, " inLimbo=", flag), LOG_LEVEL);
		}
	}

	private void DestroyObjectsInLimbo()
	{
		if (limbo.Count == 0)
		{
			return;
		}
		int num = 0;
		for (int num2 = limbo.Count - 1; num2 >= 0; num2--)
		{
			if (limbo[num2] == null)
			{
				Debug.LogWarning("An object that was destroyed using LODManager.Manager().Destroy was also destroyed using unity Destroy. This object cannot be cleaned up by the LODManager.");
			}
			else if (!limbo[num2].isInCombined)
			{
				UnityEngine.Object.Destroy(limbo[num2].gameObject);
				limbo.RemoveAt(num2);
				num++;
			}
		}
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			MB2_Log.Log(MB2_LogLevel.debug, "MB2_LODManager DestroyObjectsInLimbo destroyed " + num, LOG_LEVEL);
		}
	}

	private void UpdateSkinnedMeshApproximateBoundsIfNecessary()
	{
		for (int i = 0; i < bakers.Length; i++)
		{
			if (bakers[i].updateSkinnedMeshApproximateBounds && bakers[i].meshBaker.meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				bakers[i].baker.UpdateSkinnedMeshApproximateBounds();
			}
		}
	}

	public int GetNextFrameCheckOffset()
	{
		return checkScheduler.GetNextFrameCheckOffset();
	}

	public float GetDistanceSqrToClosestPerspectiveCamera(Vector3 pos)
	{
		if (lodCameras.Length == 0)
		{
			return 0f;
		}
		float num = float.PositiveInfinity;
		for (int i = 0; i < lodCameras.Length; i++)
		{
			MB2_LODCamera mB2_LODCamera = lodCameras[i];
			if (mB2_LODCamera.enabled && MB2_Version.GetActive(mB2_LODCamera.gameObject) && !mB2_LODCamera.GetComponent<Camera>().orthographic)
			{
				Vector3 vector = mB2_LODCamera.transform.position - pos;
				float num2 = Vector3.Dot(vector, vector);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public void ForceBakeAllDirty()
	{
		foreach (LODCombinedMesh key in dirtyCombinedMeshes.Keys)
		{
			key.ForceBakeImmediately();
		}
	}

	public void TranslateWorld(Vector3 translation)
	{
		for (int i = 0; i < bakers.Length; i++)
		{
			if (bakers[i].clusterType == BakerPrototype.CombinerType.grid)
			{
				((LODClusterManagerGrid)bakers[i].baker).TranslateAllClusters(translation);
			}
		}
	}
}
