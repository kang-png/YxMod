using Multiplayer;
using UnityEngine;

public class NavalLog : MonoBehaviour
{
	private Rigidbody rigidBody;

	private float originalMass;

	private void Start()
	{
		if (!NetGame.isClient)
		{
			rigidBody = GetComponent<Rigidbody>();
			originalMass = rigidBody.mass;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!NetGame.isClient)
		{
			NavalLogWeightModifier component = other.GetComponent<NavalLogWeightModifier>();
			if (!(component == null))
			{
				rigidBody.mass = component.weightTarget;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!NetGame.isClient)
		{
			NavalLogWeightModifier component = other.GetComponent<NavalLogWeightModifier>();
			if (!(component == null))
			{
				rigidBody.mass = originalMass;
			}
		}
	}
}
