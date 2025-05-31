using UnityEngine;

namespace HumanAPI;

[AddNodeMenuItem]
[AddComponentMenu("Human/Signals/Math/SignalMathRandom")]
public class SignalMathRandom : Node
{
	public NodeInput min;

	public NodeInput max;

	[Tooltip("When the roll input goes above 0.5, a new number will be generated")]
	public NodeInput roll;

	public NodeOutput output;

	[Tooltip("If true, only whole numbers will be returned")]
	public bool returnIntegers;

	private float prevRoll;

	public override void Process()
	{
		if (prevRoll < 0.5f && roll.value >= 0.5f)
		{
			output.SetValue((!returnIntegers) ? Random.Range(min.value, max.value) : ((float)Random.Range((int)min.value, (int)max.value)));
		}
		prevRoll = roll.value;
	}
}
