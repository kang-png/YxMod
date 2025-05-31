using UnityEngine;

public static class RigidbodyExtensions
{
	public static void ResetDynamics(this Rigidbody body)
	{
		Vector3 velocity = (body.angularVelocity = Vector3.zero);
		body.velocity = velocity;
	}
}
