using System.Collections;
using Multiplayer;
using UnityEngine;

public class RespawnPosSwitch : MonoBehaviour, IReset
{
	public Transform newPos;

	public NetBody[] body;

	private Vector3 basePos;

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (checkpoint != 0)
		{
			StartCoroutine(MoveBoat());
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator MoveBoat()
	{
		yield return new WaitForSeconds(0.5f);
		basePos = body[0].startPos;
		NetBody[] array = body;
		foreach (NetBody netBody in array)
		{
			Vector3 vector = netBody.startPos - basePos;
			netBody.transform.position = newPos.position + vector;
		}
	}
}
