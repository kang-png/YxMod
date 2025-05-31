using HumanAPI;
using Multiplayer;
using UnityEngine;

public class TriggerVolume : Node, INetBehavior, IReset
{
	public class TriggerVolumeChild : MonoBehaviour
	{
		public delegate void TriggerEvent(Collider other);

		private TriggerVolume parentVolume;

		public TriggerVolume ParentVolume
		{
			get
			{
				return parentVolume;
			}
			set
			{
				parentVolume = parentVolume ?? value;
			}
		}

		public event TriggerEvent TriggerEnter;

		public event TriggerEvent TriggerExit;

		private void OnTriggerEnter(Collider other)
		{
			if (this.TriggerEnter != null)
			{
				this.TriggerEnter(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.TriggerExit != null)
			{
				this.TriggerExit(other);
			}
		}
	}

	public NodeOutput output;

	[Tooltip("Collider that will trigger this volume. If blank, any player will trigger.")]
	public Collider colliderToCheckFor;

	[Tooltip("Any gameobject that's a child of an entry in this list will also trigger output.")]
	public GameObject[] additionalColliders;

	[Tooltip("Value to output to the graph when players are inside the volume")]
	public float outputValueInside = 1f;

	[Tooltip("Value to output when the player is outside the volume")]
	public float outputValueOutside;

	[Tooltip("Trigger Activation and De-Activation only if its player, not for any clients")]
	public bool isPlayerCheckOnActivateAndDeactivate;

	[Tooltip("Array of gameobject to activate on entry")]
	public GameObject[] activateOnEnter;

	[Tooltip("Array of gameobjects to deactivate on exit")]
	public GameObject[] deactivateOnExit;

	public bool runExitLogicAtStartup;

	[Tooltip("If this is true, colliders are tracked as they enter/leave the volume and volume will deactivate only if no colliders are inside. Otherwise it will deactivate when any collider leaves even if others remain in the volume.")]
	public bool trackColliders = true;

	[Tooltip("If true, collisions with any child object will trigger as if hitting this one. Must be set before play")]
	public bool useChildTriggers;

	private int colliderCount;

	private bool isActive;

	private void Start()
	{
		if (useChildTriggers)
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>(includeInactive: true);
			foreach (Transform transform in componentsInChildren)
			{
				TriggerVolumeChild triggerVolumeChild = transform.gameObject.AddComponent<TriggerVolumeChild>();
				triggerVolumeChild.ParentVolume = this;
				triggerVolumeChild.TriggerEnter += OnTriggerEnter;
				triggerVolumeChild.TriggerExit += OnTriggerExit;
			}
		}
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
		if (runExitLogicAtStartup)
		{
			SetExitObjectState(isPlayer: false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		bool isPlayer = false;
		if (!IsCorrectCollider(other, out isPlayer))
		{
			return;
		}
		if (trackColliders)
		{
			colliderCount++;
			if (colliderCount > 1)
			{
				return;
			}
		}
		SetEnterObjectState(isPlayer);
	}

	private void OnTriggerExit(Collider other)
	{
		bool isPlayer = false;
		if (!IsCorrectCollider(other, out isPlayer))
		{
			return;
		}
		if (trackColliders)
		{
			colliderCount--;
			if (colliderCount < 0)
			{
				colliderCount = 0;
			}
			if (colliderCount != 0)
			{
				return;
			}
		}
		SetExitObjectState(isPlayer);
	}

	private bool IsCorrectCollider(Collider other, out bool isPlayer)
	{
		bool flag = false;
		isPlayer = false;
		if (colliderToCheckFor == null && additionalColliders.Length == 0)
		{
			if (Human.Localplayer.GetComponent<Collider>() == other)
			{
				flag = true;
				isPlayer = true;
			}
			foreach (Human item in Human.all)
			{
				if (item.GetComponent<Collider>() == other)
				{
					flag = true;
					break;
				}
			}
		}
		else
		{
			flag = other == colliderToCheckFor && colliderToCheckFor != null;
			if (!flag)
			{
				Transform transform = other.gameObject.transform;
				if (additionalColliders.Length != 0)
				{
					GameObject[] array = additionalColliders;
					foreach (GameObject gameObject in array)
					{
						if (transform.IsChildOf(gameObject.transform))
						{
							flag = true;
							break;
						}
					}
				}
			}
		}
		return flag;
	}

	private void SetEnterObjectState(bool isPlayer)
	{
		isActive = true;
		if (output != null)
		{
			output.SetValue(outputValueInside);
		}
		if (isPlayerCheckOnActivateAndDeactivate && !isPlayer)
		{
			return;
		}
		GameObject[] array = activateOnEnter;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				Debug.LogError("Trigger Volume error @ Object " + base.gameObject.name);
			}
			else
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	private void SetExitObjectState(bool isPlayer)
	{
		isActive = false;
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
		if (isPlayerCheckOnActivateAndDeactivate && !isPlayer)
		{
			return;
		}
		GameObject[] array = deactivateOnExit;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				Debug.LogError("Trigger Volume error @ Object " + base.gameObject.name);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public void CollectState(NetStream stream)
	{
		NetBoolEncoder.CollectState(stream, isActive);
	}

	private void ApplyState(bool state)
	{
		if (state && !isActive)
		{
			SetEnterObjectState(isPlayer: false);
		}
		else if (!state && isActive)
		{
			SetEnterObjectState(isPlayer: false);
		}
	}

	public void ApplyState(NetStream state)
	{
		ApplyState(NetBoolEncoder.ApplyState(state));
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		ApplyState(NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		NetBoolEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return NetBoolEncoder.CalculateMaxDeltaSizeInBits();
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (output != null)
		{
			output.SetValue(outputValueOutside);
		}
		colliderCount = 0;
	}
}
