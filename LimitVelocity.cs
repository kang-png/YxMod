using UnityEngine;

public class LimitVelocity : MonoBehaviour
{
	public float maxSpeed = 100f;

	private Rigidbody body;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
		if (body == null)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (body != null && body.velocity.magnitude > maxSpeed)
		{
			body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
		}
	}
}
