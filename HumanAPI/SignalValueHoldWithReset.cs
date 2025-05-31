namespace HumanAPI;

public class SignalValueHoldWithReset : Node
{
	public NodeInput input;

	public NodeInput release;

	public bool onlyTriggerOnce;

	public NodeOutput output;

	private bool hasTriggered;

	private bool bSentOutput;

	public override void Process()
	{
		if (!onlyTriggerOnce || !hasTriggered)
		{
			if (input.value > 0f)
			{
				bSentOutput = true;
				output.SetValue(1f);
			}
			if (release.value > 0f)
			{
				Reset();
			}
		}
	}

	public void Reset()
	{
		hasTriggered = false;
		bSentOutput = false;
		output.SetValue(0f);
	}
}
