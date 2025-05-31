using UnityEngine;

public class BoatStats : MonoBehaviour, IReset
{
	public int collisions;

	public void IncreaseCount()
	{
		collisions++;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (checkpoint <= 5)
		{
			collisions = 0;
		}
	}
}
