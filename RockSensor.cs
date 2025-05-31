using System.Text;
using HumanAPI;
using UnityEngine;

public class RockSensor : MonoBehaviour
{
	public Rock.RockType rockType;

	public CartController cart;

	public MetalGratesController gratesController;

	public NodeGraph gaugeNodeGraph;

	public HingeJoint hingeJoint;

	private int rockTypeIndex;

	private float hingeRange;

	private void Start()
	{
		rockTypeIndex = Encoding.ASCII.GetBytes(rockType.ToString())[0] - 65;
		hingeRange = hingeJoint.limits.max - hingeJoint.limits.min;
	}

	public void OnTriggerEnter(Collider other)
	{
		Rock component = other.GetComponent<Rock>();
		if (!(component != null))
		{
			return;
		}
		if (component.type == rockType)
		{
			gratesController.AddRock(rockTypeIndex);
			if (gratesController.rockCounter[rockTypeIndex] >= gratesController.rockTypeMax[rockTypeIndex])
			{
				cart.StopMoving();
			}
			else
			{
				component.Despawn();
			}
			float num = (float)gratesController.rockCounter[rockTypeIndex] / (float)gratesController.rockTypeMax[rockTypeIndex];
			gaugeNodeGraph.inputs[0].inputSocket.SetValue(num);
			hingeJoint.SetLimits(CalcHingeLimitFromPower(num));
		}
		else
		{
			component.Despawn();
		}
	}

	private JointLimits CalcHingeLimitFromPower(float powerPerc)
	{
		JointLimits result = default(JointLimits);
		result.min = hingeJoint.limits.min;
		result.max = hingeRange * powerPerc + hingeJoint.limits.min;
		return result;
	}
}
