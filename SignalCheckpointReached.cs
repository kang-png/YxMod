using HumanAPI;

public class SignalCheckpointReached : Node
{
	private int lastCurrentCheckpointInternal;

	public NodeOutput currentCheckpoint;

	private void Update()
	{
		UpdateCurrentCheckpoint((Game.instance != null) ? Game.instance.currentCheckpointNumber : 0);
	}

	private void UpdateCurrentCheckpoint(int value)
	{
		if (value != lastCurrentCheckpointInternal)
		{
			lastCurrentCheckpointInternal = value;
			currentCheckpoint.SetValue(lastCurrentCheckpointInternal);
		}
	}
}
