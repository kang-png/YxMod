using UnityEngine;

public class WindowShortcutAchievement : MonoBehaviour
{
	private Collider trackedCollider;

	private float entryX;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			entryX = base.transform.InverseTransformPoint(other.transform.position).x;
			if (entryX < 0f)
			{
				trackedCollider = other;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other == trackedCollider)
		{
			if (base.transform.InverseTransformPoint(other.transform.position).x * entryX < 0f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_BREAK_WINDOW_SHORTCUT);
			}
			trackedCollider = null;
		}
	}
}
