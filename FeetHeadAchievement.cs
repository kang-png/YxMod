using UnityEngine;

public class FeetHeadAchievement : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Human componentInParent = other.GetComponentInParent<Human>();
			if (componentInParent.ragdoll.partHead.transform.position.y > componentInParent.ragdoll.partHips.transform.position.y)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_LVL_RIVER_FEET);
			}
			else
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_LVL_RIVER_HEAD);
			}
		}
	}
}
