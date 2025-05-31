using UnityEngine;

public class BallCheck : MonoBehaviour
{
	public BallStats stats;

	public BallAchievement scr;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Player") && stats.collisions == 0)
		{
			scr.output.SetValue(1f);
		}
	}
}
