using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
	public Transform light;

	public MeshRenderer earthRenderer;

	public MeshRenderer atmosphereRenderer;

	private float MIN_DIST = 200f;

	private float MAX_DIST = 5000f;

	private float dist = 400f;

	private Quaternion cameraRotation;

	private Vector2 targetOffCenter = Vector2.zero;

	private Vector2 offCenter = Vector2.zero;

	private void Start()
	{
		cameraRotation = Quaternion.LookRotation(-base.transform.position.normalized, Vector3.up);
	}

	private void Update()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			dist *= 0.87f;
		}
		else if (axis < 0f)
		{
			dist *= 1.15f;
		}
		if (dist < MIN_DIST)
		{
			dist = MIN_DIST;
		}
		else if (dist > MAX_DIST)
		{
			dist = MAX_DIST;
		}
		float axis2 = Input.GetAxis("Mouse X");
		float axis3 = Input.GetAxis("Mouse Y");
		float num = 100f;
		if (Input.GetButton("Fire1"))
		{
			if (axis2 != 0f || axis3 != 0f)
			{
				float num2 = Mathf.Min(2f, (dist - num) / num * 1.2f);
				cameraRotation *= Quaternion.AngleAxis(num2 * axis2, Vector3.up) * Quaternion.AngleAxis(num2 * axis3, Vector3.left);
			}
		}
		else if (Input.GetButton("Fire2"))
		{
			Quaternion rotation = light.rotation;
			rotation *= Quaternion.AngleAxis((0f - axis2) * 2f, Vector3.up);
			light.rotation = rotation;
		}
		else if (Input.GetButton("Fire3"))
		{
			targetOffCenter.x -= axis2 * 10f;
			targetOffCenter.y -= axis3 * 10f;
			float num3 = 0.5625f * (float)Screen.width / (float)Screen.height * Mathf.Tan(GetComponent<Camera>().fieldOfView / 2f) * dist / (float)Screen.height / 2.5f;
			offCenter.x = targetOffCenter.x * num3;
			offCenter.y = targetOffCenter.y * num3;
		}
		base.transform.rotation = cameraRotation;
		base.transform.position = cameraRotation * (-Vector3.forward * dist);
		base.transform.position += new Vector3(base.transform.right.x * offCenter.x + base.transform.up.x * offCenter.y, base.transform.right.y * offCenter.x + base.transform.up.y * offCenter.y, base.transform.right.z * offCenter.x + base.transform.up.z * offCenter.y);
	}
}
