using UnityEngine;

public class BenchAlignAchievement : MonoBehaviour
{
	private bool awarded;

	private void Update()
	{
		if (!awarded && base.transform.forward.y > 0.97f && base.transform.up.x < -0.97f)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_PUSH_BENCH_ALIGN);
			awarded = true;
		}
	}
}
