using UnityEngine;

public class TriggerAchievement : MonoBehaviour
{
	public string AchievementName;

	public void AchievementUnlocked()
	{
		if (AchievementName != string.Empty)
		{
			Debug.LogFormat("{0} Achievement unlocked!", AchievementName);
		}
		else
		{
			Debug.LogFormat("Unknown achievement unlocked, please check input on {0}.", base.gameObject.transform.name);
		}
	}
}
