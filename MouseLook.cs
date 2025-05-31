using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public float verticalSensitivity = 1f;

	public float horizontalSensitivity = 1f;

	public bool mouseCenteredAndHidden;

	public bool allowEscape;

	private Vector2 total;

	private Vector2 sensitivityModifier;

	private bool lockCursor;

	private void Start()
	{
		lockCursor = mouseCenteredAndHidden;
		sensitivityModifier = new Vector3(horizontalSensitivity, verticalSensitivity);
	}

	private void Update()
	{
		if (allowEscape && Input.GetKeyDown(KeyCode.Escape))
		{
			Screen.lockCursor = !Screen.lockCursor;
		}
		total += Vector2.Scale(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), sensitivityModifier);
		Quaternion quaternion = Quaternion.AngleAxis(total.x, Vector3.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(Mathf.Clamp(0f - total.y, -90f, 90f), Vector3.right);
		base.transform.rotation = quaternion * quaternion2;
	}
}
