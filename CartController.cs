using UnityEngine;

public class CartController : MonoBehaviour
{
	private enum State
	{
		MovingToEnd,
		WaitingAtEnd,
		MovingToStart,
		WaitingAtStart,
		Stopped
	}

	public float waitTimeUnderGate;

	public float waitTimeOutsideHole;

	public float speed = 1f;

	public Transform start;

	public Transform end;

	private Rigidbody rigidbody;

	private float startAwaitTimer;

	private float endAwaitTimer;

	private State state;

	public void StopMoving()
	{
		state = State.Stopped;
	}

	public void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	public void Reset()
	{
		state = State.MovingToEnd;
	}

	public void FixedUpdate()
	{
		switch (state)
		{
		case State.MovingToEnd:
			rigidbody.MovePosition(base.transform.position + base.transform.forward * speed * Time.fixedDeltaTime);
			break;
		case State.MovingToStart:
			rigidbody.MovePosition(base.transform.position - base.transform.forward * speed * Time.fixedDeltaTime);
			break;
		case State.WaitingAtStart:
			startAwaitTimer -= Time.fixedDeltaTime;
			if (startAwaitTimer < 0f)
			{
				state = State.MovingToEnd;
			}
			break;
		case State.WaitingAtEnd:
			endAwaitTimer -= Time.fixedDeltaTime;
			if (endAwaitTimer < 0f)
			{
				state = State.MovingToStart;
			}
			break;
		case State.Stopped:
			break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform == start)
		{
			state = State.WaitingAtStart;
			startAwaitTimer = waitTimeUnderGate;
		}
		else if (other.transform == end)
		{
			state = State.WaitingAtEnd;
			endAwaitTimer = waitTimeOutsideHole;
		}
	}
}
