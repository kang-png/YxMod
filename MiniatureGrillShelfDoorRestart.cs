using UnityEngine;

public class MiniatureGrillShelfDoorRestart : MonoBehaviour, IReset
{
	public float checkpointThreshold = 5f;

	private FixedJoint fixedJoint;

	private float fixedJointBreakForce;

	private void Start()
	{
		fixedJoint = GetComponent<FixedJoint>();
		fixedJointBreakForce = fixedJoint.breakForce;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if ((float)checkpoint < checkpointThreshold)
		{
			FixedJoint fixedJoint = base.gameObject.AddComponent<FixedJoint>();
			fixedJoint.breakForce = fixedJointBreakForce;
		}
	}
}
