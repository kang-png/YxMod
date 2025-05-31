using UnityEngine;

public class WaterDisplacement : MonoBehaviour
{
	private float scaleTarget = 1f;

	[SerializeField]
	private Transform waterLevel;

	[SerializeField]
	private float displacementMultiplier = 1f;

	private void Start()
	{
		scaleTarget = base.transform.localScale.y;
	}

	private void Update()
	{
		float y = Mathf.Lerp(base.transform.localScale.y, scaleTarget, 2.5f * Time.deltaTime);
		base.transform.localScale = new Vector3(base.transform.localScale.x, y, base.transform.localScale.z);
		waterLevel.position = new Vector3(waterLevel.position.x, base.transform.position.y + base.transform.localScale.y, waterLevel.position.z);
	}

	private void OnTriggerEnter(Collider collision)
	{
		Debug.Log("Water Triggered");
		if ((bool)collision.gameObject.GetComponent<VolumeFinder>())
		{
			float volume = collision.gameObject.GetComponent<VolumeFinder>().volume;
			AddVolume(volume);
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		Debug.Log("Water Triggered");
		if ((bool)collision.gameObject.GetComponent<VolumeFinder>())
		{
			float volume = collision.gameObject.GetComponent<VolumeFinder>().volume;
			AddVolume(0f - volume);
		}
	}

	private void AddVolume(float volume2Add)
	{
		scaleTarget = displacementMultiplier * volume2Add / base.transform.localScale.x / base.transform.localScale.z + base.transform.localScale.y;
		Debug.Log("Water rose to " + scaleTarget);
	}
}
