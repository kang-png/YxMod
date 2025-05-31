using UnityEngine;
using UnityEngine.Events;

public class ImpactSensor : MonoBehaviour
{
	public float threshold = 10f;

	public UnityEvent onThresholdExceeded;

	private void OnCollisionStay(Collision collision)
	{
		if (collision.impulse.magnitude > threshold)
		{
			onThresholdExceeded.Invoke();
		}
	}
}
