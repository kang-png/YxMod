using UnityEngine;

namespace HumanAPI;

public class CollisionByTagSensor : Node
{
	public NodeOutput output;

	public string TagToCheck;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag.Equals(TagToCheck))
		{
			output.SetValue(1f);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag.Equals(TagToCheck))
		{
			output.SetValue(1f);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag.Equals(TagToCheck))
		{
			output.SetValue(0f);
		}
	}
}
