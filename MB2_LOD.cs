using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using DigitalOpus.MB.Lod;
using UnityEngine;

[AddComponentMenu("Mesh Baker/LOD")]
public class MB2_LOD : MonoBehaviour
{
	public enum SwitchDistanceSetup
	{
		notSetup,
		error,
		setup
	}

	[Serializable]
	public class LOD
	{
		public bool swapMeshWithLOD0;

		public bool bakeIntoCombined = true;

		public Animation anim;

		public Renderer lodObject;

		public int instanceID;

		public GameObject root;

		public float screenPercentage;

		public float dimension;

		public float sqrtDist;

		public float switchDistances;

		public int numVerts;
	}

	public class MB2_LODDistToCamComparer : IComparer<MB2_LOD>
	{
		int IComparer<MB2_LOD>.Compare(MB2_LOD aObj, MB2_LOD bObj)
		{
			return (int)(aObj._distSqrToClosestCamera - bObj._distSqrToClosestCamera);
		}
	}

	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	public LODLog myLog;

	public string bakerLabel = string.Empty;

	public MB_RenderType renderType;

	public int forceToLevel = -1;

	public bool bakeIntoCombined = true;

	public LOD[] levels;

	private Mesh lodZeroMesh;

	private float _distSqrToClosestCamera;

	private LODCombinedMesh combinedMesh;

	private MB2_LODManager.BakerPrototype baker;

	private MB2_LOD hierarchyRoot;

	private int currentLODidx;

	private int nextLODidx;

	private int orthographicIdx;

	private MB2_LODManager manager;

	private SwitchDistanceSetup setupStatus;

	private bool clustersAreSetup;

	private bool _wasLODDestroyed;

	private Vector3 _position;

	private bool _isInCombined;

	private bool _isInQueue;

	[NonSerialized]
	public MB2_LODOperation action = MB2_LODOperation.none;

	public float distanceSquaredToClosestCamera => _distSqrToClosestCamera;

	public int currentLevelIdx => currentLODidx;

	public int nextLevelIdx => nextLODidx;

	public bool isInCombined => _isInCombined;

	public bool isInQueue => _isInQueue;

	public void SetWasDestroyedFlag()
	{
		_wasLODDestroyed = true;
	}

	public void Start()
	{
		if (setupStatus == SwitchDistanceSetup.notSetup && Init())
		{
			MB2_LOD componentInAncestor = MB2_LODManager.GetComponentInAncestor<MB2_LOD>(base.transform, highest: true);
			manager.SetupHierarchy(componentInAncestor);
		}
		if (combinedMesh != null && MB2_LODManager.CHECK_INTEGRITY)
		{
			combinedMesh.GetLODCluster().CheckIntegrity();
		}
	}

	public string GetStatusMessage()
	{
		float num = Mathf.Sqrt(_distSqrToClosestCamera);
		string empty = string.Empty;
		string text = empty;
		empty = text + "isInCombined= " + isInCombined + "\n";
		text = empty;
		empty = text + "isInQueue= " + isInQueue + "\n";
		text = empty;
		empty = text + "currentLODidx= " + currentLODidx + "\n";
		if (nextLODidx != currentLODidx)
		{
			text = empty;
			empty = text + " switchingTo= " + nextLODidx + "\n";
		}
		text = empty;
		empty = text + "bestOrthographicIdx=" + orthographicIdx + "\n";
		text = empty;
		empty = text + "dist to camera= " + num + "\n";
		if (combinedMesh != null)
		{
			empty = empty + "meshbaker=" + combinedMesh.GetClusterManager().GetBakerPrototype().baker;
		}
		return empty;
	}

	public void _ResetPositionMarker()
	{
		_position = base.transform.position;
	}

	public LODCombinedMesh GetCombiner()
	{
		return combinedMesh;
	}

	public void SetCombiner(LODCombinedMesh c)
	{
		combinedMesh = c;
	}

	public Vector3 GetHierarchyPosition()
	{
		return hierarchyRoot._position;
	}

	public void AdjustNextLevelIndex(int newIdx)
	{
		if (newIdx < 0)
		{
			Debug.LogError("Bad argument " + newIdx);
			return;
		}
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat("AdjustNextLevelIndex ", this, " newIdx=", newIdx), LOG_LEVEL);
		}
		if (newIdx > levels.Length)
		{
			newIdx = levels.Length;
		}
		DoStateTransition(newIdx);
	}

	public int GetGameObjectID(int idx)
	{
		if (idx >= levels.Length)
		{
			Debug.LogError("Called GetGameObjectID when level was too high. " + idx);
			return -1;
		}
		if (levels[idx].swapMeshWithLOD0)
		{
			return levels[0].instanceID;
		}
		return levels[idx].instanceID;
	}

	public GameObject GetRendererGameObject(int idx)
	{
		if (idx >= levels.Length)
		{
			Debug.LogError("Called GetRendererGameObject when level was too high. " + idx);
			return null;
		}
		if (levels[idx].swapMeshWithLOD0)
		{
			return levels[0].lodObject.gameObject;
		}
		return levels[idx].lodObject.gameObject;
	}

	public int GetNumVerts(int idx)
	{
		if (idx >= levels.Length)
		{
			return 0;
		}
		return levels[idx].numVerts;
	}

	private bool Init()
	{
		if (setupStatus == SwitchDistanceSetup.setup)
		{
			return true;
		}
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			myLog = new LODLog(100);
		}
		else
		{
			myLog = new LODLog(0);
		}
		manager = MB2_LODManager.Manager();
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat(this, "Init called"), LOG_LEVEL);
		}
		if (!MB2_LODManager.ENABLED)
		{
			return false;
		}
		if (isInQueue || isInCombined)
		{
			Debug.LogError("Should not call Init on LOD that is in queue or in combined.");
			return false;
		}
		_isInCombined = false;
		setupStatus = SwitchDistanceSetup.notSetup;
		if (manager == null)
		{
			Debug.LogError("LOD coruld not find LODManager");
			return false;
		}
		MB2_LODCamera[] cameras = manager.GetCameras();
		if (cameras.Length == 0)
		{
			Debug.LogError("There is no camera with an MB2_LODCamera script on it.");
			return false;
		}
		float fieldOfView = 60f;
		bool flag = false;
		for (int i = 0; i < cameras.Length; i++)
		{
			Camera component = cameras[i].GetComponent<Camera>();
			if (component == null)
			{
				Debug.LogError("MB2_LODCamera script is not not attached to an object with a Camera component.");
				setupStatus = SwitchDistanceSetup.error;
				return false;
			}
			if (component == Camera.main && !component.orthographic)
			{
				fieldOfView = component.fieldOfView;
				flag = true;
			}
			if (!component.orthographic && !flag)
			{
				fieldOfView = component.fieldOfView;
			}
		}
		if (levels != null)
		{
			if (!bakeIntoCombined)
			{
				for (int j = 0; j < levels.Length; j++)
				{
					if (levels[j].bakeIntoCombined)
					{
						Debug.LogWarning("Setting bakeIntoCombined to false for level " + j + " because bakeIntoCombined was false for the LOD");
						levels[j].bakeIntoCombined = false;
					}
				}
			}
			for (int k = 0; k < levels.Length; k++)
			{
				LOD lOD = levels[k];
				if (lOD.lodObject == null)
				{
					Debug.LogError(string.Concat(this, " LOD Level ", k, " does not have a renderer."));
					return false;
				}
				if (lOD.lodObject is SkinnedMeshRenderer && renderType == MB_RenderType.meshRenderer)
				{
					Debug.LogError(string.Concat(this, " LOD Level ", k, " is a skinned mesh but Baker Render Type was MeshRenderer. Baker Render Type must be set to SkinnedMesh on this LOD component."));
					return false;
				}
				if (!lOD.lodObject.transform.IsChildOf(base.transform))
				{
					Debug.LogError(string.Concat(this, " LOD Level ", k, " is not a child of the LOD object."));
					return false;
				}
				if (lOD.lodObject.gameObject == base.gameObject)
				{
					Debug.LogError(string.Concat(this, " MB2_LOD component must be a parent ancestor of the level of detail renderers. It cannot be attached to the same game object as the level of detail renderers."));
				}
				if (k == 0 && lOD.swapMeshWithLOD0)
				{
					Debug.LogWarning(string.Concat(this, " The first level of an LOD cannot have swap Mesh With LOD set."));
					lOD.swapMeshWithLOD0 = false;
				}
				Animation component2 = lOD.lodObject.GetComponent<Animation>();
				Transform parent = lOD.lodObject.transform;
				while (parent.parent != base.transform && parent.parent != parent)
				{
					parent = parent.parent;
					if (component2 == null)
					{
						component2 = parent.GetComponent<Animation>();
					}
				}
				lOD.anim = component2;
				lOD.root = parent.gameObject;
				if (lOD.swapMeshWithLOD0)
				{
					MB2_Version.SetActiveRecursively(lOD.root, isActive: false);
				}
				if (lOD.bakeIntoCombined)
				{
					lOD.numVerts = MB_Utility.GetMesh(lOD.lodObject.gameObject).vertexCount;
				}
				lOD.instanceID = lOD.lodObject.gameObject.GetInstanceID();
				if (renderType == MB_RenderType.skinnedMeshRenderer && combinedMesh != null && combinedMesh.GetClusterManager().GetBakerPrototype().meshBaker != null && combinedMesh.GetClusterManager().GetBakerPrototype().meshBaker.meshCombiner.renderType == MB_RenderType.meshRenderer)
				{
					Debug.LogError(string.Concat(" LOD ", this, " RenderType is SkinnedMeshRenderer but baker ", k, " is a MeshRenderer. won't be able to add this to the combined mesh."));
					return false;
				}
			}
			lodZeroMesh = MB_Utility.GetMesh(levels[0].lodObject.gameObject);
			if (CalculateSwitchDistances(fieldOfView, showWarnings: true))
			{
				for (int l = 0; l < levels.Length; l++)
				{
					if (levels[l].anim != null)
					{
						levels[l].anim.Sample();
					}
					MySetActiveRecursively(l, a: false);
				}
				setupStatus = SwitchDistanceSetup.setup;
				currentLODidx = levels.Length;
				nextLODidx = levels.Length;
				_position = base.transform.position;
				return true;
			}
			setupStatus = SwitchDistanceSetup.error;
			return false;
		}
		Debug.LogError(string.Concat("LOD ", this, " had no levels."));
		setupStatus = SwitchDistanceSetup.error;
		return false;
	}

	private Vector3 AbsVector3(Vector3 v)
	{
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	public bool CalculateSwitchDistances(float fieldOfView, bool showWarnings)
	{
		bool flag = true;
		if (levels.Length == 0)
		{
			Debug.LogError(string.Concat(this, " does not have any LOD levels set up."));
			flag = false;
		}
		for (int i = 1; i < levels.Length; i++)
		{
			if (levels[i].screenPercentage >= levels[i - 1].screenPercentage)
			{
				if (showWarnings)
				{
					Debug.LogError(string.Concat("LOD object ", this, " screenPercentage must be in decending order"));
				}
				flag = false;
			}
		}
		float[] array = new float[levels.Length];
		for (int j = 0; j < levels.Length; j++)
		{
			Renderer lodObject = levels[j].lodObject;
			if (lodObject != null)
			{
				Bounds bounds = lodObject.bounds;
				if (lodObject is SkinnedMeshRenderer)
				{
					bool activeSelf = GetActiveSelf(lodObject.gameObject);
					bool flag2 = lodObject.enabled;
					SetActiveSelf(lodObject.gameObject, isActive: true);
					lodObject.enabled = true;
					Matrix4x4 localToWorldMatrix = lodObject.transform.localToWorldMatrix;
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)lodObject;
					bounds = new Bounds(localToWorldMatrix * skinnedMeshRenderer.localBounds.center, AbsVector3(localToWorldMatrix * skinnedMeshRenderer.localBounds.size));
					SetActiveSelf(lodObject.gameObject, activeSelf);
					lodObject.enabled = flag2;
				}
				levels[j].switchDistances = 0f;
				levels[j].sqrtDist = 0f;
				if (levels[j].screenPercentage <= 0f)
				{
					if (showWarnings)
					{
						Debug.LogError(string.Concat("LOD object ", this, " screenPercentage must be greater than zero."));
					}
					flag = false;
					continue;
				}
				float num = (bounds.size.x + bounds.size.y + bounds.size.z) / 3f;
				if (num == 0f)
				{
					if (showWarnings)
					{
						Debug.LogError(string.Concat("LOD ", this, " the object has no size"));
					}
					flag = false;
					continue;
				}
				array[j] = num;
				levels[j].dimension = num;
				for (int k = 0; k < j; k++)
				{
					if (array[k] > 1.5f * num || array[k] < num / 1.5f)
					{
						if (showWarnings)
						{
							Debug.LogError(string.Concat("LOD ", this, " the render bounds of lod levels ", j, " and ", k, " are very differnt sizes.They should be very close to the same size. LOD uses these to determine when to switch from one LOD to another."));
						}
						flag = false;
					}
				}
				float num2 = 50f / Mathf.Tan((float)Math.PI / 180f * fieldOfView / 2f);
				levels[j].switchDistances = num * num2 / (50f * levels[j].screenPercentage);
				levels[j].sqrtDist = levels[j].switchDistances;
				levels[j].switchDistances = levels[j].switchDistances * levels[j].switchDistances;
			}
			else
			{
				flag = false;
			}
		}
		if (LOG_LEVEL >= MB2_LogLevel.trace && myLog != null)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat(this, "CalculateSwitchDistances called fov=", fieldOfView, " success=", flag), LOG_LEVEL);
		}
		return flag;
	}

	public void Clear()
	{
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat(this, "Clear called"), LOG_LEVEL);
		}
		currentLODidx = levels.Length;
		nextLODidx = currentLODidx;
		setupStatus = SwitchDistanceSetup.notSetup;
		_isInQueue = false;
		_isInCombined = false;
		action = MB2_LODOperation.none;
		combinedMesh = null;
		clustersAreSetup = false;
	}

	public void CheckIfLODsNeedToChange()
	{
		if (!MB2_LODManager.ENABLED)
		{
			return;
		}
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
		if (setupStatus == SwitchDistanceSetup.error || setupStatus == SwitchDistanceSetup.notSetup)
		{
			return;
		}
		MB2_LODCamera[] cameras = manager.GetCameras();
		if (cameras.Length == 0)
		{
			return;
		}
		int num = levels.Length;
		if (forceToLevel != -1)
		{
			if (forceToLevel < 0 || forceToLevel > levels.Length)
			{
				Debug.LogWarning("Force To Level was not a valid level index value for LOD " + this);
			}
			else
			{
				num = forceToLevel;
				_distSqrToClosestCamera = levels[num].sqrtDist;
			}
		}
		else
		{
			_distSqrToClosestCamera = manager.GetDistanceSqrToClosestPerspectiveCamera(base.transform.position);
			for (int i = 0; i < levels.Length; i++)
			{
				if (_distSqrToClosestCamera < levels[i].switchDistances)
				{
					num = i;
					break;
				}
			}
			orthographicIdx = GetHighestLODIndexOfOrothographicCameras(cameras);
			if (orthographicIdx < num)
			{
				num = orthographicIdx;
			}
		}
		if (num != nextLODidx)
		{
			if (bakeIntoCombined && combinedMesh.GetClusterManager().GetBakerPrototype().clusterType == MB2_LODManager.BakerPrototype.CombinerType.grid && _position != base.transform.position)
			{
				Debug.LogError(string.Concat("Can't move LOD ", this, " after it has been added to a combined mesh unless baker type is 'Simple'"));
			}
			else
			{
				DoStateTransition(num);
			}
		}
	}

	private void DoStateTransition(int lodIdx)
	{
		if (lodIdx < 0 || lodIdx > levels.Length)
		{
			Debug.LogError("lodIdx out of range " + lodIdx);
		}
		if (_isInQueue)
		{
			if (action == MB2_LODOperation.toAdd)
			{
				SwapBetweenLevels(nextLODidx, currentLODidx);
			}
			else if (action == MB2_LODOperation.delete)
			{
				SwapBetweenLevels(nextLODidx, currentLODidx);
			}
			_CallLODCancelTransaction();
			action = MB2_LODOperation.none;
			nextLODidx = currentLevelIdx;
		}
		MB2_LODOperation mB2_LODOperation = MB2_LODOperation.none;
		mB2_LODOperation = (_isInCombined ? ((lodIdx != levels.Length && levels[lodIdx].bakeIntoCombined) ? MB2_LODOperation.update : MB2_LODOperation.delete) : ((lodIdx >= levels.Length || !levels[lodIdx].bakeIntoCombined) ? MB2_LODOperation.none : MB2_LODOperation.toAdd));
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat(this, " DoStateTransition newA=", mB2_LODOperation, " newNextLevel=", lodIdx), LOG_LEVEL);
		}
		if (mB2_LODOperation == MB2_LODOperation.toAdd && currentLevelIdx == levels.Length)
		{
			SwapBetweenLevels(nextLODidx, lodIdx);
		}
		else if (mB2_LODOperation == MB2_LODOperation.delete && lodIdx == levels.Length)
		{
			SwapBetweenLevels(nextLODidx, lodIdx);
		}
		if (!bakeIntoCombined && lodIdx != currentLevelIdx)
		{
			SwapBetweenLevels(nextLODidx, lodIdx);
		}
		nextLODidx = lodIdx;
		if (bakeIntoCombined)
		{
			action = mB2_LODOperation;
			_CallLODChanged();
			return;
		}
		action = MB2_LODOperation.none;
		currentLODidx = nextLODidx;
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			myLog.Log(MB2_LogLevel.debug, string.Concat(this, " LODChanged but not baking next level=", nextLODidx), LOG_LEVEL);
		}
	}

	public void ForceRemove()
	{
		action = MB2_LODOperation.none;
		_isInQueue = false;
		_isInCombined = false;
		if (currentLODidx != nextLODidx && currentLODidx < levels.Length)
		{
			MySetActiveRecursively(currentLODidx, a: false);
		}
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, "ForceRemove called " + this, LOG_LEVEL);
		}
		if (nextLODidx < levels.Length)
		{
			MySetActiveRecursively(nextLODidx, a: true);
		}
		currentLODidx = nextLODidx;
	}

	public void ForceAdd()
	{
		if (!isInCombined && !isInQueue && nextLODidx < levels.Length && levels[nextLODidx].bakeIntoCombined)
		{
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				myLog.Log(MB2_LogLevel.trace, "ForceAdd called " + this, LOG_LEVEL);
			}
			if (nextLODidx < levels.Length)
			{
				action = MB2_LODOperation.toAdd;
			}
			combinedMesh.LODChanged(this, immediate: false);
		}
	}

	private void _CallLODCancelTransaction()
	{
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			myLog.Log(MB2_LogLevel.debug, string.Concat(this, " calling _CallLODCancelTransaction action=", action, " next level=", nextLODidx), LOG_LEVEL);
		}
		if (currentLevelIdx < levels.Length && currentLevelIdx > 0 && levels[currentLevelIdx].swapMeshWithLOD0)
		{
			Mesh mesh = MB_Utility.GetMesh(levels[currentLevelIdx].lodObject.gameObject);
			SetMesh(levels[0].lodObject.gameObject, mesh);
		}
		else if (currentLevelIdx == 0)
		{
			SetMesh(levels[0].lodObject.gameObject, lodZeroMesh);
		}
		combinedMesh.LODCancelTransaction(this);
	}

	private void _CallLODChanged()
	{
		if (LOG_LEVEL >= MB2_LogLevel.debug)
		{
			myLog.Log(MB2_LogLevel.debug, string.Concat(this, " calling LODChanged action=", action, " next level=", nextLODidx), LOG_LEVEL);
		}
		if (action != MB2_LODOperation.none)
		{
			if (nextLODidx < levels.Length && nextLODidx > 0 && levels[nextLODidx].swapMeshWithLOD0)
			{
				Mesh mesh = MB_Utility.GetMesh(levels[nextLODidx].lodObject.gameObject);
				SetMesh(levels[0].lodObject.gameObject, mesh);
			}
			else if (nextLODidx == 0)
			{
				SetMesh(levels[0].lodObject.gameObject, lodZeroMesh);
			}
			combinedMesh.LODChanged(this, immediate: false);
		}
		else if (nextLODidx == levels.Length || (nextLODidx < levels.Length && !levels[nextLODidx].bakeIntoCombined))
		{
			OnBakeAdded();
		}
	}

	public void OnBakeRemoved()
	{
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, "OnBakeRemoved " + this, LOG_LEVEL);
		}
		if (!_isInQueue || action != MB2_LODOperation.delete || !isInCombined)
		{
			Debug.LogError("OnBakeRemoved called on an LOD in an invalid state: " + ToString());
		}
		_isInCombined = false;
		action = MB2_LODOperation.none;
		_isInQueue = false;
		if (currentLODidx < levels.Length)
		{
			MySetActiveRecursively(currentLODidx, a: false);
		}
		if (nextLODidx < levels.Length)
		{
			MySetActiveRecursively(nextLODidx, a: true);
		}
		currentLODidx = nextLODidx;
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	public void OnBakeAdded()
	{
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, "OnBakeAdded " + this, LOG_LEVEL);
		}
		if (nextLODidx < levels.Length && levels[nextLODidx].bakeIntoCombined)
		{
			if (!_isInQueue || action != 0 || isInCombined)
			{
				Debug.LogError("OnBakeAdded called on an LOD in an invalid state: " + ToString() + " log " + myLog.Dump());
			}
			_isInCombined = true;
		}
		else
		{
			if (_isInQueue || action != MB2_LODOperation.none || isInCombined)
			{
				Debug.LogError("OnBakeAdded called on an LOD in an invalid state: " + ToString() + " log " + myLog.Dump());
			}
			_isInCombined = false;
		}
		_isInQueue = false;
		action = MB2_LODOperation.none;
		SwapBetweenLevels(currentLODidx, nextLODidx);
		currentLODidx = nextLODidx;
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	public void OnBakeUpdated()
	{
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, "OnBakeUpdated " + this, LOG_LEVEL);
		}
		if (!_isInQueue || action != MB2_LODOperation.update || !isInCombined)
		{
			Debug.LogError("OnBakeUpdated called on an LOD in an invalid state: " + ToString());
		}
		if (nextLODidx >= levels.Length)
		{
			Debug.LogError("Update will remove all meshes from combined. This should never happen.");
		}
		_isInQueue = false;
		_isInCombined = true;
		action = MB2_LODOperation.none;
		SwapBetweenLevels(currentLODidx, nextLODidx);
		currentLODidx = nextLODidx;
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	public void OnRemoveFromQueue()
	{
		_isInQueue = false;
		action = MB2_LODOperation.none;
		nextLODidx = currentLODidx;
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, "OnRemoveFromQueue complete " + this, LOG_LEVEL);
		}
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	public void OnAddToQueue()
	{
		_isInQueue = true;
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, "OnAddToAddQueue complete " + this, LOG_LEVEL);
		}
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	private void OnDestroy()
	{
		if (setupStatus == SwitchDistanceSetup.setup)
		{
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				myLog.Log(MB2_LogLevel.trace, string.Concat(this, "OnDestroy called"), LOG_LEVEL);
			}
			if (!_wasLODDestroyed)
			{
				myLog.Log(MB2_LogLevel.debug, string.Concat("An MB2_LOD object ", this, " was destroyed using Unity's Destroy method. This can leave destroyed meshes in the combined mesh. Try using MB2_LODManager.Manager().LODDestroy() instead."), LOG_LEVEL);
			}
			_removeIfInCombined();
			if (combinedMesh != null)
			{
				combinedMesh.UnassignFromCombiner(this);
			}
			combinedMesh = null;
			if (MB2_LODManager.CHECK_INTEGRITY)
			{
				CheckIntegrity();
			}
		}
	}

	private void OnEnable()
	{
		if (setupStatus != SwitchDistanceSetup.setup)
		{
			return;
		}
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat(this, "OnEnable called"), LOG_LEVEL);
		}
		if (levels != null)
		{
			for (int i = 0; i < levels.Length; i++)
			{
				if (!_isInCombined && currentLODidx == i)
				{
					MySetActiveRecursively(i, a: true);
				}
				else
				{
					MySetActiveRecursively(i, a: false);
				}
			}
		}
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	private void OnDisable()
	{
		if (myLog != null && LOG_LEVEL >= MB2_LogLevel.trace)
		{
			myLog.Log(MB2_LogLevel.trace, string.Concat(this, "OnDisable called"), LOG_LEVEL);
		}
		_removeIfInCombined();
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	private void _removeIfInCombined()
	{
		if (!_isInCombined && !_isInQueue)
		{
			return;
		}
		for (int i = 0; i < levels.Length; i++)
		{
			if (levels[i].lodObject != null && !levels[i].swapMeshWithLOD0)
			{
				MySetActiveRecursively(i, a: false);
			}
		}
		nextLODidx = levels.Length;
		if ((isInCombined || isInQueue) && MB2_LODManager.Manager() != null)
		{
			action = MB2_LODOperation.delete;
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				myLog.Log(MB2_LogLevel.trace, string.Concat(this, " Calling  LODManager.RemoveLOD"), LOG_LEVEL);
			}
			combinedMesh.RemoveLOD(this);
		}
	}

	public bool ArePrototypesSetup()
	{
		return clustersAreSetup;
	}

	public MB2_LODManager.BakerPrototype GetBaker(MB2_LODManager.BakerPrototype[] allPrototypes, bool ignoreLightmapping)
	{
		if (baker != null)
		{
			return baker;
		}
		if (setupStatus != SwitchDistanceSetup.setup && !Init())
		{
			return null;
		}
		myLog.Log(MB2_LogLevel.debug, string.Concat(this, " GetBaker called setting up baker"), LOG_LEVEL);
		MB2_LODManager.BakerPrototype bakerPrototype = null;
		for (int i = 0; i < levels.Length; i++)
		{
			if (!levels[i].bakeIntoCombined)
			{
				continue;
			}
			Renderer lodObject = levels[i].lodObject;
			Mesh mesh = MB_Utility.GetMesh(levels[i].lodObject.gameObject);
			Rect uvBounds = new Rect(0f, 0f, 1f, 1f);
			bool flag = MB_Utility.hasOutOfBoundsUVs(mesh, ref uvBounds);
			int lightmapIndex = lodObject.lightmapIndex;
			HashSet<Material> hashSet = new HashSet<Material>();
			for (int j = 0; j < lodObject.sharedMaterials.Length; j++)
			{
				if (lodObject.sharedMaterials[j] != null && lodObject.sharedMaterials[j].shader != null)
				{
					hashSet.Add(lodObject.sharedMaterials[j]);
				}
			}
			if (bakerLabel != null && bakerLabel.Length > 0)
			{
				for (int k = 0; k < allPrototypes.Length; k++)
				{
					if (allPrototypes[k].label.Equals(bakerLabel))
					{
						if (!ignoreLightmapping && lightmapIndex != allPrototypes[k].lightMapIndex)
						{
							Debug.LogError(string.Concat("LOD ", this, " had a bakerLabel, but had a different lightmap index than that baker"));
						}
						if (!hashSet.IsSubsetOf(allPrototypes[k].materials))
						{
							Debug.LogError(string.Concat("LOD ", this, " had a bakerLabel, but had materials are not in that baker"));
						}
						bakerPrototype = allPrototypes[k];
						break;
					}
				}
				if (bakerPrototype != null)
				{
					continue;
				}
				Debug.LogError(string.Concat("LOD ", this, " had a bakerLabel '", bakerLabel, "' that was not matched by any baker"));
			}
			MB2_LODManager.BakerPrototype bakerPrototype2 = null;
			MB2_LODManager.BakerPrototype bakerPrototype3 = null;
			string text = string.Empty;
			string empty = string.Empty;
			for (int l = 0; l < allPrototypes.Length; l++)
			{
				empty = string.Empty;
				MB2_LODManager.BakerPrototype bakerPrototype4 = null;
				MB2_LODManager.BakerPrototype bakerPrototype5 = null;
				if (flag && allPrototypes[l].materials.SetEquals(hashSet))
				{
					bakerPrototype5 = allPrototypes[l];
				}
				if (hashSet.IsSubsetOf(allPrototypes[l].materials))
				{
					bakerPrototype4 = allPrototypes[l];
				}
				if (!ignoreLightmapping && lightmapIndex != allPrototypes[l].lightMapIndex && bakerPrototype4 != null)
				{
					empty += "\n  lightmapping check failed";
				}
				if (allPrototypes[l].meshBaker.meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer && renderType != MB_RenderType.skinnedMeshRenderer && bakerPrototype4 != null)
				{
					empty += "\n  rendertype did not match";
				}
				if (allPrototypes[l].meshBaker.meshCombiner.renderType == MB_RenderType.meshRenderer && renderType != 0 && bakerPrototype4 != null)
				{
					empty += "\n  rendertype did not match";
				}
				if (empty.Length == 0)
				{
					if (bakerPrototype5 != null)
					{
						if (bakerPrototype3 != null)
						{
							Debug.LogWarning(string.Concat("The set of materials on LOD ", this, " matched multiple bakers. Try use labels to resolve the conflict."));
						}
						bakerPrototype3 = bakerPrototype5;
					}
					if (bakerPrototype4 != null)
					{
						if (bakerPrototype2 != null)
						{
							Debug.LogWarning(string.Concat("The set of materials on LOD ", this, " matched multiple bakers. Try use labels to resolve the conflict."));
						}
						bakerPrototype2 = bakerPrototype4;
					}
				}
				else
				{
					string text2 = text;
					text = text2 + "LOD " + i + " Baker " + l + " matched the materials but could not match because: " + empty;
				}
			}
			if (bakerPrototype3 != null)
			{
				bakerPrototype = bakerPrototype3;
				continue;
			}
			if (bakerPrototype2 == null)
			{
				string text3 = string.Empty;
				foreach (Material item in hashSet)
				{
					text3 = string.Concat(text3, item, ",");
				}
				Debug.LogError(string.Concat("Could not find a baker that can accept the materials on LOD ", this, "\nmaterials [", text3, "]\nlightmapIndex = ", lightmapIndex, " (ignore lightmapping = ", ignoreLightmapping, ")\nout of bounds uvs ", flag, " (if true then set of prototype materials must match exactly.)\n", text));
				return null;
			}
			bakerPrototype = bakerPrototype2;
		}
		baker = bakerPrototype;
		return baker;
	}

	public void SetupHierarchy(MB2_LODManager.BakerPrototype[] allPrototypes, bool ignoreLightmapping)
	{
		if (LOG_LEVEL >= MB2_LogLevel.trace)
		{
			MB2_Log.LogDebug("Setting up hierarchy for " + this);
		}
		MB2_LOD componentInAncestor = MB2_LODManager.GetComponentInAncestor<MB2_LOD>(base.transform);
		if (componentInAncestor != this)
		{
			Debug.LogError("Should only be called on the root LOD.");
		}
		hierarchyRoot = this;
		if (combinedMesh == null)
		{
			GetBaker(allPrototypes, ignoreLightmapping);
			if (baker != null)
			{
				LODCluster clusterFor = baker.baker.GetClusterFor(GetHierarchyPosition());
				combinedMesh = clusterFor.SuggestCombiner();
				clusterFor.AssignLODToCombiner(this);
			}
			else if (!bakeIntoCombined && !GetComponent<MB2_LODSoloChecker>())
			{
				base.gameObject.AddComponent<MB2_LODSoloChecker>();
			}
			_RecurseSetup(base.transform, allPrototypes, ignoreLightmapping);
		}
	}

	private static void _RecurseSetup(Transform t, MB2_LODManager.BakerPrototype[] allPrototypes, bool ignoreLightmapping)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			MB2_LOD component = child.GetComponent<MB2_LOD>();
			if (component != null)
			{
				if (component.Init())
				{
					component.GetBaker(allPrototypes, ignoreLightmapping);
				}
				if (component.baker != null)
				{
					component.FindHierarchyRoot();
					LODCluster clusterFor = component.baker.baker.GetClusterFor(component.GetHierarchyPosition());
					component.combinedMesh = clusterFor.SuggestCombiner();
					clusterFor.AssignLODToCombiner(component);
					component.clustersAreSetup = true;
				}
			}
			_RecurseSetup(child, allPrototypes, ignoreLightmapping);
		}
	}

	public MB2_LOD GetHierarchyRoot()
	{
		return hierarchyRoot;
	}

	private MB2_LOD FindHierarchyRoot()
	{
		Transform parent = base.transform.parent;
		MB2_LOD result = this;
		while (parent != null)
		{
			MB2_LOD component = parent.GetComponent<MB2_LOD>();
			if (component != null)
			{
				MB2_LODManager.BakerPrototype bakerPrototype = component.baker;
				if (bakerPrototype != null && bakerPrototype == baker)
				{
					result = component;
				}
			}
			if (parent == parent.root)
			{
				break;
			}
			parent = parent.parent;
		}
		hierarchyRoot = result;
		return result;
	}

	public override string ToString()
	{
		string text = string.Empty;
		if (nextLODidx < levels.Length)
		{
			text += levels[nextLODidx].instanceID;
		}
		return $"[MB2_LOD {base.name} id={GetInstanceID()}: inComb={isInCombined} inQ={isInQueue} act={action} nxt={nextLODidx} curr={currentLODidx} nxtRendInstId={text}]";
	}

	public void CheckState(bool exInCombined, bool exInQueue, MB2_LODOperation exAction, int exNextIdx, int exCurrentIdx)
	{
		if (isInCombined != exInCombined)
		{
			Debug.LogError("inCombined Test fail. was " + isInCombined + " expects=" + exInCombined);
		}
		if (isInQueue != exInQueue)
		{
			Debug.LogError(GetInstanceID() + " inQueue Test fail. was " + isInQueue + " expects=" + exInQueue);
		}
		if (action != exAction)
		{
			Debug.LogError(string.Concat("action Test fail. was ", action, " expects=", exAction));
		}
		if (nextLODidx != exNextIdx)
		{
			Debug.LogError("next idx Test fail. was " + nextLODidx + " expects=" + exNextIdx);
		}
		if (currentLODidx != exCurrentIdx)
		{
			Debug.LogError("current idx Test fail. was " + currentLODidx + " expects=" + exCurrentIdx);
		}
		if (MB2_LODManager.CHECK_INTEGRITY)
		{
			CheckIntegrity();
		}
	}

	private void CheckIntegrity()
	{
		if (this == null)
		{
			return;
		}
		if (_isInCombined && currentLODidx >= levels.Length)
		{
			Debug.LogError(string.Concat(this, " IntegrityCheckFailed invalid currentLODidx", this));
		}
		if (action != MB2_LODOperation.none && !isInQueue)
		{
			Debug.LogError(string.Concat(this, " Invalid action if not in queue ", this));
		}
		if (action == MB2_LODOperation.none && isInQueue)
		{
			Debug.LogError(string.Concat(this, " Invalid action if in queue ", this));
		}
		if (action == MB2_LODOperation.toAdd && isInCombined)
		{
			Debug.LogError(string.Concat(this, " Invalid action if in combined ", this));
		}
		if (action == MB2_LODOperation.delete && !isInCombined)
		{
			Debug.LogError(string.Concat(this, " Invalid action if not in combined ", this));
		}
		if (action == MB2_LODOperation.delete && currentLODidx >= levels.Length)
		{
			Debug.LogError(string.Concat(this, " Invalid currentLODidx ", currentLODidx));
		}
		if (setupStatus == SwitchDistanceSetup.setup)
		{
			for (int i = 0; i < levels.Length; i++)
			{
				if (levels[i].lodObject != null && GetActiveSelf(levels[i].lodObject.gameObject) && (i != 0 || currentLODidx >= levels.Length || !levels[currentLODidx].swapMeshWithLOD0) && !levels[i].swapMeshWithLOD0)
				{
					if (!isInQueue && i != currentLODidx)
					{
						Debug.LogError(string.Concat("f=", Time.frameCount, " ", this, " lodObject of wrong level was active was:", i, " should be:", currentLODidx));
						Debug.Log("LogDump " + myLog.Dump());
					}
					Renderer component = levels[i].lodObject.GetComponent<Renderer>();
					if (_isInCombined && component.enabled)
					{
						Debug.LogError(string.Concat("f=", Time.frameCount, " ", this, " lodObject object in combined and its renderer was enabled id ", levels[i].instanceID, " when inCombined. should all be inactive ", currentLODidx));
						Debug.Log("LogDump " + myLog.Dump());
					}
				}
			}
		}
		if (combinedMesh != null)
		{
			if (!combinedMesh.IsAssignedToThis(this))
			{
				Debug.LogError("LOD was assigned to combinedMesh but combinedMesh didn't contain " + this);
			}
			LODCluster lODCluster = combinedMesh.GetLODCluster();
			if (lODCluster != null && !lODCluster.GetCombiners().Contains(combinedMesh))
			{
				Debug.LogError("Cluster was assigned to cell but it wasn't in its list of clusters");
			}
		}
		if (!GetActiveSelf(base.gameObject) || _isInCombined)
		{
			return;
		}
		bool flag = false;
		for (int j = 0; j < levels.Length; j++)
		{
			if (GetActiveSelf(levels[j].lodObject.gameObject) && levels[j].lodObject.enabled)
			{
				flag = true;
			}
		}
		if (!flag && nextLODidx < levels.Length)
		{
			Debug.LogError("All levels were invisible " + this);
		}
	}

	private int GetHighestLODIndexOfOrothographicCameras(MB2_LODCamera[] cameras)
	{
		if (cameras.Length == 0)
		{
			return 0;
		}
		int num = levels.Length;
		foreach (MB2_LODCamera mB2_LODCamera in cameras)
		{
			if (!mB2_LODCamera.enabled || !GetActiveSelf(mB2_LODCamera.gameObject) || !mB2_LODCamera.GetComponent<Camera>().orthographic)
			{
				continue;
			}
			float num2 = mB2_LODCamera.GetComponent<Camera>().orthographicSize * 2f;
			float num3 = levels[0].dimension / num2;
			for (int j = 0; j < num; j++)
			{
				if (num3 > levels[j].screenPercentage)
				{
					if (j < num)
					{
						num = j;
					}
					break;
				}
			}
		}
		return num;
	}

	private void SwapBetweenLevels(int oldIdx, int newIdx)
	{
		if (oldIdx < levels.Length && newIdx < levels.Length && (oldIdx == 0 || levels[oldIdx].swapMeshWithLOD0) && (newIdx == 0 || levels[newIdx].swapMeshWithLOD0))
		{
			base.gameObject.SendMessage("LOD_OnSetLODActive", levels[0].root, SendMessageOptions.DontRequireReceiver);
			return;
		}
		if (oldIdx < levels.Length)
		{
			MySetActiveRecursively(oldIdx, a: false);
		}
		if (newIdx < levels.Length)
		{
			MySetActiveRecursively(newIdx, a: true);
		}
	}

	private void MySetActiveRecursively(int idx, bool a)
	{
		if (idx >= levels.Length)
		{
			return;
		}
		if (idx > 0 && levels[idx].swapMeshWithLOD0)
		{
			if (a)
			{
				Mesh mesh = MB_Utility.GetMesh(levels[idx].lodObject.gameObject);
				SetMesh(levels[0].lodObject.gameObject, mesh);
				if (levels[0].anim != null)
				{
					levels[0].anim.Sample();
				}
			}
			else
			{
				SetMesh(levels[0].lodObject.gameObject, lodZeroMesh);
				if (levels[0].anim != null)
				{
					levels[0].anim.Sample();
				}
			}
			if (GetActiveSelf(levels[0].root) != a)
			{
				MB2_Version.SetActiveRecursively(levels[0].root, a);
				base.gameObject.SendMessage("LOD_OnSetLODActive", levels[0].root, SendMessageOptions.DontRequireReceiver);
			}
			if (a && !_isInCombined)
			{
				levels[0].lodObject.enabled = true;
			}
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				myLog.Log(MB2_LogLevel.trace, "SettingActive (swaps mesh on level zero) to " + a + " for level " + idx + " on " + this, LOG_LEVEL);
			}
		}
		else
		{
			if (GetActiveSelf(levels[idx].root) != a)
			{
				MB2_Version.SetActiveRecursively(levels[idx].root, a);
				base.gameObject.SendMessage("LOD_OnSetLODActive", levels[idx].root, SendMessageOptions.DontRequireReceiver);
			}
			if (a && !_isInCombined)
			{
				levels[idx].lodObject.enabled = true;
			}
			if (LOG_LEVEL >= MB2_LogLevel.trace)
			{
				myLog.Log(MB2_LogLevel.trace, "SettingActive to " + a + " for level " + idx + " on " + this, LOG_LEVEL);
			}
		}
	}

	private void SetMesh(GameObject go, Mesh m)
	{
		if (go == null)
		{
			return;
		}
		MeshFilter component = go.GetComponent<MeshFilter>();
		if (component != null)
		{
			component.sharedMesh = m;
			return;
		}
		SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
		if (component2 != null)
		{
			component2.sharedMesh = m;
		}
		else
		{
			Debug.LogError("Object " + go.name + " does not have a MeshFilter or a SkinnedMeshRenderer component");
		}
	}

	public static bool GetActiveSelf(GameObject go)
	{
		return go.activeSelf;
	}

	public static void SetActiveSelf(GameObject go, bool isActive)
	{
		go.SetActive(isActive);
	}
}
