using HumanAPI;
using Multiplayer;

public class NetBodyResetSensor : Node
{
	public NodeOutput value;

	public NodeOutput invertedValue;

	public NetBody bodyToTrack;

	private void FixedUpdate()
	{
		if (!ReplayRecorder.isPlaying && !NetGame.isClient && !(bodyToTrack == null))
		{
			value.SetValue(bodyToTrack.HandlingReset ? 1 : 0);
			invertedValue.SetValue((!bodyToTrack.HandlingReset) ? 1 : 0);
		}
	}
}
