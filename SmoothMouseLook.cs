using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class SmoothMouseLook : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public RotationAxes axes;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationX;

	private float rotationY;

	private List<float> rotArrayX = new List<float>();

	private float rotAverageX;

	private List<float> rotArrayY = new List<float>();

	private float rotAverageY;

	public float frameCounter = 20f;

	private Quaternion originalRotation;

	private Quaternion parentOriginalRotation;

	private void Update()
	{
		if (!Input.GetButton("Look"))
		{
			return;
		}
		if (axes == RotationAxes.MouseXAndY)
		{
			rotAverageY = 0f;
			rotAverageX = 0f;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotArrayY.Add(rotationY);
			rotArrayX.Add(rotationX);
			if ((float)rotArrayY.Count >= frameCounter)
			{
				rotArrayY.RemoveAt(0);
			}
			if ((float)rotArrayX.Count >= frameCounter)
			{
				rotArrayX.RemoveAt(0);
			}
			for (int i = 0; i < rotArrayY.Count; i++)
			{
				rotAverageY += rotArrayY[i];
			}
			for (int j = 0; j < rotArrayX.Count; j++)
			{
				rotAverageX += rotArrayX[j];
			}
			rotAverageY /= rotArrayY.Count;
			rotAverageX /= rotArrayX.Count;
			rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
			rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);
			Quaternion quaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
			Quaternion quaternion2 = Quaternion.AngleAxis(rotAverageX, Vector3.up);
			base.transform.localRotation = originalRotation * quaternion;
			base.transform.parent.localRotation = parentOriginalRotation * quaternion2;
		}
		else if (axes == RotationAxes.MouseX)
		{
			rotAverageX = 0f;
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotArrayX.Add(rotationX);
			if ((float)rotArrayX.Count >= frameCounter)
			{
				rotArrayX.RemoveAt(0);
			}
			for (int k = 0; k < rotArrayX.Count; k++)
			{
				rotAverageX += rotArrayX[k];
			}
			rotAverageX /= rotArrayX.Count;
			rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);
			Quaternion quaternion3 = Quaternion.AngleAxis(rotAverageX, Vector3.up);
			base.transform.parent.localRotation = parentOriginalRotation * quaternion3;
		}
		else
		{
			rotAverageY = 0f;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotArrayY.Add(rotationY);
			if ((float)rotArrayY.Count >= frameCounter)
			{
				rotArrayY.RemoveAt(0);
			}
			for (int l = 0; l < rotArrayY.Count; l++)
			{
				rotAverageY += rotArrayY[l];
			}
			rotAverageY /= rotArrayY.Count;
			rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
			Quaternion quaternion4 = Quaternion.AngleAxis(rotAverageY, Vector3.left);
			base.transform.localRotation = originalRotation * quaternion4;
		}
	}

	private void Start()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if ((bool)component)
		{
			component.freezeRotation = true;
		}
		originalRotation = base.transform.localRotation;
		parentOriginalRotation = base.transform.parent.localRotation;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle %= 360f;
		if (angle >= -360f && angle <= 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}
}
