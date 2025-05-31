using UnityEngine;

public class ChainFerry : MonoBehaviour
{
	[SerializeField]
	private Rigidbody boat;

	[SerializeField]
	private float maxDisp;

	private Vector3 startPos;

	private void Start()
	{
		startPos = boat.position;
	}

	private void Update()
	{
		if ((boat.position - startPos).z < 0f)
		{
			boat.MovePosition(startPos);
		}
		else if ((boat.position - startPos).z > maxDisp)
		{
			boat.MovePosition(startPos + maxDisp * Vector3.forward);
		}
	}
}
