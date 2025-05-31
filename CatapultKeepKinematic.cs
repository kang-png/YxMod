using UnityEngine;

public class CatapultKeepKinematic : MonoBehaviour
{
	private Rigidbody rigidBody;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	private void LateUpdate()
	{
		rigidBody.isKinematic = true;
	}
}
