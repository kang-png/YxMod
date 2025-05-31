using UnityEngine;

public class ResetVelocity : MonoBehaviour, IReset
{
	public void ResetState(int checkpoint, int subObjectives)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if ((bool)component)
		{
			component.velocity = Vector3.zero;
		}
	}
}
