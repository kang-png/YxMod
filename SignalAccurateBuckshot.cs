using HumanAPI;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SignalAccurateBuckshot : Node
{
	public NodeOutput output;

	[SerializeField]
	private BuckshotTracker targetTracker;

	[Tooltip("Defines the longest time (in seconds) between a \"buckshot\" being fired and one of the cannonballs reaching the target for the shot to be considered accurate")]
	public float maximumShotTravelTime = 0.8f;

	private bool isExpectingCannonballs;

	private float remainingShotWaitingTime = -1f;

	private void Awake()
	{
		targetTracker.ShotFired += OnShotFired;
	}

	private void FixedUpdate()
	{
		if (remainingShotWaitingTime > 0f)
		{
			remainingShotWaitingTime -= Time.fixedDeltaTime;
		}
		else
		{
			remainingShotWaitingTime = -1f;
		}
		if (isExpectingCannonballs && remainingShotWaitingTime <= 0f)
		{
			isExpectingCannonballs = false;
		}
	}

	private void OnShotFired(bool isBuckshot)
	{
		if (isBuckshot)
		{
			isExpectingCannonballs = true;
			remainingShotWaitingTime = maximumShotTravelTime;
		}
		else
		{
			output.SetValue(0f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (targetTracker.TrackedBalls.Contains(other) && isExpectingCannonballs)
		{
			output.SetValue(1f);
		}
	}
}
