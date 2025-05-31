using UnityEngine;

public class ScrewHinge : MonoBehaviour
{
	private Quaternion preAngle;

	private float preHeight;

	[SerializeField]
	private float deadAngle;

	[SerializeField]
	private float diffAngle;

	[SerializeField]
	private float diffHeight;

	private Rigidbody rb;

	private Rigidbody vrb;

	[SerializeField]
	private float screwRatio;

	[SerializeField]
	private GameObject vertical;

	[SerializeField]
	private float heightMax;

	[SerializeField]
	private float heightMin;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		vrb = vertical.GetComponent<Rigidbody>();
		preAngle = base.transform.rotation;
		preHeight = base.transform.position.y;
	}

	private void Update()
	{
		diffAngle = Quaternion.Angle(base.transform.rotation, preAngle);
		if (diffAngle > deadAngle)
		{
			vrb.isKinematic = true;
			if (preAngle.eulerAngles.y > base.transform.eulerAngles.y)
			{
				diffAngle = 0f - diffAngle;
			}
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + diffAngle * screwRatio, base.transform.position.z);
			vrb.MovePosition(position);
		}
		else
		{
			vrb.isKinematic = false;
			diffHeight = vertical.transform.position.y - preHeight;
			Vector3 euler = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y + diffHeight / screwRatio, base.transform.eulerAngles.z);
			vrb.MoveRotation(Quaternion.Euler(euler));
		}
		preHeight = vertical.transform.position.y;
		preAngle = base.transform.rotation;
	}
}
