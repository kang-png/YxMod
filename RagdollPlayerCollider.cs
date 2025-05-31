using UnityEngine;

public class RagdollPlayerCollider : MonoBehaviour
{
	public float ragdollDuration = 0.5f;

	public void OnCollisionEnter(Collision collision)
	{
		Rigidbody rigidbody = collision.rigidbody;
		if (rigidbody != null)
		{
			Human componentInParent = rigidbody.GetComponentInParent<Human>();
			if (componentInParent != null)
			{
				componentInParent.MakeUnconscious(ragdollDuration);
			}
		}
	}
}
