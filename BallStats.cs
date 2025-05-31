using UnityEngine;

public class BallStats : MonoBehaviour, IReset
{
	public int collisions;

	public void IncreaseCount()
	{
		collisions++;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (checkpoint < 11)
		{
			collisions = 0;
		}
	}
}
