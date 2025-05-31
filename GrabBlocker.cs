using UnityEngine;

public class GrabBlocker : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		OnTriggerStay(other);
	}

	public void OnTriggerStay(Collider other)
	{
		CollisionSensor component = other.GetComponent<CollisionSensor>();
		if ((bool)component)
		{
			component.ReleaseGrab();
		}
	}
}
