using HumanAPI;
using UnityEngine;

public class SignalShattered : Node
{
	public NodeOutput output;

	[SerializeField]
	private ShatterBase shatterObjectToTrack;

	private bool previousValue;

	private void Update()
	{
		if (previousValue != shatterObjectToTrack.shattered)
		{
			OnShatterStateChanged(shatterObjectToTrack.shattered);
			previousValue = shatterObjectToTrack.shattered;
		}
	}

	private void OnShatterStateChanged(bool value)
	{
		output.SetValue(value ? 1 : 0);
	}
}
