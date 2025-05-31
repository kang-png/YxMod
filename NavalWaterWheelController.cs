using Multiplayer;
using UnityEngine;

public class NavalWaterWheelController : MonoBehaviour, INetBehavior
{
	private HingeJoint joint;

	private float waterCount;

	private bool flipped;

	private NetFloat waterCountEncoder = new NetFloat(2f, 9, 3, 3);

	private void Start()
	{
		joint = GetComponent<HingeJoint>();
	}

	private void UpdateMotorAndParticles()
	{
		joint.useMotor = waterCount > 0.5f;
		UpdateParticles();
	}

	private void UpdateParticles()
	{
		bool flag = waterCount > 0.5f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!ReplayRecorder.isPlaying && !NetGame.isClient)
		{
			WaterPlane component = other.GetComponent<WaterPlane>();
			if (!(component == null))
			{
				waterCount += 1f;
				UpdateMotorAndParticles();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!ReplayRecorder.isPlaying && !NetGame.isClient)
		{
			WaterPlane component = other.GetComponent<WaterPlane>();
			if (!(component == null))
			{
				waterCount -= 1f;
				UpdateMotorAndParticles();
			}
		}
	}

	private void Update()
	{
		if (ReplayRecorder.isPlaying && NetGame.isClient && flipped)
		{
			UpdateParticles();
			flipped = false;
		}
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}

	public void CollectState(NetStream stream)
	{
		waterCountEncoder.CollectState(stream, waterCount);
	}

	public void ApplyState(NetStream state)
	{
		float num = Mathf.Floor(waterCount);
		waterCount = Mathf.Floor(waterCountEncoder.ApplyState(state));
		flipped = num != waterCount;
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		float num = Mathf.Floor(waterCount);
		waterCount = Mathf.Floor(waterCountEncoder.ApplyLerpedState(state0, state1, mix));
		flipped = num != waterCount;
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		waterCountEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		waterCountEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return waterCountEncoder.CalculateMaxDeltaSizeInBits();
	}

	public void SetMaster(bool isMaster)
	{
	}
}
