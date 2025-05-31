using Multiplayer;
using UnityEngine;

public class PlanksNoThanksAchievement : MonoBehaviour, IReset
{
	public static PlanksNoThanksAchievement instance;

	public float plankMoveThreshold;

	public float plankAngleThreshold;

	public NetBody[] plankNetBodies;

	private bool isCancelled;

	private bool hasCheckedCheckpointStart;

	public static bool AchievementValid => instance != null && !instance.isCancelled;

	private void Awake()
	{
		instance = this;
	}

	public void OnDestroy()
	{
		instance = null;
	}

	public void Update()
	{
		if (NetGame.isClient || isCancelled)
		{
			return;
		}
		NetBody[] array = plankNetBodies;
		foreach (NetBody netBody in array)
		{
			if (netBody != null && netBody.RigidBody != null)
			{
				NetBody netBody2 = netBody;
				Rigidbody rigidBody = netBody.RigidBody;
				if (Vector3.Distance(rigidBody.position, netBody2.startPos) >= plankMoveThreshold || Quaternion.Angle(rigidBody.rotation, netBody2.startRot) >= plankAngleThreshold)
				{
					isCancelled = true;
					break;
				}
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (!hasCheckedCheckpointStart || checkpoint == 0)
		{
			hasCheckedCheckpointStart = true;
			isCancelled = checkpoint != 0;
		}
	}
}
