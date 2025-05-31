using UnityEngine;

public class BoulderHandler : MonoBehaviour
{
	public float forceToKnockPlayer;

	public ForceMode forceMode;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.collider.gameObject.tag != "Player"))
		{
			Rigidbody component = collision.collider.gameObject.GetComponent<Rigidbody>();
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
			component.AddExplosionForce(forceToKnockPlayer, component.position, 10f, 20f, forceMode);
		}
	}
}
