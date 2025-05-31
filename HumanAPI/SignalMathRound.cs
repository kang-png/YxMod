using System;
using UnityEngine;

namespace HumanAPI;

[AddNodeMenuItem]
[AddComponentMenu("Human/Signals/Math/SignalMathRound")]
public class SignalMathRound : Node
{
	[Serializable]
	public enum ROUNDDIRECTION
	{
		UP,
		DOWN,
		NEAREST
	}

	public NodeInput input;

	public NodeOutput output;

	public ROUNDDIRECTION operation = ROUNDDIRECTION.NEAREST;

	public override string Title => "Round: " + operation;

	public override void Process()
	{
		output.SetValue((operation == ROUNDDIRECTION.NEAREST) ? Mathf.Round(input.value) : ((operation != ROUNDDIRECTION.DOWN) ? Mathf.Ceil(input.value) : Mathf.Floor(input.value)));
	}
}
