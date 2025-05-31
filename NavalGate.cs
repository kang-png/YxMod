using UnityEngine;

public class NavalGate : MonoBehaviour
{
	public Animator waterAnimator;

	public float high;

	public float low;

	private int animatorAttributeId = Animator.StringToHash("gateHeight");

	private float minAnimAttr = -1f;

	private float maxAnimAttr = 1f;

	public void Update()
	{
		float value = scale(low, high, minAnimAttr, maxAnimAttr, base.transform.localPosition.y);
		waterAnimator.SetFloat(animatorAttributeId, value);
	}

	public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
	{
		float num = OldMax - OldMin;
		float num2 = NewMax - NewMin;
		return (OldValue - OldMin) * num2 / num + NewMin;
	}
}
