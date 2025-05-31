using UnityEngine;

public class RotateAroundNoPhysics : MonoBehaviour
{
	private Rigidbody rigidBody;

	public Vector3 velocity;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Quaternion quaternion = Quaternion.Euler(velocity * Time.deltaTime);
		rigidBody.MoveRotation(rigidBody.rotation * quaternion);
	}
}
