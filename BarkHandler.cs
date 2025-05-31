using UnityEngine;

public class BarkHandler : MonoBehaviour
{
	public Transform steamBoat;

	public Vector3 boatOffset;

	public void AlignWithBoatOnRespawn()
	{
		base.transform.localPosition = steamBoat.localPosition + boatOffset;
	}
}
