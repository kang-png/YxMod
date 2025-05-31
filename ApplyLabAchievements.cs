using HumanAPI;
using UnityEngine;

public class ApplyLabAchievements : MonoBehaviour
{
	public UnlockAchievement[] achievementUnlockers = new UnlockAchievement[0];

	private void Start()
	{
		UnlockAchievement[] array = achievementUnlockers;
		foreach (UnlockAchievement unlockAchievement in array)
		{
			switch (unlockAchievement.name)
			{
			case "LavaEnd":
			case "WaterSquare":
				unlockAchievement.achievementToUnlock = Achievement.ACH_LAB_CAST_IT_INTO_THE_FIRE;
				break;
			case "KaboomTrigger":
				unlockAchievement.achievementToUnlock = Achievement.ACH_LAB_KABOOM;
				break;
			case "Scene3: PlatformRoom":
				unlockAchievement.achievementToUnlock = Achievement.ACH_LAB_OVERACHIEVER;
				break;
			case "StageCompleteSystem_C3":
				unlockAchievement.achievementToUnlock = Achievement.ACH_LAB_BULLSEYE;
				break;
			}
		}
	}
}
