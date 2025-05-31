using UnityEngine;

namespace HumanAPI;

public class FallCheckpoint : Checkpoint, IReset
{
	public Rigidbody fallingObject;

	public float triggerYPos = 5f;

	private bool triggered;

	private Transform fallTransform;

	private void Awake()
	{
		if (fallingObject != null)
		{
			fallTransform = fallingObject.transform;
		}
	}

	private void FixedUpdate()
	{
		if (!triggered && !(fallTransform == null) && fallTransform.position.y < triggerYPos)
		{
			triggered = true;
			Pass();
		}
	}

	void IReset.ResetState(int checkpoint, int subObjectives)
	{
		if (checkpoint <= number)
		{
			triggered = false;
		}
	}
}
