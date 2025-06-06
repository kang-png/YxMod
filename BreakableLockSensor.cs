using UnityEngine;

public class BreakableLockSensor : MonoBehaviour
{
	[Tooltip("Defines the Lock which needs to break")]
	public BreakableLock theLock;

	[Tooltip("Use ths in order to print debug info to the Log")]
	public bool showDebug;

	public void OnCollisionEnter(Collision collision)
	{
		if (theLock != null)
		{
			theLock.OnCollisionEnter(collision);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (theLock != null)
		{
			theLock.OnCollisionStay(collision);
		}
	}
}
