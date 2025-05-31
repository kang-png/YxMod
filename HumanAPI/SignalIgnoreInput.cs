using UnityEngine;

namespace HumanAPI;

public class SignalIgnoreInput : Node, IReset, IPreReset
{
	public NodeInput input;

	public NodeOutput output;

	[Tooltip("Length of time to ignore input after a checkpoint or level reset")]
	public float ignoreTime = 0.5f;

	private float timer;

	private void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
		}
	}

	public override void Process()
	{
		base.Process();
		if (!(timer > 0f))
		{
			output.SetValue(input.value);
		}
	}

	public void PreResetState(int checkpoint)
	{
		output.value = 0f;
		timer = ignoreTime;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		output.value = 0f;
		timer = ignoreTime;
	}
}
