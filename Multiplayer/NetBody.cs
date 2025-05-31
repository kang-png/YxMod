using System;
using System.Collections.Generic;
using HumanAPI;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer;

[RequireComponent(typeof(NetIdentity))]
public class NetBody : MonoBehaviour, INetBehavior, IRespawnable
{
	public Transform relativeTo;

	public NetBodySyncPosition syncPosition = NetBodySyncPosition.World;

	public NetBodySyncRotation syncRotation = NetBodySyncRotation.World;

	public float posRangeOverride;

	public int posPrecision;

	public int rotPrecision;

	public bool smooth;

	public bool respawn;

	public float despawnHeight = -50f;

	public float respawnHeight = 5f;

	public bool ignoreRespawnOffset;

	[Tooltip("Forces the cachedPos and cachedRot values on the current Level associated with this body to be cleared when this is reset")]
	public bool isClearingRigidCacheOnReset;

	public bool isKinematic;

	public bool manageActive = true;

	public UnityEvent m_respawnEvent;

	[NonSerialized]
	public float posRange = 500f;

	[NonSerialized]
	public ushort possmall = 4;

	[NonSerialized]
	public ushort poslarge = 8;

	[NonSerialized]
	public ushort posfull = 18;

	[NonSerialized]
	public ushort rotsmall = 4;

	[NonSerialized]
	public ushort rotlarge = 6;

	[NonSerialized]
	public ushort rotfull = 9;

	private NetFloat eulerEncoder = new NetFloat(180f, 12, 4, 8);

	private Rigidbody body;

	public bool syncLocalScale;

	private Vector3 startLocalScale;

	[NonSerialized]
	public Vector3 startPos;

	[NonSerialized]
	public Quaternion startRot;

	private Quaternion baseRot;

	private Quaternion baseRotInv;

	private Vector3 basePos;

	[NonSerialized]
	public NetQuaternion collectedRot;

	[NonSerialized]
	public NetQuaternion appliedRot;

	[NonSerialized]
	public float appliedEuler;

	[NonSerialized]
	public float collectedEuler;

	private NetVector3 zero;

	private NetQuaternion identity;

	private NetBodySleep sleep;

	[NonSerialized]
	public bool disableSleep;

	private bool isStarted;

	private bool hasNetSetActive;

	private int mResetFramesAdditionalDelay;

	[Header("Additional Rotation Smoothening")]
	[Tooltip("Introduces slight inaccuracy on the client side when syncing rotation to soften the jittering. This should only be used in the cases where the rotation of an object changes quickly and irregularly.")]
	public bool smoothenClientRotationLocally;

	[Tooltip("Defines how closely the approximation should follow the real movement. Lower values will result in smoother but less accurate movement.")]
	public float scrlTimeFactor = 15f;

	private Quaternion? scrlLerpTarget;

	private const float ScrlLerpThreshold = 0.1f;

	private const float ScrlLerpCeiling = 5f;

	private int mJustReset;

	private Vector3 mResetPosition;

	private Quaternion mResetRotation;

	private const int kResetFramesToReset = 5;

	private string[] mBadObjectNames = new string[2] { "MotorBoat", "BigBoat" };

	private SmoothQuaternion smoothRot;

	private SmoothVector3 smoothPos;

	[NonSerialized]
	public bool isVisible = true;

	private List<RespawningBodyOverride> overrides;

	public bool IsKinematic
	{
		get
		{
			return isKinematic;
		}
		set
		{
			isKinematic = value;
		}
	}

	public Rigidbody RigidBody => body;

	public bool HandlingReset { get; private set; }

	public bool ResetLastFrame { get; private set; }

	public void Start()
	{
		if (!isStarted)
		{
			isStarted = true;
			if (NetGame.isClient)
			{
				hasNetSetActive = GetComponent<NetSetActive>() != null;
			}
			NetBodyResetOverride component = GetComponent<NetBodyResetOverride>();
			if ((bool)component)
			{
				mResetFramesAdditionalDelay = component.FramesDelay;
			}
			body = GetComponent<Rigidbody>();
			isKinematic = body.isKinematic;
			if (syncPosition == NetBodySyncPosition.Relative)
			{
				posRange = 10f;
				posfull -= 6;
			}
			if (syncPosition == NetBodySyncPosition.Local)
			{
				posRange = 10f;
				posfull -= 6;
			}
			if (posRangeOverride != 0f)
			{
				posRange = posRangeOverride;
			}
			eulerEncoder.fullBits = (ushort)(eulerEncoder.fullBits + rotPrecision);
			eulerEncoder.deltaSmall = (ushort)(eulerEncoder.deltaSmall + rotPrecision);
			eulerEncoder.deltaLarge = (ushort)(eulerEncoder.deltaLarge + rotPrecision);
			possmall = (ushort)(possmall + posPrecision);
			poslarge = (ushort)(poslarge + posPrecision);
			posfull = (ushort)(posfull + posPrecision);
			rotfull = (ushort)(rotfull + rotPrecision);
			rotsmall = (ushort)(rotsmall + rotPrecision);
			rotlarge = (ushort)(rotlarge + rotPrecision);
			startPos = base.transform.position;
			startRot = base.transform.rotation;
			startLocalScale = base.transform.localScale;
			if (syncPosition == NetBodySyncPosition.Relative)
			{
				basePos = base.transform.position - relativeTo.position;
			}
			else if (syncPosition == NetBodySyncPosition.Absolute)
			{
				basePos = Vector3.zero;
			}
			else
			{
				basePos = ((syncPosition != NetBodySyncPosition.Local) ? base.transform.position : base.transform.localPosition);
			}
			if (syncRotation == NetBodySyncRotation.Relative)
			{
				baseRot = Quaternion.Inverse(relativeTo.rotation) * base.transform.rotation;
			}
			else if (syncRotation == NetBodySyncRotation.Absolute)
			{
				baseRot = Quaternion.identity;
			}
			else
			{
				baseRot = ((syncRotation != NetBodySyncRotation.Local && syncRotation != NetBodySyncRotation.EulerX && syncRotation != NetBodySyncRotation.EulerY && syncRotation != NetBodySyncRotation.EulerZ) ? base.transform.rotation : base.transform.localRotation);
			}
			baseRotInv = Quaternion.Inverse(baseRot);
			zero = NetVector3.Quantize(Vector3.zero, posRange, posfull);
			identity = NetQuaternion.Quantize(Quaternion.identity, rotfull);
			if (!disableSleep)
			{
				sleep = new NetBodySleep(body);
			}
		}
	}

	private void Update()
	{
		ApplyQuaternionStateDelayed();
	}

	public void StartNetwork(NetIdentity identity)
	{
		if (!isStarted)
		{
			Start();
		}
	}

	public void SetMaster(bool master)
	{
		if (!isStarted)
		{
			Start();
		}
		body.isKinematic = !master || isKinematic;
	}

	private void FixedUpdate()
	{
		if (!isStarted)
		{
			Start();
		}
		HandleRespawn();
		if (ResetLastFrame)
		{
			ResetLastFrame = false;
		}
		if (mJustReset > 0)
		{
			mJustReset--;
			if (mJustReset >= 5 - (1 + mResetFramesAdditionalDelay))
			{
				body.isKinematic = true;
				body.position = mResetPosition;
				body.rotation = mResetRotation;
				if (isClearingRigidCacheOnReset && Game.currentLevel != null)
				{
					Game.currentLevel.ForceRecacheRigid(body);
				}
				body.angularVelocity = Vector3.zero;
				body.velocity = Vector3.zero;
			}
			if (mJustReset == 0)
			{
				HandlingReset = false;
				ResetLastFrame = true;
				if (!isKinematic)
				{
					body.isKinematic = false;
					UpdateVisibility();
				}
			}
		}
		if (sleep != null)
		{
			sleep.HandleSleep();
		}
	}

	public void CollectState(NetStream stream)
	{
		if (syncPosition == NetBodySyncPosition.Relative)
		{
			NetVector3.Quantize(base.transform.position - relativeTo.position - basePos, posRange, posfull).Write(stream);
		}
		else if (syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World)
		{
			NetVector3.Quantize(((syncPosition != NetBodySyncPosition.Local) ? base.transform.position : base.transform.localPosition) - basePos, posRange, posfull).Write(stream);
		}
		if (syncRotation == NetBodySyncRotation.Relative)
		{
			NetQuaternion netQuaternion = (collectedRot = NetQuaternion.Quantize(baseRotInv * Quaternion.Inverse(relativeTo.rotation) * base.transform.rotation, rotfull));
			netQuaternion.Write(stream);
		}
		else if (syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
		{
			NetQuaternion netQuaternion2 = (collectedRot = NetQuaternion.Quantize(baseRotInv * ((syncRotation != NetBodySyncRotation.Local) ? base.transform.rotation : base.transform.localRotation), rotfull));
			netQuaternion2.Write(stream);
		}
		else if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			float value = (collectedEuler = syncRotation switch
			{
				NetBodySyncRotation.EulerX => 0f - Math3d.SignedVectorAngle(baseRotInv * base.transform.localRotation * Vector3.up, Vector3.up, Vector3.right), 
				NetBodySyncRotation.EulerY => 0f - Math3d.SignedVectorAngle(baseRotInv * base.transform.localRotation * Vector3.forward, Vector3.forward, Vector3.up), 
				NetBodySyncRotation.EulerZ => 0f - Math3d.SignedVectorAngle(baseRotInv * base.transform.localRotation * Vector3.right, Vector3.right, Vector3.forward), 
				_ => throw new InvalidOperationException(), 
			});
			eulerEncoder.CollectState(stream, value);
		}
		if (syncLocalScale)
		{
			NetVector3.Quantize(base.transform.localScale, posRange, posfull).Write(stream);
		}
	}

	public void ApplyState(NetStream state)
	{
		if (syncPosition == NetBodySyncPosition.Relative || syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World)
		{
			Vector3 target = NetVector3.Read(state, posfull).Dequantize(posRange);
			ApplyPositionState(target);
		}
		if (syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
		{
			NetQuaternion netQuaternion = (appliedRot = NetQuaternion.Read(state, rotfull));
			Quaternion target2 = netQuaternion.Dequantize();
			ApplyQuaternionState(target2);
		}
		else if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			float diff = eulerEncoder.ApplyState(state);
			ApplyEulerState(diff);
		}
		if (body != null)
		{
			Rigidbody rigidbody = body;
			Vector3 vector = Vector3.zero;
			body.velocity = vector;
			rigidbody.angularVelocity = vector;
		}
		if (syncLocalScale)
		{
			Vector3 localScale = NetVector3.Read(state, posfull).Dequantize(posRange);
			base.transform.localScale = localScale;
		}
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		if (syncPosition == NetBodySyncPosition.Relative || syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World)
		{
			Vector3 vector = NetVector3.Read(state0, posfull).Dequantize(posRange);
			Vector3 vector2 = NetVector3.Read(state1, posfull).Dequantize(posRange);
			if ((vector - vector2).sqrMagnitude > 15f)
			{
				vector = vector2;
			}
			Vector3 target = Vector3.Lerp(vector, vector2, mix);
			ApplyPositionState(target);
		}
		if (syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
		{
			Quaternion a = NetQuaternion.Read(state0, rotfull).Dequantize();
			NetQuaternion netQuaternion = (appliedRot = NetQuaternion.Read(state1, rotfull));
			Quaternion b = netQuaternion.Dequantize();
			Quaternion target2 = Quaternion.Slerp(a, b, mix);
			ApplyQuaternionState(target2);
		}
		else if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			float diff = eulerEncoder.ApplyLerpedState(state0, state1, mix);
			ApplyEulerState(diff);
		}
		if (body != null)
		{
			Rigidbody rigidbody = body;
			Vector3 vector3 = Vector3.zero;
			body.velocity = vector3;
			rigidbody.angularVelocity = vector3;
		}
		if (syncLocalScale && (syncPosition == NetBodySyncPosition.Relative || syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World))
		{
			Vector3 a2 = NetVector3.Read(state0, posfull).Dequantize(posRange);
			Vector3 b2 = NetVector3.Read(state1, posfull).Dequantize(posRange);
			Vector3 localScale = Vector3.Lerp(a2, b2, mix);
			base.transform.localScale = localScale;
		}
	}

	private void ApplyPositionState(Vector3 target)
	{
		target += basePos;
		if (smooth)
		{
			if (smoothPos == null)
			{
				smoothPos = new SmoothVector3();
			}
			target = smoothPos.Next(target);
		}
		if (syncPosition == NetBodySyncPosition.Relative)
		{
			target += relativeTo.position;
			if (base.transform.position != target)
			{
				base.transform.position = target;
			}
		}
		else if (syncPosition == NetBodySyncPosition.Local)
		{
			if (base.transform.localPosition != target)
			{
				base.transform.localPosition = target;
			}
		}
		else if (base.transform.position != target)
		{
			base.transform.position = target;
		}
		if (!hasNetSetActive)
		{
			UpdateVisibility();
		}
	}

	private void ApplyQuaternionState(Quaternion target)
	{
		target = baseRot * target;
		if (smooth)
		{
			if (smoothRot == null)
			{
				smoothRot = new SmoothQuaternion();
			}
			target = smoothRot.Next(target);
		}
		if (syncRotation == NetBodySyncRotation.Relative)
		{
			target = relativeTo.rotation * target;
			if (base.transform.rotation.eulerAngles != target.eulerAngles)
			{
				base.transform.rotation = target;
			}
		}
		else if (syncRotation == NetBodySyncRotation.Local)
		{
			if (base.transform.localRotation.eulerAngles != target.eulerAngles)
			{
				base.transform.localRotation = target;
			}
		}
		else if (smoothenClientRotationLocally)
		{
			scrlLerpTarget = target;
		}
		else if (base.transform.rotation.eulerAngles != target.eulerAngles)
		{
			base.transform.rotation = target;
		}
	}

	private void ApplyQuaternionStateDelayed()
	{
		if (!smoothenClientRotationLocally || !NetGame.isClient)
		{
			return;
		}
		Quaternion? quaternion = scrlLerpTarget;
		if (quaternion.HasValue)
		{
			float num = Quaternion.Angle(base.transform.rotation, scrlLerpTarget.Value);
			if (num < 0.1f || num > 5f)
			{
				base.transform.rotation = scrlLerpTarget.Value;
				scrlLerpTarget = null;
			}
			else
			{
				Quaternion rotation = Quaternion.Lerp(base.transform.rotation, scrlLerpTarget.Value, scrlTimeFactor * Time.deltaTime);
				base.transform.rotation = rotation;
			}
		}
	}

	private void ApplyEulerState(float diff)
	{
		appliedEuler = diff;
		Quaternion localRotation = baseRot;
		switch (syncRotation)
		{
		case NetBodySyncRotation.EulerX:
			localRotation *= Quaternion.AngleAxis(diff, Vector3.right);
			break;
		case NetBodySyncRotation.EulerY:
			localRotation *= Quaternion.AngleAxis(diff, Vector3.up);
			break;
		case NetBodySyncRotation.EulerZ:
			localRotation *= Quaternion.AngleAxis(diff, Vector3.forward);
			break;
		}
		if (base.transform.localRotation.eulerAngles != localRotation.eulerAngles)
		{
			base.transform.localRotation = localRotation;
		}
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		bool flag = false;
		NetVector3 netVector = default(NetVector3);
		NetVector3 netVector2 = default(NetVector3);
		NetQuaternion netQuaternion = default(NetQuaternion);
		NetQuaternion netQuaternion2 = default(NetQuaternion);
		if (syncPosition == NetBodySyncPosition.Relative || syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World || syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
		{
			if (syncPosition != 0)
			{
				netVector = ((state0 != null) ? NetVector3.Read(state0, posfull) : zero);
				netVector2 = NetVector3.Read(state1, posfull);
				flag |= netVector2 != netVector;
			}
			if (syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
			{
				netQuaternion = ((state0 != null) ? NetQuaternion.Read(state0, rotfull) : identity);
				netQuaternion2 = NetQuaternion.Read(state1, rotfull);
				flag |= netQuaternion2 != netQuaternion;
			}
			if (flag)
			{
				delta.Write(v: true);
				if (syncPosition != 0)
				{
					NetVector3.Delta(netVector, netVector2, poslarge).Write(delta, possmall, poslarge, posfull);
				}
				if (syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
				{
					NetQuaternion.Delta(netQuaternion, netQuaternion2, rotlarge).Write(delta, rotsmall, rotlarge, rotfull);
				}
			}
			else
			{
				delta.Write(v: false);
			}
		}
		if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			eulerEncoder.CalculateDelta(state0, state1, delta);
		}
		if (syncLocalScale)
		{
			NetVector3 netVector3 = default(NetVector3);
			NetVector3 netVector4 = default(NetVector3);
			netVector3 = ((state0 != null) ? NetVector3.Read(state0, posfull) : zero);
			netVector4 = NetVector3.Read(state1, posfull);
			if (netVector4 != netVector3)
			{
				delta.Write(v: true);
				NetVector3.Delta(netVector3, netVector4, poslarge).Write(delta, possmall, poslarge, posfull);
			}
			else
			{
				delta.Write(v: false);
			}
		}
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		if (syncPosition == NetBodySyncPosition.Relative || syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World || syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
		{
			if (delta.ReadBool())
			{
				if (syncPosition != 0)
				{
					NetVector3 from = ((state0 != null) ? NetVector3.Read(state0, posfull) : zero);
					NetVector3Delta delta2 = NetVector3Delta.Read(delta, possmall, poslarge, posfull);
					NetVector3.AddDelta(from, delta2).Write(result);
				}
				if (syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
				{
					NetQuaternion from2 = ((state0 != null) ? NetQuaternion.Read(state0, rotfull) : identity);
					NetQuaternionDelta delta3 = NetQuaternionDelta.Read(delta, rotsmall, rotlarge, rotfull);
					NetQuaternion.AddDelta(from2, delta3).Write(result);
				}
			}
			else
			{
				if (syncPosition != 0)
				{
					((state0 != null) ? NetVector3.Read(state0, posfull) : zero).Write(result);
				}
				if (syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
				{
					((state0 != null) ? NetQuaternion.Read(state0, rotfull) : identity).Write(result);
				}
			}
		}
		if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			eulerEncoder.AddDelta(state0, delta, result);
		}
		if (syncLocalScale)
		{
			if (delta.ReadBool())
			{
				NetVector3 from3 = ((state0 != null) ? NetVector3.Read(state0, posfull) : zero);
				NetVector3Delta delta4 = NetVector3Delta.Read(delta, possmall, poslarge, posfull);
				NetVector3.AddDelta(from3, delta4).Write(result);
			}
			else
			{
				((state0 != null) ? NetVector3.Read(state0, posfull) : zero).Write(result);
			}
		}
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		int num = 0;
		if (syncPosition == NetBodySyncPosition.Relative || syncPosition == NetBodySyncPosition.Absolute || syncPosition == NetBodySyncPosition.Local || syncPosition == NetBodySyncPosition.World || syncRotation == NetBodySyncRotation.Relative || syncRotation == NetBodySyncRotation.Absolute || syncRotation == NetBodySyncRotation.Local || syncRotation == NetBodySyncRotation.World)
		{
			num++;
			num += NetVector3Delta.CalculateMaxDeltaSizeInBits(possmall, poslarge, posfull);
			num += NetQuaternionDelta.CalculateMaxDeltaSizeInBits(rotsmall, rotlarge, rotfull);
		}
		if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			num += eulerEncoder.CalculateMaxDeltaSizeInBits();
		}
		if (syncLocalScale)
		{
			num++;
			num += NetVector3Delta.CalculateMaxDeltaSizeInBits(possmall, poslarge, posfull);
		}
		return num;
	}

	public void RefreshAbsolutePosRangeOverride(int value)
	{
		posRangeOverride = value;
		if (value != 0)
		{
			posRange = value;
			zero = NetVector3.Quantize(Vector3.zero, posRange, posfull);
		}
	}

	public void SetVisible(bool visible)
	{
		isVisible = visible;
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		if (manageActive)
		{
			bool flag = base.transform.position.y > despawnHeight && isVisible;
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
		}
	}

	public void AddOverride(RespawningBodyOverride respawningBodyOverride)
	{
		if (overrides == null)
		{
			overrides = new List<RespawningBodyOverride>();
		}
		overrides.Add(respawningBodyOverride);
	}

	public void HandleRespawn()
	{
		if (!(base.transform.position.y < despawnHeight))
		{
			return;
		}
		RespawnRoot componentInParent = GetComponentInParent<RespawnRoot>();
		if (respawn)
		{
			if (componentInParent != null)
			{
				componentInParent.Respawn(Vector3.up * respawnHeight);
			}
			else
			{
				Respawn(Vector3.up * respawnHeight);
			}
		}
		else if (componentInParent == null)
		{
			UpdateVisibility();
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		Respawn(Vector3.zero);
	}

	public void Respawn()
	{
		base.gameObject.GetComponent<IPreRespawn>()?.PreRespawn();
		Respawn(Vector3.up * respawnHeight);
	}

	private bool IsBadObject()
	{
		for (int i = 0; i < mBadObjectNames.Length; i++)
		{
			if (body.name.Equals(mBadObjectNames[i]))
			{
				return true;
			}
		}
		return false;
	}

	public void Respawn(Vector3 offset)
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		GrabManager.Release(base.gameObject);
		Vector3 vector = startPos + offset;
		if (ignoreRespawnOffset)
		{
			vector = startPos;
		}
		Quaternion quaternion = startRot;
		if (overrides != null)
		{
			RespawningBodyOverride respawningBodyOverride = null;
			for (int i = 0; i < overrides.Count; i++)
			{
				if (overrides[i].checkpoint.number <= Game.instance.currentCheckpointNumber && (respawningBodyOverride == null || overrides[i].checkpoint.number > respawningBodyOverride.checkpoint.number))
				{
					respawningBodyOverride = overrides[i];
				}
			}
			if (respawningBodyOverride != null)
			{
				vector = respawningBodyOverride.initialToNewLocationMatrtix.MultiplyPoint3x4(vector);
				quaternion = respawningBodyOverride.initialToNewLocationRotation * quaternion;
			}
		}
		HandlingReset = true;
		mJustReset = 5;
		mResetPosition = vector;
		mResetRotation = quaternion;
		base.transform.position = vector;
		base.transform.rotation = quaternion;
		if (syncLocalScale)
		{
			base.transform.localScale = startLocalScale;
		}
		UpdateVisibility();
		if (m_respawnEvent != null)
		{
			m_respawnEvent.Invoke();
		}
		else
		{
			Debug.LogError("Null respawn event.", this);
		}
	}

	private float Max(float dist, Vector3 p)
	{
		return Mathf.Max(dist, (p - base.transform.position).magnitude);
	}

	public float FurthestPointDistance()
	{
		float num = 0f;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i].GetComponentInParent<NetBody>() != this))
			{
				Bounds bounds = componentsInChildren[i].bounds;
				num = Max(num, new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
				num = Max(num, new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
				num = Max(num, new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
				num = Max(num, new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
				num = Max(num, new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
				num = Max(num, new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
				num = Max(num, new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
				num = Max(num, new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));
			}
		}
		return num;
	}

	public void CalculatePrecision(out float posMeter, out float rotDeg, out float rotMeter)
	{
		float num = posRange;
		ushort num2 = posfull;
		ushort num3 = rotfull;
		if (syncPosition == NetBodySyncPosition.Relative)
		{
			num = 10f;
			num2 -= 6;
		}
		if (syncPosition == NetBodySyncPosition.Local)
		{
			num = 10f;
			num2 -= 6;
		}
		if (posRangeOverride != 0f)
		{
			num = posRangeOverride;
		}
		num2 = (ushort)(num2 + posPrecision);
		num3 = (ushort)(num3 + rotPrecision);
		posMeter = num / (float)(1 << num2 - 1);
		rotDeg = 81f / (float)(1 << num3 - 2);
		if (syncRotation == NetBodySyncRotation.EulerX || syncRotation == NetBodySyncRotation.EulerY || syncRotation == NetBodySyncRotation.EulerZ)
		{
			rotDeg = 360f / (float)(1 << eulerEncoder.fullBits + rotPrecision);
		}
		rotMeter = Mathf.Sin(rotDeg * ((float)Math.PI / 180f)) * FurthestPointDistance();
		if (syncPosition == NetBodySyncPosition.None)
		{
			posMeter = 0.003f;
		}
		if (syncRotation == NetBodySyncRotation.None)
		{
			rotMeter = 0.003f;
		}
	}
}
