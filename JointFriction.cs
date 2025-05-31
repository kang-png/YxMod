using UnityEngine;

public class JointFriction : MonoBehaviour
{
	[Tooltip("mulitiplier for the angular velocity for the torque to apply.")]
	public float Friction = 0.4f;

	private HingeJoint _hinge;

	private Rigidbody _thisBody;

	private Rigidbody _connectedBody;

	private Vector3 _axis;

	private void Start()
	{
		_hinge = GetComponent<HingeJoint>();
		_connectedBody = _hinge.connectedBody;
		_axis = _hinge.axis;
		_thisBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		float velocity = _hinge.velocity;
		Vector3 vector = base.transform.TransformVector(_axis);
		Vector3 vector2 = Friction * velocity * vector;
		_thisBody.AddTorque(-vector2);
		_connectedBody.AddTorque(vector2);
	}
}
