using UnityEngine;

public static class ConfigurableJointExtensions
{
	public static void SetXMotionAnchorsAndLimits(this ConfigurableJoint joint, float centerOffset, float actionRange)
	{
		SoftJointLimit linearLimit = joint.linearLimit;
		linearLimit.limit = actionRange / 2f;
		joint.linearLimit = linearLimit;
		joint.xMotion = ConfigurableJointMotion.Limited;
		joint.autoConfigureConnectedAnchor = false;
		Vector3 vector = joint.transform.TransformPoint(joint.anchor + joint.axis * centerOffset);
		if (joint.connectedBody != null)
		{
			joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(vector);
		}
		else
		{
			joint.connectedAnchor = vector;
		}
	}

	public static void ApplyXMotionTarget(this ConfigurableJoint joint)
	{
		Vector3 vector = ((!(joint.connectedBody != null)) ? joint.connectedAnchor : joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));
		float limit = joint.linearLimit.limit;
		float num = joint.targetPosition.x;
		if (limit > 0f)
		{
			num = Mathf.Clamp(num, 0f - limit, limit);
		}
		Vector3 position = joint.anchor + joint.axis * num;
		Vector3 vector2 = joint.transform.TransformPoint(position);
		joint.transform.position += vector - vector2;
		Rigidbody component = joint.GetComponent<Rigidbody>();
		Vector3 angularVelocity = (component.velocity = Vector3.zero);
		component.angularVelocity = angularVelocity;
	}

	public static float GetXAngle(this Joint joint, Quaternion invInitialLocalRotation)
	{
		(invInitialLocalRotation * joint.transform.localRotation).ToAngleAxis(out var angle, out var axis);
		if (Vector3.Dot(axis, joint.axis) < 0f)
		{
			return 0f - angle;
		}
		return angle;
	}

	public static void SetXAngleTarget(this ConfigurableJoint joint, float angle)
	{
		Quaternion quaternion = Quaternion.AngleAxis(0f - angle, Vector3.right);
		if (joint.targetRotation != quaternion)
		{
			joint.targetRotation = quaternion;
		}
	}

	public static void ApplyXAngle(this Joint joint, Quaternion invInitialLocalRotation, float angle)
	{
		joint.transform.localRotation = Quaternion.Inverse(invInitialLocalRotation) * Quaternion.AngleAxis(angle, joint.axis);
		Rigidbody component = joint.GetComponent<Rigidbody>();
		Vector3 angularVelocity = (component.velocity = Vector3.zero);
		component.angularVelocity = angularVelocity;
	}

	public static Quaternion ReadInitialRotation(this Joint joint)
	{
		return Quaternion.Inverse(joint.transform.localRotation);
	}
}
