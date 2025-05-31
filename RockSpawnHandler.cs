using System.Collections.Generic;
using System.Linq;
using HumanAPI;
using Multiplayer;
using UnityEngine;

public class RockSpawnHandler : Node, IReset
{
	private enum HitType
	{
		TooWeak,
		Cut
	}

	[Tooltip("The collider the tree will look for when being chopped down")]
	public List<Collider> hittyTools;

	[Tooltip("The offset the rock is spawned from parent rock")]
	public Vector3 spawnOffsetPosition;

	[Tooltip("The direction the rock is pushed in when spawned")]
	public Vector3 pushDirection;

	[Tooltip("The amount of cuts needed to make the tree fall")]
	public int cutsRequired = 3;

	[Tooltip("The amount of force needed to spawn new rock")]
	public float cutForce = 2f;

	[Tooltip("The FX drawn when there is a cut")]
	public GameObject cutFxTemplate;

	[Tooltip("Length of the list of the effects use")]
	public int poolFxItemCount = 5;

	[Tooltip("Prefab used to spawn rock")]
	public GameObject rockPrefab;

	[Tooltip("Delay in seconds after last spawned rock")]
	public float delayBetweenRockSpawn = 0.5f;

	[Tooltip("Rockspawn parent, so we can add spawned rocks under this")]
	public GameObject rockSpawnParent;

	public Transform rockSpawnPoint;

	private List<GameObject> poolObjects = new List<GameObject>();

	private int cutCount;

	private float lastRockSpawn;

	public GameObject newRock;

	public NodeOutput output;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private uint evtHit;

	private NetIdentity identity;

	private NetVector3Encoder posEncoder = new NetVector3Encoder(500f, 18, 4, 8);

	public Transform rockPoolRoot;

	public List<GameObject> rockPool = new List<GameObject>();

	public int maxRocks;

	public int rockIndex;

	private void Start()
	{
		OnEnable();
		if (showDebug)
		{
			Debug.Log(base.name + " Started ");
		}
		identity = GetComponent<NetIdentity>();
		evtHit = identity.RegisterEvent(OnHit);
		lastRockSpawn = Time.time;
		rockPool = (from rock in rockPoolRoot.GetComponentsInChildren<Rock>(includeInactive: true).Select(delegate(Rock rock)
			{
				rock.SetSpawner(this);
				return rock;
			})
			select rock.gameObject).ToList();
		maxRocks = rockPool.Count;
	}

	private new void OnEnable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Enabled ");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Collision Enter ");
		}
		if (!isValidTool(collision) || !(Time.time > lastRockSpawn + delayBetweenRockSpawn))
		{
			return;
		}
		if (showDebug)
		{
			Debug.Log(base.name + " Hit by valid tool ");
		}
		if (GetCurrentRockCount() <= 0)
		{
			return;
		}
		if (collision.relativeVelocity.magnitude > cutForce)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Hit Hard enough ");
			}
			cutCount++;
			if (cutCount >= cutsRequired)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " No More hits needed ");
				}
				InstantiateRock();
				newRock.transform.parent = rockSpawnParent.transform;
				newRock.transform.localPosition = new Vector3(0f, 0f, 0f);
				if (showDebug)
				{
					Debug.Log(base.name + " Turning off kinematic flag ");
				}
				lastRockSpawn = Time.time;
				cutCount = 0;
				output.SetValue(rockIndex);
			}
		}
		else
		{
			if (showDebug)
			{
				Debug.Log(base.name + " More hits needed ");
			}
			SendHit(HitType.TooWeak, collision.GetPoint());
		}
	}

	private int GetCurrentRockCount()
	{
		return rockPoolRoot.childCount;
	}

	private bool isValidTool(Collision collision)
	{
		foreach (Collider hittyTool in hittyTools)
		{
			if (collision.collider == hittyTool)
			{
				return true;
			}
		}
		return false;
	}

	private void OnHit(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " OnHit ");
		}
		HitType type = (HitType)stream.ReadUInt32(1);
		Vector3 position = posEncoder.ApplyState(stream);
		DoHit(type, position);
	}

	private void SendHit(HitType type, Vector3 position)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " SendHit ");
		}
		DoHit(type, position);
		if (NetGame.isServer || ReplayRecorder.isRecording)
		{
			NetStream netStream = identity.BeginEvent(evtHit);
			netStream.Write((uint)type, 1);
			posEncoder.CollectState(netStream, position);
			identity.EndEvent();
		}
	}

	private void DoHit(HitType type, Vector3 position)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " DoHit ");
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " ResetState ");
		}
		foreach (Transform item in rockSpawnParent.transform)
		{
			ReturnToPool(item.gameObject);
		}
		output.SetValue(0f);
		cutCount = 0;
	}

	public void OnRespawn()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " OnRespawn ");
		}
		ResetState(0, 0);
	}

	public void InstantiateRock()
	{
		if (rockPool.Count != 0)
		{
			newRock = rockPool[0];
			rockPool.RemoveAt(0);
			newRock.GetComponent<MeshRenderer>().enabled = true;
			newRock.GetComponent<MeshCollider>().enabled = true;
			int.TryParse(newRock.name.Substring(5, 1), out rockIndex);
		}
	}

	public void ReturnToPool(GameObject rock)
	{
		rock.transform.parent = rockPoolRoot.transform;
		rock.GetComponent<MeshRenderer>().enabled = false;
		rock.GetComponent<MeshCollider>().enabled = false;
		rockPool.Add(rock);
	}
}
