using UnityEngine;

public class SharkMove : MonoBehaviour
{
	public float speed;

	public float timeToReverse;

	private bool isTurning;

	private bool isReversing;

	private float timeStart;

	private float newAngle;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (isTurning)
		{
			base.transform.localEulerAngles = new Vector3(0f, Mathf.MoveTowardsAngle(base.transform.localEulerAngles.y, newAngle, 1f), 0f);
			if (Mathf.Approximately(newAngle, base.transform.localEulerAngles.y))
			{
				isTurning = false;
			}
		}
		else if (isReversing)
		{
			rb.velocity = base.transform.forward * (0f - speed);
			if (Time.time > timeStart + timeToReverse)
			{
				isReversing = false;
				isTurning = true;
				Vector3 localEulerAngles = base.transform.localEulerAngles;
				base.transform.Rotate(0f, 180 + Random.Range(-90, 90), 0f);
				newAngle = base.transform.localEulerAngles.y;
				base.transform.localEulerAngles = localEulerAngles;
			}
		}
		else
		{
			rb.velocity = base.transform.forward * speed;
			base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!isReversing && !isTurning)
		{
			isReversing = true;
			timeStart = Time.time;
		}
	}
}
