using System.Collections;
using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
	public Vector3 directionToMove = new Vector3(1f, 0f, 0f);

	public float moveDuration = 2f;

	public float speed = 1f;

	private Vector3 startLoc;

	private void Awake()
	{
		startLoc = base.transform.position;
	}

	public void StartMove()
	{
		StartCoroutine(MoveObject(moveDuration));
	}

	private IEnumerator MoveObject(float duration)
	{
		float elapsedTime = 0f;
		Vector3 startingPos = base.transform.position;
		while (elapsedTime < duration)
		{
			base.transform.position += directionToMove * Time.deltaTime * speed;
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	public void ResetPosition()
	{
		base.transform.position = startLoc;
	}
}
