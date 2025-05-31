using UnityEngine;

public class NavalWaterWheelElevator : MonoBehaviour
{
	public Transform wheelElevatorPivot;

	public float pivotBottom = 6.2f;

	public float pivotTop = 10.9f;

	public float selfMin;

	public float selfMax = 4f;

	public int maxFramesIdle = 30;

	public Vector3 idleSpringNudge;

	private SpringJoint spring;

	private Rigidbody rigidBody;

	private float previousSpringAppliedForce;

	private int idleFrameCount;

	private void Start()
	{
		spring = GetComponent<SpringJoint>();
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		float num = wheelElevatorPivot.position.y - pivotBottom;
		float num2 = pivotTop - pivotBottom;
		float num3 = selfMax - selfMin;
		float maxDistance = num / num2 * num3;
		spring.maxDistance = maxDistance;
		if (spring.currentForce.sqrMagnitude == previousSpringAppliedForce)
		{
			idleFrameCount++;
		}
		if (idleFrameCount > maxFramesIdle)
		{
			rigidBody.AddForce(idleSpringNudge);
			idleFrameCount = 0;
		}
		previousSpringAppliedForce = spring.currentForce.sqrMagnitude;
	}
}
