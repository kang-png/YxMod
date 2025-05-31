using UnityEngine;

namespace HumanAPI;

public class SignalCounter : Node, IReset
{
	[Tooltip("What to set the output to when this is changed")]
	public NodeInput setValue;

	[Tooltip("Increase the output by one")]
	public NodeInput increment;

	[Tooltip("Decrease the output by one")]
	public NodeInput decrement;

	[Tooltip("Reset the output to zero")]
	public NodeInput reset;

	[Tooltip("Set the aimed value of the counter via node. Could be used with SignalRandom")]
	public NodeInput setAimedValue;

	public NodeOutput output;

	[Tooltip("Is the output less than")]
	public NodeOutput isLessThanAimedValue;

	public NodeOutput isAimedValue;

	public NodeOutput isGreaterThanAimedValue;

	public float aimedValue = 1f;

	[Tooltip("If true, the output will always be an integer, and will always round")]
	public bool integerOnly = true;

	private float prevSetValue;

	private float prevIncr;

	private float prevDecr;

	private float prevReset;

	private float prevSetAimedValue;

	private float originalAimedValue;

	public override string Title => $"Counter: {aimedValue.ToString()}";

	public void Awake()
	{
		originalAimedValue = aimedValue;
	}

	public override void Process()
	{
		if (setAimedValue.value != prevSetAimedValue)
		{
			aimedValue = setAimedValue.value;
		}
		if (setValue.value != prevSetValue)
		{
			output.SetValue(setValue.value);
		}
		if (increment.value >= 0.5f && prevIncr < 0.5f)
		{
			output.SetValue(output.value + 1f);
		}
		if (decrement.value >= 0.5f && prevDecr < 0.5f)
		{
			output.SetValue(output.value - 1f);
		}
		if (reset.value >= 0.5f && prevReset < 0.5f)
		{
			output.SetValue(0f);
		}
		if (integerOnly)
		{
			output.SetValue(Mathf.Round(output.value));
		}
		isLessThanAimedValue.SetValue((output.value < aimedValue) ? 1 : 0);
		isAimedValue.SetValue(Mathf.Approximately(output.value, aimedValue) ? 1 : 0);
		isGreaterThanAimedValue.SetValue((output.value > aimedValue) ? 1 : 0);
		prevSetValue = setValue.value;
		prevIncr = increment.value;
		prevDecr = decrement.value;
		prevReset = reset.value;
		prevSetAimedValue = setAimedValue.value;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		output.SetValue(0f);
		isLessThanAimedValue.SetValue((output.value < aimedValue) ? 1 : 0);
		isAimedValue.SetValue(Mathf.Approximately(output.value, aimedValue) ? 1 : 0);
		isGreaterThanAimedValue.SetValue((output.value > aimedValue) ? 1 : 0);
		aimedValue = originalAimedValue;
	}
}
