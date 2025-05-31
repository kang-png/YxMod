using UnityEngine;

public class BallCol : MonoBehaviour
{
	[SerializeField]
	public BallStats stats;

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.gameObject.tag != "Player") && (bool)stats)
		{
			stats.IncreaseCount();
		}
	}
}
