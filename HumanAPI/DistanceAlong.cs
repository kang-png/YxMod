using UnityEngine;

namespace HumanAPI;

public class DistanceAlong : Node
{
	public Vector3 axis;

	public NodeOutput value;

	private void FixedUpdate()
	{
		value.SetValue(Vector3.Dot(base.transform.localPosition, axis));
	}
}
