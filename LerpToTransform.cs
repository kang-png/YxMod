using UnityEngine;

public class LerpToTransform : MonoBehaviour
{
	public GameObject objectToLerp;

	public Transform destinationTransform;

	public float lerpTime = 1f;

	private float t = 1.1f;

	private void Reset()
	{
		objectToLerp = base.gameObject;
	}

	private void OnValidate()
	{
		objectToLerp = objectToLerp ?? base.gameObject;
	}

	public void BeginLerp()
	{
		t = 0f;
	}

	private void FixedUpdate()
	{
		if (t < 1f)
		{
			t += Mathf.Clamp01(1f / lerpTime * Time.fixedDeltaTime);
			objectToLerp.transform.position = Vector3.Lerp(objectToLerp.transform.position, destinationTransform.position, t);
		}
	}
}
