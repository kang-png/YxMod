using UnityEngine;

public class VoronoiShatterLinked : MonoBehaviour
{
	public VoronoiShatter toShatter;

	public float impulseTreshold = 10f;

	public float breakTreshold = 100f;

	public float humanHardness;

	public float maxMomentum;

	private Vector3 adjustedImpulse = Vector3.zero;

	private Vector3 lastFrameImpact;

	private Vector3 maxImpactPoint;

	private float maxImpact;

	public void OnCollisionEnter(Collision collision)
	{
		if (!(toShatter == null) && !toShatter.shattered)
		{
			HandleCollision(collision, enter: false);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (!(toShatter == null) && !toShatter.shattered)
		{
			HandleCollision(collision, enter: false);
		}
	}

	private void HandleCollision(Collision collision, bool enter)
	{
		if (collision.contacts.Length == 0)
		{
			return;
		}
		Vector3 vector = -collision.GetImpulse();
		float magnitude = vector.magnitude;
		if (magnitude < impulseTreshold)
		{
			return;
		}
		float num = 1f;
		Transform parent = collision.transform;
		while (parent != null)
		{
			if ((bool)parent.GetComponent<Human>())
			{
				num = humanHardness;
				break;
			}
			ShatterHardness component = parent.GetComponent<ShatterHardness>();
			if (component != null)
			{
				num = component.hardness;
				break;
			}
			parent = parent.parent;
		}
		if (magnitude * num > maxImpact)
		{
			maxImpact = magnitude * num;
			maxImpactPoint = collision.contacts[0].point;
		}
		adjustedImpulse += vector * num;
	}

	private void FixedUpdate()
	{
		if (!(toShatter == null) && !toShatter.shattered)
		{
			if (adjustedImpulse.magnitude > maxMomentum)
			{
				maxMomentum = adjustedImpulse.magnitude;
			}
			maxImpact = 0f;
			if ((adjustedImpulse + lastFrameImpact).magnitude > breakTreshold)
			{
				toShatter.ShatterLocal(maxImpactPoint, adjustedImpulse + lastFrameImpact);
			}
			lastFrameImpact = adjustedImpulse;
			adjustedImpulse = Vector3.zero;
		}
	}
}
