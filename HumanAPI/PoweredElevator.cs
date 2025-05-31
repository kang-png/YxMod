using System.Collections;
using UnityEngine;

namespace HumanAPI;

[RequireComponent(typeof(Rigidbody))]
public class PoweredElevator : Node
{
	public NodeInput input;

	public NodeOutput output;

	public NodeOutput invertedOutput;

	public float speed = 1f;

	public float unitsToMove = 5f;

	public float holdTime = 2f;

	private Rigidbody rb;

	private Vector3 startPos;

	private Vector3 endPos;

	private Vector3 direction = Vector3.up;

	private Vector3 oldDirection = Vector3.up;

	private Vector3 destination;

	private bool isAtEnd;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		startPos = base.transform.position;
		endPos = base.transform.position + Vector3.up * unitsToMove;
		destination = endPos;
	}

	public override void Process()
	{
		base.Process();
		direction = Vector3.up;
		output.SetValue(1f);
		invertedOutput.SetValue(0f);
	}

	private void FixedUpdate()
	{
		if ((double)input.value > 0.5)
		{
			rb.MovePosition(base.transform.position + direction * speed * Time.deltaTime);
			if ((double)(base.transform.position - destination).sqrMagnitude < 0.01 && !isAtEnd)
			{
				isAtEnd = true;
				direction = Vector3.zero;
				StartCoroutine(SwitchDirection());
			}
		}
		else if ((double)input.value < 0.5)
		{
			if ((double)(base.transform.position - startPos).sqrMagnitude > 0.01)
			{
				rb.MovePosition(base.transform.position + -Vector3.up * speed * Time.deltaTime);
				return;
			}
			rb.velocity = Vector3.zero;
			invertedOutput.SetValue(1f);
			output.SetValue(0f);
		}
	}

	private IEnumerator SwitchDirection()
	{
		invertedOutput.SetValue(1f);
		output.SetValue(0f);
		yield return new WaitForSeconds(holdTime);
		direction = ((!(oldDirection == Vector3.up)) ? Vector3.up : (-Vector3.up));
		oldDirection = direction;
		destination = ((!(destination == endPos)) ? endPos : startPos);
		isAtEnd = false;
		output.SetValue(1f);
		invertedOutput.SetValue(0f);
	}
}
