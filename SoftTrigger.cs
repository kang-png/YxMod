using System;
using UnityEngine;

public class SoftTrigger : MonoBehaviour
{
	[NonSerialized]
	public Transform target;

	private bool inside;

	public float distance = 1f;

	public SoftTriggerEvent onEnter;

	public SoftTriggerEvent onExit;

	private void Start()
	{
		inside = IsInside();
	}

	private void Update()
	{
		bool flag = IsInside();
		if (flag != inside)
		{
			inside = flag;
			Vector3 normalized = (target.position - base.transform.position).normalized;
			if (inside)
			{
				onEnter.Invoke(normalized);
			}
			else
			{
				onExit.Invoke(normalized);
			}
		}
	}

	private bool IsInside()
	{
		if (target == null)
		{
			return false;
		}
		float magnitude = (base.transform.position - target.position).magnitude;
		return magnitude < distance;
	}
}
