using HumanAPI;

public class AchievementLasersVolume : Node
{
	public NodeInput input;

	public override void Process()
	{
		if (input.value >= 0.5f)
		{
			AchievementLasers.HasTriggeredLasers = true;
		}
	}
}
