using HumanAPI;
using UnityEngine;

public class SignalChangeLayer : Node, IReset
{
	public GameObject objectToChange;

	[Tooltip("Layer to change to when input >= 0.5")]
	public int changeToLayer = 9;

	[Tooltip("Revert back to previous layer after set amount of time?")]
	public bool revertAfterTime;

	[Tooltip("How long after initial layer change until reverting if revertAfterTime is true")]
	public float timeUntilRevert = 5f;

	public NodeInput input;

	private float prevInput;

	private int initialLayer;

	private float timeSinceChange;

	public void OnValidate()
	{
		objectToChange = objectToChange ?? base.gameObject;
	}

	public void Awake()
	{
		initialLayer = objectToChange.layer;
	}

	public override void Process()
	{
		base.Process();
		if (prevInput < 0.5f && input.value >= 0.5f)
		{
			objectToChange.layer = changeToLayer;
			timeSinceChange = 0f;
		}
		prevInput = input.value;
	}

	public void Update()
	{
		if (revertAfterTime && !(timeSinceChange > timeUntilRevert))
		{
			timeSinceChange += Time.deltaTime;
			if (timeSinceChange >= timeUntilRevert)
			{
				objectToChange.layer = initialLayer;
			}
		}
	}

	public void ResetState(int checkpoint, int subObject)
	{
		objectToChange.layer = initialLayer;
	}
}
