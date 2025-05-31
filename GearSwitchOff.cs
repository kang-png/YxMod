using HumanAPI;
using UnityEngine;

public class GearSwitchOff : MonoBehaviour
{
	public HingeJoint joint;

	public SignalAngle sig;

	private void FixedUpdate()
	{
		if (sig.currentValue == 1f)
		{
			joint.useMotor = false;
		}
		else
		{
			joint.useMotor = true;
		}
	}
}
