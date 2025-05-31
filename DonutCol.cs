using UnityEngine;

public class DonutCol : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		BoatStats componentInParent = collision.gameObject.GetComponentInParent<BoatStats>();
		if ((bool)componentInParent)
		{
			componentInParent.IncreaseCount();
		}
	}
}
