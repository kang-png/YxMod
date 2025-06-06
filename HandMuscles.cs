using System;
using HumanAPI;
using UnityEngine;

[Serializable]
public class HandMuscles
{
	public enum TargetingMode
	{
		Shoulder,
		Chest,
		Hips,
		Ball
	}

	private class ScanMem
	{
		public Vector3 pos;

		public Vector3 shoulder;

		public Vector3 hand;

		public float grabTime;

		public float grabAngle;
	}

	public float spring = 10f;

	public float extraUpSpring;

	public float damper;

	public float squareDamper = 0.2f;

	public float maxHorizontalForce = 250f;

	public float maxVertialForce = 500f;

	public float grabSpring = 10f;

	public float grabExtraUpSpring;

	public float grabDamper;

	public float grabSquareDamper = 0.2f;

	public float grabMaxHorizontalForce = 250f;

	public float grabMaxVertialForce = 500f;

	public float maxLiftForce = 500f;

	public float maxPushForce = 200f;

	public float liftDampSqr = 0.1f;

	public float liftDamp = 0.1f;

	private readonly Human human;

	private readonly Ragdoll ragdoll;

	private readonly HumanMotion2 motion;

	public TargetingMode targetingMode;

	public TargetingMode grabTargetingMode = TargetingMode.Ball;

	private ScanMem leftMem = new ScanMem();

	private ScanMem rightMem = new ScanMem();

	public float forwardMultiplier = 10f;

	public float armMass = 20f;

	public float bodyMass = 50f;

	public float maxForce = 300f;

	public float grabMaxForce = 450f;

	public float climbMaxForce = 800f;

	public float gravityForce = 100f;

	public float anisotrophy = 1f;

	public float maxStopForce = 150f;

	public float grabMaxStopForce = 500f;

	public float maxSpeed = 100f;

	public float onAxisAnisotrophy;

	public float offAxisAnisotrophy;

	public const float grabSnap = 0.3f;

	public const float targetHelperSnap = 0.3f;

	public const float targetHelperPull = 0.5f;

	public const float targetSnap = 0.2f;

	public const float targetPull = 0.5f;

	public const float regularSnap = 0.1f;

	public const float regularPull = 0.2f;

	private Collider[] colliders = new Collider[20];

	public HandMuscles(Human human, Ragdoll ragdoll, HumanMotion2 motion)
	{
		this.human = human;
		this.ragdoll = ragdoll;
		this.motion = motion;
	}

	public void OnFixedUpdate()
	{
		float targetPitchAngle = human.controls.targetPitchAngle;
		float targetYawAngle = human.controls.targetYawAngle;
		float leftExtend = human.controls.leftExtend;
		float rightExtend = human.controls.rightExtend;
		bool grab = human.controls.leftGrab;
		bool grab2 = human.controls.rightGrab;
		bool onGround = human.onGround;
		if ((ragdoll.partLeftHand.transform.position - ragdoll.partChest.transform.position).sqrMagnitude > 6f)
		{
			grab = false;
		}
		if ((ragdoll.partRightHand.transform.position - ragdoll.partChest.transform.position).sqrMagnitude > 6f)
		{
			grab2 = false;
		}
		Quaternion quaternion = Quaternion.Euler(targetPitchAngle, targetYawAngle, 0f);
		Quaternion quaternion2 = Quaternion.Euler(0f, targetYawAngle, 0f);
		Vector3 worldPos = Vector3.zero;
		Vector3 worldPos2 = Vector3.zero;
		float num = 0f;
		float z = 0f;
		if (targetPitchAngle > 0f && onGround)
		{
			z = 0.4f * targetPitchAngle / 90f;
		}
		TargetingMode targetingMode = ((!(ragdoll.partLeftHand.sensor.grabJoint != null)) ? this.targetingMode : grabTargetingMode);
		TargetingMode targetingMode2 = ((!(ragdoll.partRightHand.sensor.grabJoint != null)) ? this.targetingMode : grabTargetingMode);
		switch (targetingMode)
		{
		case TargetingMode.Shoulder:
			worldPos = ragdoll.partLeftArm.transform.position + quaternion * new Vector3(0f, 0f, leftExtend * ragdoll.handLength);
			break;
		case TargetingMode.Chest:
			worldPos = ragdoll.partChest.transform.position + quaternion2 * new Vector3(-0.2f, 0.15f, 0f) + quaternion * new Vector3(0f, 0f, leftExtend * ragdoll.handLength);
			break;
		case TargetingMode.Hips:
			if (targetPitchAngle > 0f)
			{
				num = -0.3f * targetPitchAngle / 90f;
			}
			worldPos = ragdoll.partHips.transform.position + quaternion2 * new Vector3(-0.2f, 0.65f + num, z) + quaternion * new Vector3(0f, 0f, leftExtend * ragdoll.handLength);
			break;
		case TargetingMode.Ball:
			if (targetPitchAngle > 0f)
			{
				num = -0.2f * targetPitchAngle / 90f;
			}
			if (ragdoll.partLeftHand.sensor.grabJoint != null)
			{
				z = ((!human.isClimbing) ? 0f : (-0.2f));
			}
			worldPos = ragdoll.partBall.transform.position + quaternion2 * new Vector3(-0.2f, 0.7f + num, z) + quaternion * new Vector3(0f, 0f, leftExtend * ragdoll.handLength);
			break;
		}
		switch (targetingMode2)
		{
		case TargetingMode.Shoulder:
			worldPos2 = ragdoll.partRightArm.transform.position + quaternion * new Vector3(0f, 0f, rightExtend * ragdoll.handLength);
			break;
		case TargetingMode.Chest:
			worldPos2 = ragdoll.partChest.transform.position + quaternion2 * new Vector3(0.2f, 0.15f, 0f) + quaternion * new Vector3(0f, 0f, rightExtend * ragdoll.handLength);
			break;
		case TargetingMode.Hips:
			if (targetPitchAngle > 0f)
			{
				num = -0.3f * targetPitchAngle / 90f;
			}
			worldPos2 = ragdoll.partHips.transform.position + quaternion2 * new Vector3(0.2f, 0.65f + num, z) + quaternion * new Vector3(0f, 0f, rightExtend * ragdoll.handLength);
			break;
		case TargetingMode.Ball:
			if (targetPitchAngle > 0f)
			{
				num = -0.2f * targetPitchAngle / 90f;
			}
			if (ragdoll.partRightHand.sensor.grabJoint != null)
			{
				z = ((!human.isClimbing) ? 0f : (-0.2f));
			}
			worldPos2 = ragdoll.partBall.transform.position + quaternion2 * new Vector3(0.2f, 0.7f + num, z) + quaternion * new Vector3(0f, 0f, rightExtend * ragdoll.handLength);
			break;
		}
		ProcessHand(leftMem, ragdoll.partLeftArm, ragdoll.partLeftForearm, ragdoll.partLeftHand, worldPos, leftExtend, grab, motion.legs.legPhase + 0.5f, right: false);
		ProcessHand(rightMem, ragdoll.partRightArm, ragdoll.partRightForearm, ragdoll.partRightHand, worldPos2, rightExtend, grab2, motion.legs.legPhase, right: true);
	}

	private void ProcessHand(ScanMem mem, HumanSegment arm, HumanSegment forearm, HumanSegment hand, Vector3 worldPos, float extend, bool grab, float animationPhase, bool right)
	{
		double num = 0.1 + (double)(0.14f * Mathf.Abs(human.controls.targetPitchAngle - mem.grabAngle) / 80f);
		double num2 = num * 2.0;
		if (CheatCodes.climbCheat)
		{
			num2 = (num /= 4.0);
		}
		if (grab && !hand.sensor.grab)
		{
			if ((double)mem.grabTime > num)
			{
				mem.pos = arm.transform.position;
			}
			else
			{
				grab = false;
			}
		}
		if (hand.sensor.grab && !grab)
		{
			mem.grabTime = 0f;
			mem.grabAngle = human.controls.targetPitchAngle;
		}
		else
		{
			mem.grabTime += Time.fixedDeltaTime;
		}
		hand.sensor.grab = (double)mem.grabTime > num2 && grab;
		if (extend > 0.2f)
		{
			bool flag = false;
			hand.sensor.targetPosition = worldPos;
			Vector3 vector = worldPos;
			mem.shoulder = arm.transform.position;
			mem.hand = hand.transform.position;
			if (hand.sensor.grabJoint == null)
			{
				worldPos = FindTarget(mem, worldPos, out hand.sensor.grabFilter);
			}
			PlaceHand(arm, hand, worldPos, active: true, hand.sensor.grabJoint != null, hand.sensor.grabBody);
			if (hand.sensor.grabBody != null)
			{
				LiftBody(hand, hand.sensor.grabBody);
			}
			hand.sensor.grabPosition = worldPos;
		}
		else
		{
			hand.sensor.grabFilter = null;
			if (human.state == HumanState.Walk)
			{
				AnimateHand(arm, forearm, hand, animationPhase, 1f, right);
			}
			else if (human.state == HumanState.FreeFall)
			{
				Vector3 targetDirection = human.targetDirection;
				targetDirection.y = 0f;
				HumanMotion2.AlignToVector(arm, arm.transform.up, -targetDirection, 2f);
				HumanMotion2.AlignToVector(forearm, forearm.transform.up, targetDirection, 2f);
			}
			else
			{
				Vector3 targetDirection2 = human.targetDirection;
				targetDirection2.y = 0f;
				HumanMotion2.AlignToVector(arm, arm.transform.up, -targetDirection2, 20f);
				HumanMotion2.AlignToVector(forearm, forearm.transform.up, targetDirection2, 20f);
			}
		}
	}

	private void AnimateHand(HumanSegment arm, HumanSegment forearm, HumanSegment hand, float phase, float tonus, bool right)
	{
		tonus *= 50f * human.controls.walkSpeed;
		phase -= Mathf.Floor(phase);
		Vector3 vector = Quaternion.Euler(0f, human.controls.targetYawAngle, 0f) * Vector3.forward;
		Vector3 vector2 = Quaternion.Euler(0f, human.controls.targetYawAngle, 0f) * Vector3.right;
		if (!right)
		{
			vector2 = -vector2;
		}
		if (phase < 0.5f)
		{
			HumanMotion2.AlignToVector(arm, arm.transform.up, Vector3.down + vector2 / 2f, 3f * tonus);
			HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector / 2f - vector2, 3f * tonus);
		}
		else
		{
			HumanMotion2.AlignToVector(arm, arm.transform.up, -vector + vector2 / 2f, 3f * tonus);
			HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector + Vector3.down, 3f * tonus);
		}
	}

	private void PlaceHand(HumanSegment arm, HumanSegment hand, Vector3 worldPos, bool active, bool grabbed, Rigidbody grabbedBody)
	{
		if (!active)
		{
			return;
		}
		Rigidbody rigidbody = hand.rigidbody;
		Vector3 worldCenterOfMass = rigidbody.worldCenterOfMass;
		Vector3 offset = worldPos - worldCenterOfMass;
		Vector3 vector = new Vector3(0f, offset.y, 0f);
		Vector3 velocity = rigidbody.velocity - ragdoll.partBall.rigidbody.velocity;
		float num = armMass;
		float num2 = maxForce;
		if (grabbed)
		{
			if (grabbedBody != null)
			{
				num += Mathf.Clamp(grabbedBody.mass / 2f, 0f, bodyMass);
				num2 = Mathf.Lerp(grabMaxForce, climbMaxForce, (human.controls.targetPitchAngle - 50f) / 30f);
			}
			else
			{
				num += bodyMass;
				num2 = Mathf.Lerp(grabMaxForce, climbMaxForce, (human.controls.targetPitchAngle - 50f) / 30f);
			}
		}
		float maxAcceleration = num2 / num;
		Vector3 vector2 = ConstantAccelerationControl.Solve(offset, velocity, maxAcceleration, 0.1f);
		int num3 = 600;
		Vector3 vector3 = vector2 * num + Vector3.up * gravityForce;
		if (human.grabbedByHuman != null && human.grabbedByHuman.state == HumanState.Climb)
		{
			vector3 *= 1.7f;
			num3 *= 2;
		}
		if (!grabbed)
		{
			rigidbody.SafeAddForce(vector3);
			ragdoll.partHips.rigidbody.SafeAddForce(-vector3);
			return;
		}
		Vector3 normalized = human.targetDirection.ZeroY().normalized;
		Vector3 vector4 = Mathf.Min(0f, Vector3.Dot(normalized, vector3)) * normalized;
		Vector3 vector5 = vector3 - vector4;
		Vector3 vector6 = vector3.SetX(0f).SetZ(0f);
		Vector3 b = -vector3 * 0.25f;
		Vector3 b2 = -vector3 * 0.75f;
		Vector3 a = -vector3 * 0.1f - vector6 * 0.5f - vector5 * 0.25f;
		Vector3 a2 = -vector6 * 0.2f - vector5 * 0.4f;
		if (grabbedBody != null)
		{
			Carryable component = grabbedBody.GetComponent<Carryable>();
			if (component != null)
			{
				b *= component.handForceMultiplier;
				b2 *= component.handForceMultiplier;
			}
		}
		float num4 = ((human.state != HumanState.Climb) ? 1f : Mathf.Clamp01((human.controls.targetPitchAngle - 10f) / 60f));
		Vector3 vector7 = Vector3.Lerp(a, b, offset.y + 0.5f) * num4;
		Vector3 vector8 = Vector3.Lerp(a2, b2, offset.y + 0.5f) * num4;
		float num5 = Mathf.Abs(vector7.y + vector8.y);
		if (num5 > (float)num3)
		{
			vector7 *= (float)num3 / num5;
			vector8 *= (float)num3 / num5;
		}
		ragdoll.partChest.rigidbody.SafeAddForce(vector7);
		ragdoll.partBall.rigidbody.SafeAddForce(vector8);
		rigidbody.SafeAddForce(-vector7 - vector8);
	}

	private void LiftBody(HumanSegment hand, Rigidbody body)
	{
		if (human.GetComponent<GroundManager>().IsStanding(body.gameObject) || body.tag == "NoLift")
		{
			return;
		}
		float num = 0.5f + 0.5f * Mathf.InverseLerp(0f, 100f, body.mass);
		Vector3 vector = (human.targetLiftDirection.ZeroY() * maxPushForce).SetY(Mathf.Max(0f, human.targetLiftDirection.y) * maxLiftForce);
		float magnitude = (hand.transform.position - body.worldCenterOfMass).magnitude;
		float num2 = num;
		float num3 = 1f;
		float num4 = 1f;
		Carryable component = body.GetComponent<Carryable>();
		if (component != null)
		{
			num2 *= component.liftForceMultiplier;
			num3 = component.forceHalfDistance;
			num4 = component.damping;
			if (num3 <= 0f)
			{
				throw new InvalidOperationException("halfdistance cant be 0 or less!");
			}
		}
		float num5 = num3 / (num3 + magnitude);
		vector *= num2;
		vector *= num5;
		body.SafeAddForce(vector);
		hand.rigidbody.SafeAddForce(-vector * 0.5f);
		ragdoll.partChest.rigidbody.SafeAddForce(-vector * 0.5f);
		body.SafeAddTorque(-body.angularVelocity * liftDamp * num4, ForceMode.Acceleration);
		body.SafeAddTorque(-body.angularVelocity.normalized * body.angularVelocity.sqrMagnitude * liftDampSqr * num4, ForceMode.Acceleration);
		if (!(component != null) || component.aiming == CarryableAiming.None)
		{
			return;
		}
		Vector3 vector2 = human.targetLiftDirection;
		if (component.limitAlignToHorizontal)
		{
			vector2.y = 0f;
			vector2.Normalize();
		}
		Vector3 vector3 = ((component.aiming != CarryableAiming.ForwardAxis) ? (body.worldCenterOfMass - hand.transform.position).normalized : body.transform.forward);
		float aimSpring = component.aimSpring;
		float num6 = ((!(component.aimTorque < float.PositiveInfinity)) ? aimSpring : component.aimTorque);
		if (!component.alwaysForward)
		{
			float num7 = Vector3.Dot(vector3, vector2);
			if (num7 < 0f)
			{
				vector2 = -vector2;
				num7 = 0f - num7;
			}
			num6 *= Mathf.Pow(num7, component.aimAnglePower);
		}
		else
		{
			float num8 = Vector3.Dot(vector3, vector2);
			num8 = 0.5f + num8 / 2f;
			num6 *= Mathf.Pow(num8, component.aimAnglePower);
		}
		if (component.aimDistPower != 0f)
		{
			num6 *= Mathf.Pow((body.worldCenterOfMass - hand.transform.position).magnitude, component.aimDistPower);
		}
		HumanMotion2.AlignToVector(body, vector3, vector2, aimSpring, num6);
	}

	private Vector3 FindTarget(ScanMem mem, Vector3 worldPos, out Collider targetCollider)
	{
		targetCollider = null;
		Vector3 vector = worldPos - mem.shoulder;
		Ray ray = new Ray(mem.shoulder, vector.normalized);
		int num = Physics.OverlapCapsuleNonAlloc(ray.origin, worldPos, 0.5f, colliders, motion.grabLayers, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num; i++)
		{
			Collider collider = colliders[i];
			TargetHelper componentInChildren = collider.GetComponentInChildren<TargetHelper>();
			if (componentInChildren != null)
			{
				Vector3 vector2 = componentInChildren.transform.position - worldPos;
				float magnitude = (Math3d.ProjectPointOnLineSegment(ray.origin, worldPos, componentInChildren.transform.position) - componentInChildren.transform.position).magnitude;
				if (magnitude < 0.3f && (componentInChildren.transform.position - mem.hand).magnitude < 0.3f)
				{
					worldPos = componentInChildren.transform.position;
					targetCollider = collider;
				}
				else
				{
					worldPos += vector2 * Mathf.InverseLerp(0.5f, 0.3f, magnitude);
				}
				return worldPos;
			}
		}
		Vector3 vector3 = mem.hand + Vector3.ClampMagnitude(worldPos - mem.hand, 0.3f);
		targetCollider = null;
		Vector3 vector4 = vector3 - mem.pos;
		Ray ray2 = new Ray(mem.pos, vector4.normalized);
		Debug.DrawRay(ray2.origin, ray2.direction * vector4.magnitude, Color.yellow, 0.2f);
		float num2 = float.PositiveInfinity;
		Vector3 vector5 = vector3;
		for (float num3 = 0.05f; num3 <= 0.5f; num3 += 0.05f)
		{
			if (!Physics.SphereCast(ray2, num3, out var hitInfo, vector4.magnitude, motion.grabLayers, QueryTriggerInteraction.Ignore))
			{
				continue;
			}
			float magnitude2 = (vector3 - hitInfo.point).magnitude;
			magnitude2 += num3 / 10f;
			if (hitInfo.collider.tag == "Target")
			{
				magnitude2 /= 100f;
			}
			else
			{
				if (num3 > 0.2f)
				{
					continue;
				}
				Vector3 normalized = (worldPos - mem.shoulder).normalized;
				Vector3 normalized2 = (hitInfo.point - mem.shoulder).normalized;
				if (Vector3.Dot(normalized, normalized2) < 0.7f)
				{
					continue;
				}
			}
			if (magnitude2 < num2)
			{
				num2 = magnitude2;
				vector5 = hitInfo.point;
				targetCollider = hitInfo.collider;
			}
		}
		if (targetCollider != null)
		{
			Vector3 vector6 = vector5 - vector3;
			float magnitude3 = (Math3d.ProjectPointOnLineSegment(ray2.origin, vector3, vector5) - vector5).magnitude;
			if (targetCollider.tag == "Target")
			{
				if (magnitude3 < 0.2f && (mem.hand - vector5).magnitude < 0.5f)
				{
					worldPos = vector5;
				}
				else
				{
					worldPos = vector3 + vector6 * Mathf.InverseLerp(0.5f, 0.2f, magnitude3);
					targetCollider = null;
				}
			}
			else if (magnitude3 < 0.1f && vector6.magnitude < 0.1f)
			{
				worldPos = vector5;
			}
			else
			{
				worldPos = vector3 + vector6 * Mathf.InverseLerp(0.2f, 0.1f, magnitude3);
				targetCollider = null;
			}
		}
		mem.pos = vector3;
		return worldPos;
	}
}
