using Multiplayer;
using UnityEngine;

namespace HumanAPI;

public class ScriptCrumblingFloorWS1 : Node, INetBehavior
{
	[Tooltip("The time in seconds the player can stand on this thing")]
	public float standTime = 2f;

	[Tooltip("Write_something_here_to_explain_this_thing")]
	public float standRecovery = 4f;

	[Tooltip("The length of time in seconds the player can hold this thing before it breaks")]
	public float hangTime = 3f;

	[Tooltip("The length of time in seconds the player can use to recover when hanging onto this thing ")]
	public float hangRecovery = 6f;

	[Tooltip("The amount of force needed to break this thing")]
	public float breakForce = 50000f;

	private float hangHealth = 1f;

	private float standHealth = 1f;

	private bool moved;

	public NodeOutput leftJointBroken;

	public NodeOutput rightJointBroken;

	private Vector3 axis;

	private ConfigurableJoint leftJoint;

	private ConfigurableJoint rightJoint;

	private Vector3 startPos;

	private Quaternion startRot;

	public bool showDebug;

	private int brokenCount;

	private new void OnEnable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Ran the Enable stuff ");
		}
		startPos = base.transform.position;
		startRot = base.transform.rotation;
		Vector3 size = GetComponent<BoxCollider>().size;
		if (size.x > size.y && size.x > size.z)
		{
			axis = Vector3.right * size.x;
		}
		else if (size.y > size.z)
		{
			axis = Vector3.up * size.y;
		}
		else
		{
			axis = Vector3.forward * size.z;
		}
		leftJoint = AddJoint(-axis / 3f);
		rightJoint = AddJoint(axis / 3f);
	}

	private ConfigurableJoint AddJoint(Vector3 pos)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Adding joints ");
		}
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.anchor = pos;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.linearLimit = new SoftJointLimit
		{
			limit = 0.03f
		};
		configurableJoint.yDrive = new JointDrive
		{
			positionSpring = 2000f,
			maximumForce = 5000f,
			positionDamper = 100f
		};
		configurableJoint.targetPosition = -Vector3.up / 2f;
		configurableJoint.breakForce = breakForce;
		return configurableJoint;
	}

	public void FixedUpdate()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient || leftJoint == null || rightJoint == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.grabManager.IsGrabbed(base.gameObject))
			{
				flag = true;
			}
			else if (human.groundManager.IsStanding(base.gameObject))
			{
				flag2 = true;
				float num = human.ragdoll.partBall.transform.position.y - base.transform.position.y;
				if (num > -0.3f && num < 0.2f)
				{
					return;
				}
			}
		}
		if (flag2)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " We are standing ");
			}
			standHealth -= Time.fixedDeltaTime / standTime;
		}
		else
		{
			standHealth += Time.fixedDeltaTime / standRecovery;
		}
		if (flag)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " We have been grabbed ");
			}
			hangHealth -= Time.fixedDeltaTime / hangTime;
		}
		else
		{
			hangHealth += Time.fixedDeltaTime / hangRecovery;
		}
		standHealth = Mathf.Clamp01(standHealth);
		hangHealth = Mathf.Clamp01(hangHealth);
		if (!moved && (standHealth < 0.5f || hangHealth < 0.5f))
		{
			moved = true;
			if (base.transform.TransformPoint(leftJoint.anchor).y < base.transform.TransformPoint(rightJoint.anchor).y)
			{
				leftJoint.linearLimit = new SoftJointLimit
				{
					limit = 0.2f
				};
				leftJoint.yDrive = new JointDrive
				{
					positionSpring = 1000f,
					maximumForce = 0f,
					positionDamper = 100f
				};
			}
			else
			{
				rightJoint.linearLimit = new SoftJointLimit
				{
					limit = 0.1f
				};
				rightJoint.yDrive = new JointDrive
				{
					positionSpring = 1000f,
					maximumForce = 0f,
					positionDamper = 100f
				};
			}
		}
		else if (standHealth == 0f || hangHealth == 0f)
		{
			Object.Destroy(leftJoint);
			Object.Destroy(rightJoint);
		}
	}

	public void OnJointBreak(float breakForce)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Joint Break ");
		}
		if (brokenCount == 0)
		{
			int num = Random.Range(0, 1);
			if (num == 1)
			{
				Object.Destroy(leftJoint);
				LeftJointSignal();
			}
			else
			{
				Object.Destroy(rightJoint);
				RightJointSignal();
			}
		}
		else if (brokenCount == 1)
		{
			if (leftJoint != null)
			{
				Object.Destroy(leftJoint);
				LeftJointSignal();
			}
			if (rightJoint != null)
			{
				Object.Destroy(rightJoint);
				RightJointSignal();
			}
		}
		brokenCount++;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Reset State ");
		}
		hangHealth = (standHealth = 1f);
		Rigidbody component = GetComponent<Rigidbody>();
		Vector3 position = startPos;
		base.transform.position = position;
		component.position = position;
		Quaternion rotation = startRot;
		base.transform.rotation = rotation;
		component.rotation = rotation;
		position = (component.angularVelocity = Vector3.zero);
		component.velocity = position;
		if (leftJoint != null)
		{
			Object.Destroy(leftJoint);
		}
		if (rightJoint != null)
		{
			Object.Destroy(rightJoint);
		}
		leftJoint = AddJoint(-axis / 3f);
		rightJoint = AddJoint(axis / 3f);
	}

	private void ApplyState(bool left, bool right)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Apply bool state ");
		}
		if (!left && leftJoint != null)
		{
			Object.Destroy(leftJoint);
		}
		if (!right && rightJoint != null)
		{
			Object.Destroy(rightJoint);
		}
		if (left && leftJoint == null)
		{
			leftJoint = AddJoint(-axis / 3f);
		}
		if (right && rightJoint == null)
		{
			rightJoint = AddJoint(axis / 3f);
		}
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void CollectState(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Collected State ");
		}
		NetBoolEncoder.CollectState(stream, leftJoint != null);
		NetBoolEncoder.CollectState(stream, rightJoint != null);
	}

	public void ApplyState(NetStream state)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Applying State ");
		}
		ApplyState(NetBoolEncoder.ApplyState(state), NetBoolEncoder.ApplyState(state));
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Apply Lerped State ");
		}
		ApplyState(NetBoolEncoder.ApplyLerpedState(state0, state1, mix), NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Calc the Delta ");
		}
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Add the delta  ");
		}
		NetBoolEncoder.AddDelta(state0, delta, result);
		NetBoolEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Calc max delta size in bits ");
		}
		return 2 * NetBoolEncoder.CalculateMaxDeltaSizeInBits();
	}

	private void LeftJointSignal()
	{
		leftJointBroken.SetValue(1f);
	}

	private void RightJointSignal()
	{
		rightJointBroken.SetValue(1f);
	}
}
