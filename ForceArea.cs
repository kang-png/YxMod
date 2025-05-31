using UnityEngine;

public class ForceArea : MonoBehaviour
{
	public Vector3 forceDirection;

	public float forceMultiplier = 1f;

	public Transform[] ignoreParents;

	public void OnEnable()
	{
		Collider component = GetComponent<Collider>();
		for (int i = 0; i < ignoreParents.Length; i++)
		{
			Collider[] componentsInChildren = ignoreParents[i].GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Physics.IgnoreCollision(component, componentsInChildren[j]);
			}
		}
	}

	public void OnTriggerStay(Collider other)
	{
		Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
		if (!(componentInParent == null) && !componentInParent.isKinematic)
		{
			componentInParent.AddForce(forceDirection * forceMultiplier);
		}
	}
}
