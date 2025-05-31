using System.Collections;
using UnityEngine;

public class LerpLocalRotation : MonoBehaviour
{
	private Quaternion startingRotation;

	private Quaternion currentRotation;

	public float duration = 1f;

	private void Start()
	{
		startingRotation = base.transform.localRotation;
	}

	public void CheckRotation()
	{
		currentRotation = base.transform.localRotation;
		if (currentRotation != startingRotation)
		{
			StartCoroutine(ReturnToRotation());
		}
	}

	private IEnumerator ReturnToRotation()
	{
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			base.transform.localRotation = Quaternion.Lerp(currentRotation, startingRotation, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		base.transform.localRotation = startingRotation;
	}
}
