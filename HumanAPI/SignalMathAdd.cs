using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI;

[AddNodeMenuItem]
[AddComponentMenu("Human/Signals/Math/SignalMathAdd", 10)]
public class SignalMathAdd : Node
{
	public NodeInput in1;

	public NodeInput in2;

	public NodeInput[] extraInputs;

	public NodeOutput output;

	protected override void CollectAllSockets(List<NodeSocket> sockets)
	{
		in1.node = this;
		in1.name = "input1";
		in2.node = this;
		in2.name = "input2";
		sockets.Add(in1);
		sockets.Add(in2);
		if (extraInputs != null && extraInputs.Length > 0)
		{
			for (int i = 0; i < extraInputs.Length; i++)
			{
				extraInputs[i].node = this;
				extraInputs[i].name = "input" + (i + 3);
				sockets.Add(extraInputs[i]);
			}
		}
		output.name = "output";
		output.node = this;
		sockets.Add(output);
	}

	public override void Process()
	{
		float num = in1.value + in2.value;
		if (extraInputs != null)
		{
			for (int i = 0; i < extraInputs.Length; i++)
			{
				if (extraInputs[i] != null)
				{
					num += extraInputs[i].value;
				}
			}
		}
		output.SetValue(num);
	}
}
