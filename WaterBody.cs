using UnityEngine;

public abstract class WaterBody : WaterBodyCollider
{
	public bool canDrown;

	[Tooltip("Determines how fast the player will drown in this body of water. Values higher than 1 will accelerate the drowning, lower values will slow it down.")]
	public float drownSpeedFactor = 1f;

	public abstract float SampleDepth(Vector3 pos, out Vector3 velocity);

	public void ForceLeaveCollider(WaterSensor sensor)
	{
		OnTriggerExit(sensor.GetComponent<Collider>());
	}
}
