using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ResetPositionAtTrigger : MonoBehaviour
{
	public Transform postionAtReset;

	public bool isKinematicAfterReset = true;

	private Rigidbody rb;

	public void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
	}

	public void ResetPostion()
	{
		rb.isKinematic = isKinematicAfterReset;
		base.transform.position = postionAtReset.position;
	}
}
