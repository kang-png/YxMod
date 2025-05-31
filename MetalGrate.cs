using UnityEngine;

public class MetalGrate : MonoBehaviour
{
	public float impulseForceToPryOpen = 10f;

	public Transform impulsePosition;

	public float openingForce = 1000f;

	public float massAfterOpening;

	public float limitAfterOpening;

	public float massWhenClosed;

	public float limitWhenClosed;

	private Rigidbody rigidBody;

	private HingeJoint hinge;

	public void OpenGrate()
	{
		rigidBody.AddForceAtPosition(openingForce * Vector3.up, impulsePosition.position, ForceMode.Impulse);
		rigidBody.mass = massAfterOpening;
		JointLimits jointLimits = default(JointLimits);
		jointLimits.min = limitAfterOpening;
		jointLimits.max = hinge.limits.max;
		jointLimits.bounciness = hinge.limits.bounciness;
		jointLimits.contactDistance = hinge.limits.contactDistance;
		jointLimits.bounceMinVelocity = hinge.limits.bounceMinVelocity;
		JointLimits limits = jointLimits;
		hinge.SetLimits(limits);
	}

	public void ResetGrate()
	{
		rigidBody.mass = massWhenClosed;
		JointLimits jointLimits = default(JointLimits);
		jointLimits.min = limitWhenClosed;
		jointLimits.max = hinge.limits.max;
		jointLimits.bounciness = hinge.limits.bounciness;
		jointLimits.contactDistance = hinge.limits.contactDistance;
		jointLimits.bounceMinVelocity = hinge.limits.bounceMinVelocity;
		JointLimits limits = jointLimits;
		hinge.SetLimits(limits);
	}

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		hinge = GetComponent<HingeJoint>();
	}
}
