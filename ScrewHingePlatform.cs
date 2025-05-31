using HumanAPI;
using UnityEngine;

public class ScrewHingePlatform : Node
{
	[SerializeField]
	private Rigidbody rotableRigidbody;

	[SerializeField]
	private float localMaxY;

	[SerializeField]
	private float localMinY;

	[SerializeField]
	private float speed = 5f;

	[SerializeField]
	private float maxAngularVelocityY = 1f;

	public NodeInput grabState;

	public NodeInput isCollidingBottom;

	private Vector3 topLocalPosition;

	private Vector3 bottomLocalPosition;

	private Vector3 maxAngularVelocityVector;

	private Vector3 targetPosition;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
		topLocalPosition = new Vector3(0f, localMaxY, 0f);
		bottomLocalPosition = new Vector3(0f, localMinY, 0f);
		maxAngularVelocityVector = new Vector3(0f, maxAngularVelocityY, 0f);
	}

	private void Update()
	{
		if (base.transform.localPosition.y >= bottomLocalPosition.y + 0.1f && grabState.value < 1f && isCollidingBottom.value != 1f)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Falling , player has let go ");
			}
			rotableRigidbody.AddRelativeTorque(Vector3.up * 8000f, ForceMode.Force);
		}
		if (rotableRigidbody.angularVelocity.y > maxAngularVelocityY)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Soinning too fast + ");
			}
			rotableRigidbody.angularVelocity = maxAngularVelocityVector;
		}
		else if (rotableRigidbody.angularVelocity.y < 0f - maxAngularVelocityY)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Soinning too fast - ");
			}
			rotableRigidbody.angularVelocity = -maxAngularVelocityVector;
		}
		if ((base.transform.localPosition.y >= topLocalPosition.y - 0.05f && rotableRigidbody.angularVelocity.y < 0f) || (base.transform.localPosition.y <= bottomLocalPosition.y + 0.05f && rotableRigidbody.angularVelocity.y > 0f) || (rotableRigidbody.angularVelocity.y > 0f && isCollidingBottom.value == 1f))
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Sector D ");
			}
			rotableRigidbody.angularVelocity = Vector3.zero;
		}
		else if (rotableRigidbody.angularVelocity.y > 0f)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Going Down");
			}
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, bottomLocalPosition, Time.fixedDeltaTime * speed * Mathf.Abs(rotableRigidbody.angularVelocity.y));
		}
		else if (rotableRigidbody.angularVelocity.y < 0f)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Going Up ");
			}
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, topLocalPosition, Time.fixedDeltaTime * speed * Mathf.Abs(rotableRigidbody.angularVelocity.y));
		}
	}
}
