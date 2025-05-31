using UnityEngine;

public class NavalCaveLightChanger : MonoBehaviour, IReset
{
	public Vector3 inDirection;

	public AnimationCurve transition;

	public float transitionDistance;

	public Color targetColor;

	private Human human;

	private Color originalColor;

	private void Start()
	{
		human = Human.Localplayer;
		originalColor = RenderSettings.ambientLight;
	}

	private void Update()
	{
		Vector3 to = human.transform.position - base.transform.position;
		if (!(Vector3.Angle(inDirection, to) >= 90f))
		{
			float magnitude = to.magnitude;
			if (!(magnitude > transitionDistance + 1f))
			{
				float time = Mathf.Clamp01(magnitude / transitionDistance);
				float t = transition.Evaluate(time);
				RenderSettings.ambientLight = Color.Lerp(originalColor, targetColor, t);
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		RenderSettings.ambientLight = originalColor;
	}
}
