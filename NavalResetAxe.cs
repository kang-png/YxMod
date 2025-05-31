using Multiplayer;
using UnityEngine;

public class NavalResetAxe : MonoBehaviour
{
	[SerializeField]
	private Rigidbody axeRB;

	[SerializeField]
	private Rigidbody doorRB;

	private NetBody netBody;

	private void Awake()
	{
		netBody = GetComponent<NetBody>();
	}

	public void Reset()
	{
		if (doorRB.isKinematic)
		{
			axeRB.isKinematic = true;
		}
		else
		{
			axeRB.isKinematic = false;
		}
		if ((bool)netBody)
		{
			netBody.isKinematic = axeRB.isKinematic;
		}
	}

	public void Restart()
	{
		axeRB.isKinematic = true;
		if ((bool)netBody)
		{
			netBody.isKinematic = axeRB.isKinematic;
		}
	}
}
