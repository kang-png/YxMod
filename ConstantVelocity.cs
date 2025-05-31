using UnityEngine;

public class ConstantVelocity : MonoBehaviour
{
	[SerializeField]
	private float velocity;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (rb != null)
		{
			rb.velocity = velocity * base.transform.forward * Time.deltaTime;
		}
	}

	private void OnDisable()
	{
		if (rb != null)
		{
			rb.velocity = Vector3.zero;
		}
	}
}
