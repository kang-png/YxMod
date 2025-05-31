using UnityEngine;

public class PartsExplode : MonoBehaviour
{
	public Rigidbody[] bodiesToExplode;

	public void Explode()
	{
		if (bodiesToExplode.Length != 0)
		{
			Rigidbody[] array = bodiesToExplode;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.isKinematic = false;
				rigidbody.AddForceAtPosition(new Vector3(0f, 0.05f, -0.05f), rigidbody.transform.position);
			}
		}
	}
}
