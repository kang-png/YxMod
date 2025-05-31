using UnityEngine;

public class ResetPosAndRot : MonoBehaviour
{
	[SerializeField]
	private Vector3 startingPos;

	[SerializeField]
	private Quaternion startingRotation;

	[SerializeField]
	private Transform transformToReset;

	public void ResetPositionRotation()
	{
		transformToReset.localPosition = startingPos;
		transformToReset.rotation = startingRotation;
	}
}
