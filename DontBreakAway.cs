using UnityEngine;

public class DontBreakAway : MonoBehaviour
{
	public float thresholdForce = 10000f;

	public float pullBackForceMultiplier = 1f;

	private FixedJoint joint;

	private void Start()
	{
		joint = GetComponent<FixedJoint>();
	}

	private void Update()
	{
		Debug.Log($"joint force: {joint.currentForce.magnitude}");
		if (joint.currentForce.magnitude > thresholdForce)
		{
			Debug.Log("pulled");
			joint.connectedBody.AddForceAtPosition(-joint.currentForce * pullBackForceMultiplier, base.transform.position, ForceMode.Force);
		}
	}
}
