using HumanAPI;
using UnityEngine;

public class ThreeLapsAchievement : Node, IReset
{
	public NodeOutput output;

	private readonly float timer = 60f;

	private readonly int laps = 3;

	private float currentTimer;

	private LapCheckpoint start;

	private LapCheckpoint target;

	private LapCheckpoint previous;

	private int direction;

	private int currentLaps;

	public void ResetState(int checkpoint, int subObjectives)
	{
		currentTimer = timer;
		currentLaps = 0;
		start = null;
		target = null;
		previous = null;
		direction = 0;
		output.SetValue(0f);
	}

	public void CheckpointTriggered(LapCheckpoint triggered)
	{
		if (start == null)
		{
			ResetState(0, 0);
			start = triggered;
			previous = triggered;
		}
		else
		{
			if (previous == triggered)
			{
				return;
			}
			if (direction == 0)
			{
				if (triggered == start.Next)
				{
					target = triggered.Next;
					direction = 1;
				}
				else if (triggered == start.Previous)
				{
					target = triggered.Previous;
					direction = -1;
				}
				else
				{
					ResetState(0, 0);
				}
				previous = triggered;
			}
			else if (triggered == target)
			{
				if (direction == 1)
				{
					target = triggered.Next;
				}
				else if (direction == -1)
				{
					target = triggered.Previous;
				}
				previous = triggered;
				if (triggered == start)
				{
					currentLaps++;
					if (currentLaps == laps && currentTimer >= 0f)
					{
						output.SetValue(1f);
					}
				}
			}
			else
			{
				ResetState(0, 0);
			}
		}
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
	}
}
