using UnityEngine;

public class ScrewHinge1 : MonoBehaviour
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

	[SerializeField]
	private float screwRatio;

	[SerializeField]
	private float heightMax;

	[SerializeField]
	private float heightMin;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		preAngle = base.transform.rotation;
		preHeight = base.transform.position.y;
	}

	private void Update()
	{
		preHeight = base.transform.position.y;
		diffAngle = Quaternion.Angle(base.transform.rotation, preAngle);
		if (diffAngle > deadAngle)
		{
			if (preAngle.eulerAngles.y > base.transform.eulerAngles.y)
			{
				diffAngle = 0f - diffAngle;
			}
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + diffAngle * screwRatio, base.transform.position.z);
			bool flag = false;
			if (position.y > heightMax && preAngle.eulerAngles.y < base.transform.eulerAngles.y)
			{
				flag = true;
			}
			if (position.y < heightMin && preAngle.eulerAngles.y > base.transform.eulerAngles.y)
			{
				flag = true;
			}
			if (!flag)
			{
				rb.MovePosition(position);
			}
			else
			{
				base.transform.rotation = preAngle;
			}
		}
		preAngle = base.transform.rotation;
	}
}
