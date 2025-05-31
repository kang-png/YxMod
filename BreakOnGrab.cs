using UnityEngine;

public class BreakOnGrab : MonoBehaviour, IGrabbable
{
	public void OnGrab()
	{
		Object.Destroy(GetComponent<FixedJoint>());
	}

	public void OnRelease()
	{
	}
}
