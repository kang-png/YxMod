using UnityEngine;

public class StopRigidBodyDrift : MonoBehaviour
{
	private Rigidbody rb;

	private float initX;

	private float initY;

	private float initZ;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		rb.centerOfMass = Vector3.zero;
		rb.inertiaTensorRotation = Quaternion.identity;
	}
}
