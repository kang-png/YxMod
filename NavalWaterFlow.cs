using UnityEngine;

public class NavalWaterFlow : MonoBehaviour
{
	public Vector3 flowDirection;

	public float flowForce;

	private void OnTriggerStay(Collider other)
	{
		FloatingMesh component = other.GetComponent<FloatingMesh>();
		if (!(component == null))
		{
			Rigidbody component2 = component.GetComponent<Rigidbody>();
			component2.AddForce(base.transform.TransformDirection(flowDirection.normalized) * flowForce);
		}
	}
}
