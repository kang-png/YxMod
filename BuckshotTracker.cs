using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HumanAPI;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuckshotTracker : Node
{
	public delegate void ShotFiredHandler(bool isBuckshot);

	public NodeOutput output;

	public NodeInput input;

	[Tooltip("List of cannonballs to track.")]
	public Collider[] ballsToTrack;

	[Tooltip("Number of cannonballs in the tracker for the shot to be considered a \"buckshot\"")]
	public int ballCountForBuckshot;

	[Tooltip("Defines how often (in seconds) a \"buckshot\" can be detected")]
	public float shotCooldown = 0.5f;

	private int loadedBallCount;

	private float remainingCooldown = -1f;

	public HashSet<Collider> TrackedBalls { get; private set; }

	public bool IsOnCooldown
	{
		[CompilerGenerated]
		get
		{
			return remainingCooldown > 0f;
		}
	}

	public event ShotFiredHandler ShotFired;

	public override void Process()
	{
		base.Process();
		if (input.value >= 0.5f && remainingCooldown <= 0f)
		{
			if (this.ShotFired != null)
			{
				this.ShotFired(loadedBallCount >= ballCountForBuckshot);
			}
			remainingCooldown = shotCooldown;
			output.SetValue(1f);
		}
	}

	private void Awake()
	{
		TrackedBalls = new HashSet<Collider>(ballsToTrack);
		loadedBallCount = 0;
	}

	private void Update()
	{
		if (remainingCooldown > 0f)
		{
			remainingCooldown -= Time.deltaTime;
			return;
		}
		remainingCooldown = -1f;
		output.SetValue(0f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (TrackedBalls.Contains(other))
		{
			loadedBallCount++;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (TrackedBalls.Contains(other))
		{
			loadedBallCount--;
		}
	}
}
