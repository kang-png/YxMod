using UnityEngine;

namespace HumanAPI;

[AddComponentMenu("Human/Signals/MathSignalMathCompare", 10)]
public class SignalMathCompare : Node
{
	public enum SignalCompareOperation
	{
		EqualExact,
		EqualApprox,
		NotEqualExact,
		NotEqualApprox,
		LessThan,
		GreaterThan,
		LessThanOrEqual,
		GreaterThanOrEqual
	}

	public NodeInput in1;

	public NodeInput in2;

	public NodeOutput output;

	public NodeOutput invertedOutput;

	public SignalCompareOperation operation;

	public override string Title => "Compare: " + operation;

	public override void Process()
	{
		bool flag = operation switch
		{
			SignalCompareOperation.EqualExact => in1.value == in2.value, 
			SignalCompareOperation.EqualApprox => Mathf.Approximately(in1.value, in2.value), 
			SignalCompareOperation.NotEqualExact => in1.value != in2.value, 
			SignalCompareOperation.NotEqualApprox => !Mathf.Approximately(in1.value, in2.value), 
			SignalCompareOperation.LessThan => in1.value < in2.value, 
			SignalCompareOperation.GreaterThan => in1.value > in2.value, 
			SignalCompareOperation.LessThanOrEqual => in1.value <= in2.value, 
			SignalCompareOperation.GreaterThanOrEqual => in1.value >= in2.value, 
			_ => false, 
		};
		output.SetValue((!flag) ? 0f : 1f);
		invertedOutput.SetValue((!flag) ? 1f : 0f);
	}
}
