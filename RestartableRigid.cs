using UnityEngine;

public class RestartableRigid : MonoBehaviour, IReset
{
	private struct RigidState
	{
		public Rigidbody rigid;

		public Vector3 position;

		public Quaternion rotation;

		public bool recorded;

		public Vector3 recordedPosition;

		public Quaternion recordedRotation;
	}

	private struct JointState
	{
		private bool valid;

		private bool isHinge;

		private bool isConfig;

		public Joint joint;

		public Rigidbody rigid;

		public Vector3 anchor;

		public Vector3 axis;

		public float breakForce;

		public float breakTorque;

		public Vector3 connectedAnchor;

		public Rigidbody connectedBody;

		public bool enableCollision;

		public bool enablePreprocessing;

		public JointLimits limits;

		public JointMotor motor;

		public JointSpring spring;

		public bool useLimits;

		public bool useMotor;

		public bool useSpring;

		public SoftJointLimitSpring softLimits;

		public ConfigurableJointMotion xMotion;

		public ConfigurableJointMotion yMotion;

		public ConfigurableJointMotion zMotion;

		public ConfigurableJointMotion aXMotion;

		public ConfigurableJointMotion aYMotion;

		public ConfigurableJointMotion aZMotion;

		public static JointState FromJoint(Joint joint)
		{
			JointState jointState = default(JointState);
			jointState.joint = joint;
			jointState.rigid = joint.GetComponent<Rigidbody>();
			jointState.anchor = joint.anchor;
			jointState.axis = joint.axis;
			jointState.breakForce = joint.breakForce;
			jointState.breakTorque = joint.breakTorque;
			jointState.connectedAnchor = joint.connectedAnchor;
			jointState.connectedBody = joint.connectedBody;
			jointState.enableCollision = joint.enableCollision;
			jointState.enablePreprocessing = joint.enablePreprocessing;
			JointState result = jointState;
			HingeJoint hingeJoint = joint as HingeJoint;
			if (hingeJoint != null)
			{
				result.isHinge = true;
				result.limits = hingeJoint.limits;
				result.motor = hingeJoint.motor;
				result.spring = hingeJoint.spring;
				result.useLimits = hingeJoint.useLimits;
				result.useMotor = hingeJoint.useMotor;
				result.useSpring = hingeJoint.useSpring;
				result.valid = true;
			}
			else if (joint is FixedJoint)
			{
				result.isHinge = false;
				result.valid = true;
			}
			else if (joint is ConfigurableJoint)
			{
				ConfigurableJoint configurableJoint = joint as ConfigurableJoint;
				result.softLimits = configurableJoint.linearLimitSpring;
				result.xMotion = configurableJoint.xMotion;
				result.yMotion = configurableJoint.yMotion;
				result.zMotion = configurableJoint.zMotion;
				result.aXMotion = configurableJoint.angularXMotion;
				result.aYMotion = configurableJoint.angularYMotion;
				result.aZMotion = configurableJoint.angularZMotion;
				result.isConfig = true;
				result.valid = true;
			}
			return result;
		}

		public Joint RecreateJoint()
		{
			if (!valid)
			{
				return null;
			}
			if (joint == null)
			{
				HingeJoint hingeJoint = null;
				ConfigurableJoint configurableJoint = null;
				if (isHinge)
				{
					hingeJoint = (HingeJoint)(joint = rigid.gameObject.AddComponent<HingeJoint>());
				}
				else if (isConfig)
				{
					configurableJoint = (ConfigurableJoint)(joint = rigid.gameObject.AddComponent<ConfigurableJoint>());
				}
				else
				{
					joint = rigid.gameObject.AddComponent<FixedJoint>();
				}
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedBody = connectedBody;
				joint.anchor = anchor;
				joint.axis = axis;
				joint.breakForce = breakForce;
				joint.breakTorque = breakTorque;
				joint.connectedAnchor = connectedAnchor;
				joint.enableCollision = enableCollision;
				joint.enablePreprocessing = enablePreprocessing;
				if (hingeJoint != null)
				{
					hingeJoint.limits = limits;
					hingeJoint.motor = motor;
					hingeJoint.spring = spring;
					hingeJoint.useLimits = useLimits;
					hingeJoint.useMotor = useMotor;
					hingeJoint.useSpring = useSpring;
				}
				if (configurableJoint != null)
				{
					configurableJoint.linearLimitSpring = softLimits;
					configurableJoint.xMotion = xMotion;
					configurableJoint.yMotion = yMotion;
					configurableJoint.zMotion = zMotion;
					configurableJoint.angularXMotion = aXMotion;
					configurableJoint.angularYMotion = aYMotion;
					configurableJoint.angularZMotion = aZMotion;
				}
			}
			return joint;
		}
	}

	public bool resetJoints;

	private RigidState[] initialState;

	private JointState[] jointState;

	private void OnEnable()
	{
		if (resetJoints && initialState == null)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			initialState = new RigidState[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ref RigidState reference = ref initialState[i];
				reference = new RigidState
				{
					recorded = false,
					rigid = componentsInChildren[i],
					position = componentsInChildren[i].transform.position,
					rotation = componentsInChildren[i].transform.rotation
				};
			}
			Joint[] componentsInChildren2 = GetComponentsInChildren<Joint>();
			jointState = new JointState[componentsInChildren2.Length];
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				ref JointState reference2 = ref jointState[j];
				reference2 = JointState.FromJoint(componentsInChildren2[j]);
			}
		}
	}

	public void Reset(Vector3 offset)
	{
		if (resetJoints)
		{
			for (int i = 0; i < jointState.Length; i++)
			{
				jointState[i].RecreateJoint();
			}
		}
		Rigidbody component = GetComponent<Rigidbody>();
		if ((bool)component)
		{
			component.velocity = Vector3.zero;
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		Reset(Vector3.zero);
	}

	private string GetGameObjectPath(Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}

	public void SetRecordedInfo(string rigidBodyName, Vector3 pos, Quaternion rot)
	{
		if (initialState == null)
		{
			return;
		}
		for (int i = 0; i < initialState.Length; i++)
		{
			string gameObjectPath = GetGameObjectPath(initialState[i].rigid.transform);
			if (gameObjectPath == rigidBodyName)
			{
				RigidState rigidState = initialState[i];
				rigidState.recorded = true;
				rigidState.recordedPosition = pos;
				rigidState.recordedRotation = rot;
				initialState[i] = rigidState;
				rigidState.rigid.MovePosition(pos);
				rigidState.rigid.MoveRotation(rot);
				rigidState.rigid.transform.position = pos;
				rigidState.rigid.transform.rotation = rot;
				Rigidbody rigid = rigidState.rigid;
				Vector3 zero = Vector3.zero;
				rigidState.rigid.velocity = zero;
				rigid.angularVelocity = zero;
				rigidState.rigid.Sleep();
				break;
			}
		}
	}
}
