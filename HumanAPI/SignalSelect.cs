using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI;

[AddNodeMenuItem]
[AddComponentMenu("Human/Signals/Math/SignalSelect")]
public class SignalSelect : Node
{
	public NodeInput[] inputs = new NodeInput[2];

	public NodeInput setOutput;

	public NodeInput cycle;

	public NodeInput reset;

	[ReadOnly]
	public NodeOutput[] outputs;

	private int activeInput = -1;

	private float setOutputPrevInput;

	private float cyclePrevInput;

	private float resetPrevInput;

	public override string Title => "Select - Inputs: " + inputs.Length;

	private int ActiveInput
	{
		get
		{
			return activeInput;
		}
		set
		{
			activeInput = Mathf.Max((value < inputs.Length) ? value : 0, -1);
		}
	}

	protected override void CollectAllSockets(List<NodeSocket> sockets)
	{
		if (inputs == null || inputs.Length < 3)
		{
			inputs = new NodeInput[2];
		}
		if (inputs != null && inputs.Length > 0)
		{
			if (outputs == null || outputs.Length != inputs.Length)
			{
				outputs = new NodeOutput[inputs.Length];
			}
			for (int i = 0; i < inputs.Length; i++)
			{
				inputs[i] = inputs[i] ?? new NodeInput();
				inputs[i].node = this;
				inputs[i].name = "input" + i;
				sockets.Add(inputs[i]);
			}
			for (int j = 0; j < inputs.Length; j++)
			{
				outputs[j] = outputs[j] ?? new NodeOutput();
				outputs[j].node = this;
				outputs[j].name = "output" + j;
				sockets.Add(outputs[j]);
			}
		}
		setOutput.node = (cycle.node = (reset.node = this));
		setOutput.name = "set output";
		cycle.name = "cycle";
		reset.name = "reset";
		sockets.Add(setOutput);
		sockets.Add(cycle);
		sockets.Add(reset);
	}

	public override void Process()
	{
		if (setOutputPrevInput != setOutput.value)
		{
			ActiveInput = Mathf.RoundToInt(setOutput.value);
		}
		else if (resetPrevInput < 0.5f && reset.value >= 0.5f)
		{
			activeInput = -1;
		}
		else if (cyclePrevInput < 0.5f && cycle.value >= 0.5f)
		{
			ActiveInput++;
		}
		for (int i = 0; i < outputs.Length; i++)
		{
			outputs[i].SetValue((i != activeInput) ? 0f : inputs[i].value);
		}
		setOutputPrevInput = setOutput.value;
		cyclePrevInput = cycle.value;
		resetPrevInput = reset.value;
	}
}
