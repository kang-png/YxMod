using UnityEngine;

namespace HumanAPI;

public class MoveCart : MonoBehaviour
{
	public GameObject cart;

	public int force;

	private Rigidbody cartRB;

	private bool direction;

	private bool lastVal = true;

	private bool val = true;

	private bool addForce;

	private void Awake()
	{
		cartRB = cart.GetComponent<Rigidbody>();
	}

	public void SetVal(bool val)
	{
		if (val != lastVal)
		{
			addForce = true;
			lastVal = val;
		}
	}

	private void FixedUpdate()
	{
		if (addForce)
		{
			addForce = false;
			cartRB.AddForce(new Vector3((!direction) ? (-force) : force, 0f, 0f));
		}
	}

	public void SetDirectionForward()
	{
		direction = false;
	}

	public void SetDirectionBackward()
	{
		direction = true;
	}

	private void OnGUI()
	{
	}
}
