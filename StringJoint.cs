using UnityEngine;

public class StringJoint : MonoBehaviour
{
	public Rigidbody connectedBody;

	public Vector3 anchor;

	public bool autoConfigureConnectedAnchor;

	public Vector3 connectedAnchor;

	public bool autoconfigureStringLength;

	public float stringLength;

	private void Awake()
	{
		Vector3 vector = base.transform.TransformPoint(anchor);
		if (autoConfigureConnectedAnchor)
		{
			connectedAnchor = connectedBody.transform.InverseTransformPoint(vector);
		}
		Vector3 vector2 = connectedBody.transform.TransformPoint(connectedAnchor);
		if (autoconfigureStringLength)
		{
			stringLength = (vector - vector2).magnitude;
		}
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.anchor = anchor;
		configurableJoint.autoConfigureConnectedAnchor = false;
		configurableJoint.connectedBody = connectedBody;
		configurableJoint.connectedAnchor = connectedAnchor;
		SoftJointLimit softJointLimit = default(SoftJointLimit);
		softJointLimit.limit = stringLength;
		SoftJointLimit linearLimit = softJointLimit;
		configurableJoint.linearLimit = linearLimit;
		ConfigurableJointMotion configurableJointMotion2 = (configurableJoint.zMotion = ConfigurableJointMotion.Limited);
		configurableJointMotion2 = (configurableJoint.yMotion = configurableJointMotion2);
		configurableJoint.xMotion = configurableJointMotion2;
		configurableJointMotion2 = (configurableJoint.angularZMotion = ConfigurableJointMotion.Free);
		configurableJointMotion2 = (configurableJoint.angularYMotion = configurableJointMotion2);
		configurableJoint.angularXMotion = configurableJointMotion2;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(base.transform.TransformPoint(anchor), Vector3.one * 0.2f);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(connectedBody.transform.TransformPoint(connectedAnchor), Vector3.one * 0.2f);
	}
}
