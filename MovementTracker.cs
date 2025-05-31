using System.Collections.Generic;
using HumanAPI;
using UnityEngine;

public class MovementTracker : Node
{
	public TrackingTriggerVolume startTrigger;

	public TrackingTriggerVolume floorTrigger;

	public TrackingTriggerVolume endTrigger;

	public NodeOutput output;

	private List<Collider> playerColliders = new List<Collider>();

	private void Start()
	{
		if (startTrigger != null)
		{
			startTrigger.OnEnterCollider += StartTracking;
		}
		if (floorTrigger != null)
		{
			floorTrigger.OnEnterCollider += StopTracking;
		}
		if (endTrigger != null)
		{
			endTrigger.OnEnterCollider += FinishTracking;
		}
	}

	private void StartTracking(Collider collider)
	{
		Debug.Log("StartTracking=" + collider.name);
		if (!playerColliders.Contains(collider))
		{
			playerColliders.Add(collider);
		}
	}

	private void StopTracking(Collider collider)
	{
		Debug.Log("StopTracking=" + collider.name);
		playerColliders.Remove(collider);
		if (playerColliders.Count == 0)
		{
			output.SetValue(0f);
		}
	}

	private void FinishTracking(Collider collider)
	{
		Debug.Log("FinishTracking=" + collider.name);
		if (playerColliders.Contains(collider))
		{
			output.SetValue(1f);
		}
		playerColliders.Remove(collider);
	}
}
