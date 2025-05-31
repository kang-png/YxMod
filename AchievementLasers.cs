using UnityEngine;

public class AchievementLasers : MonoBehaviour, IReset
{
	public static bool HasTriggeredLasers;

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (checkpoint == 0 && subObjectives == 0)
		{
			HasTriggeredLasers = false;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!HasTriggeredLasers)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_MUSEUM_LASERS);
		}
	}
}
