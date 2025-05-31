using System;
using UnityEngine;

public class LightTweekBasedOnRange : MonoBehaviour
{
	[Serializable]
	public class LightSetting
	{
		public Light light;

		public Transform position1;

		public Transform position2;

		public float intensity1;

		public float intensity2;
	}

	[SerializeField]
	private LightSetting[] lights;

	private void Update()
	{
		if (!(Human.Localplayer == null) && lights != null && lights.Length != 0)
		{
			Vector3 position = Human.Localplayer.gameObject.transform.position;
			LightSetting[] array = lights;
			foreach (LightSetting lightSetting in array)
			{
				lightSetting.light.intensity = Mathf.Lerp(lightSetting.intensity1, lightSetting.intensity2, GetInterpolationRatio(lightSetting.position1.position, lightSetting.position2.position, position));
			}
		}
	}

	private float GetInterpolationRatio(Vector3 vA, Vector3 vB, Vector3 vPoint)
	{
		Vector3 rhs = vPoint - vA;
		Vector3 normalized = (vB - vA).normalized;
		float num = Vector3.Distance(vA, vB);
		float num2 = Vector3.Dot(normalized, rhs);
		if (num2 <= 0f)
		{
			return 0f;
		}
		if (num2 >= num)
		{
			return 1f;
		}
		return Mathf.Clamp01(num2 / num);
	}
}
