using UnityEngine;

namespace HumanAPI;

public class GolfParAchievement : MonoBehaviour, IReset
{
	private int hitCount;

	private bool grabbed;

	private float timeSinceHit;

	public bool debugColliderNames;

	[Header("Number of hits allowed to unlock achievement")]
	[SerializeField]
	private int maxHits = 3;

	private const float kTimeBetweenHits = 1f;

	[Header("ColliderLabel strings checked for by this script")]
	[SerializeField]
	private string labelHole = "ParAchievementHole";

	[SerializeField]
	private string labelClub = "ParAchievementClub";

	private void OnCollisionEnter(Collision collision)
	{
		if (debugColliderNames)
		{
			Debug.LogError("Collision entered: " + collision.collider.gameObject.name);
		}
		ColliderLabel component = collision.collider.gameObject.GetComponent<ColliderLabel>();
		if ((bool)component)
		{
			if (component.Label == labelClub)
			{
				if (timeSinceHit == 0f)
				{
					hitCount++;
					timeSinceHit = 1f;
				}
			}
			else if (component.Label == labelHole && hitCount <= maxHits && !grabbed)
			{
				UnlockParAchievement();
			}
		}
		if (collision.collider.gameObject.GetComponentInParent<Ragdoll>() != null)
		{
			grabbed = true;
		}
	}

	private void Update()
	{
		if (timeSinceHit > 0f)
		{
			timeSinceHit -= Time.deltaTime;
			if (timeSinceHit <= 0f)
			{
				timeSinceHit = 0f;
			}
		}
	}

	private void UnlockParAchievement()
	{
		StatsAndAchievements.UnlockAchievement(Achievement.ACH_GOLF_PAR);
	}

	public void ResetState(int checkpointNum, int subCheckpointNum)
	{
		hitCount = 0;
		timeSinceHit = 0f;
		grabbed = false;
	}
}
