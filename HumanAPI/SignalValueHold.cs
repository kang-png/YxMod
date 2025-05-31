namespace HumanAPI;

public class SignalValueHold : Node
{
	public NodeInput input;

	public bool onlyTriggerOnce;

	public NodeOutput output;

	private bool hasTriggered;

	private bool bSentOutput;

	public override void Process()
	{
		if ((!onlyTriggerOnce || !hasTriggered) && input.value > 0f)
		{
			bSentOutput = true;
			output.SetValue(1f);
		}
	}

	public void Reset()
	{
		hasTriggered = false;
		bSentOutput = false;
		output.SetValue(0f);
	}
}
