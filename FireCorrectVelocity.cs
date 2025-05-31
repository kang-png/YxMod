using UnityEngine;

public class FireCorrectVelocity : MonoBehaviour
{
	private Rigidbody rb;

	public float speed = 40f;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Fire()
	{
		if ((bool)rb)
		{
			rb.angularVelocity = Vector3.zero;
			rb.velocity = base.transform.forward * speed;
		}
	}
}
