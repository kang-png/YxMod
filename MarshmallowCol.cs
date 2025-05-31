using UnityEngine;

public class MarshmallowCol : MonoBehaviour
{
	private MarshmallowAchievement scr;

	private void Start()
	{
		scr = GetComponent<MarshmallowAchievement>();
	}

	private void OnCollisionExit(Collision collision)
	{
		ConfigurableJoint component = GetComponent<ConfigurableJoint>();
		if (!component)
		{
			scr.output.SetValue(1f);
		}
	}
}
