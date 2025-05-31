using HumanAPI;
using UnityEngine;

public class Bellows : Node
{
	public NodeOutput output;

	public float velocityOutputScale;

	public Rigidbody topBellow;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		output.SetValue(Mathf.Max(topBellow.angularVelocity.x * velocityOutputScale, 0f));
	}
}
