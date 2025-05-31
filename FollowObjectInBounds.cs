using UnityEngine;

public class FollowObjectInBounds : MonoBehaviour
{
	public GameObject objectToFollow;

	[Tooltip("The boundaries this object should be constrained to")]
	public Collider boundaries;

	private void FixedUpdate()
	{
		if ((bool)objectToFollow)
		{
			base.transform.position = boundaries.ClosestPoint(objectToFollow.transform.position);
		}
	}
}
