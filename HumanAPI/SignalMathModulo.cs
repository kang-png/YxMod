using UnityEngine;

namespace HumanAPI;

[AddNodeMenuItem]
[AddComponentMenu("Human/Signals/Math/SignalMathModulo")]
public class SignalMathModulo : Node
{
	public NodeInput input;

	public NodeOutput output;

	public int modulo = 1;

	public override string Title => "Modulo: " + modulo;

	public override void Process()
	{
		output.SetValue(input.value % (float)modulo);
	}
}
