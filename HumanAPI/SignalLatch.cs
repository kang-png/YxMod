using UnityEngine;

namespace HumanAPI;

public class SignalLatch : Node, IPreReset, IPostEndReset
{
	public NodeInput input;

	public NodeInput latch;

	public NodeInput reset;

	public NodeOutput output;

	public bool ignoreCheckpointReset;

	private bool latched;

	private float outputValueLatched;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	public override void Process()
	{
		base.Process();
		if (!latched)
		{
			output.SetValue(input.value);
			if (showDebug)
			{
				Debug.Log(base.name + " Latch Setting " + input.value);
			}
			if (latch.value > 0.5f)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Latch True ");
				}
				latched = true;
				outputValueLatched = output.value;
			}
		}
		else if (reset.value > 0.5f)
		{
			latched = false;
			output.SetValue(input.value);
		}
		else
		{
			output.SetValue(outputValueLatched);
		}
	}

	public void PreResetState(int checkpoint)
	{
		if (!ignoreCheckpointReset)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset State ");
			}
			latched = false;
			latch.value = 0f;
			output.SetValue(output.initialValue);
		}
	}

	public void PostEndResetState(int checkpoint)
	{
		if (ignoreCheckpointReset)
		{
			output.SetValue(outputValueLatched);
		}
	}
}
