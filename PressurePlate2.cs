using UnityEngine;

public class PressurePlate2 : SignalBase
{
	public Transform sensor;

	public float pressTreshold = 0.06f;

	public float releaseTreshold = 0.12f;

	public float holdState = 1f;

	private float timer;

	private void Start()
	{
		sensor.GetComponent<Rigidbody>().sleepThreshold *= 0.1f;
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (base.boolValue && sensor.localPosition.y > releaseTreshold && timer <= 0f)
		{
			SetValue(0f);
			timer = holdState;
		}
		if (!base.boolValue && sensor.localPosition.y < pressTreshold && timer <= 0f)
		{
			SetValue(1f);
			timer = holdState;
		}
	}
}
