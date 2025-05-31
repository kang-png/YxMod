using UnityEngine;

[RequireComponent(typeof(ConstantForce))]
public class TriggerConstantForce : MonoBehaviour
{
	[Tooltip("The force that is being applied when the player is holding 2 umbrellas's at the same time.")]
	public float ForceValueDouble = 1000f;

	private ConstantForce cf;

	private float originalForceValue;

	private GameObject otherUmbrella;

	private void OnTriggerStay(Collider other)
	{
		if (!otherUmbrella)
		{
			if (!cf)
			{
				cf = GetComponent<ConstantForce>();
			}
			if (originalForceValue == 0f || originalForceValue != cf.force.y)
			{
				originalForceValue = cf.force.y;
			}
			ConstantForce component = other.gameObject.GetComponent<ConstantForce>();
			if ((bool)component && component.isActiveAndEnabled && originalForceValue > 700f)
			{
				otherUmbrella = other.gameObject;
				cf.force = new Vector3(0f, ForceValueDouble, 0f);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == otherUmbrella)
		{
			cf.force = new Vector3(0f, originalForceValue, 0f);
			otherUmbrella = null;
		}
	}
}
