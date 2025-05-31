using System;
using System.Collections;
using HumanAPI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NavalCaveVolumeController : MonoBehaviour
{
	[Serializable]
	public struct Settings
	{
		public enum When
		{
			None,
			TriggerEnter,
			TriggerExit
		}

		public When when;

		public Vector3 direction;

		public float angleThreshold;

		public float lightIntensity;

		public float sunShadowStrength;

		public bool postProcessingState;

		public bool clouds;
	}

	public Settings insideVolume;

	public Settings outsideVolume;

	public float blendTime = 1f;

	public Light sun;

	public Light[] fillLights;

	public BuiltinLevel level;

	private PostProcessVolume post;

	public void ProcessTriggerEnter(Vector3 direction)
	{
		ProcessTrigger(direction, Settings.When.TriggerEnter);
	}

	public void ProcessTriggerExit(Vector3 direction)
	{
		ProcessTrigger(direction, Settings.When.TriggerExit);
	}

	private void ProcessTrigger(Vector3 direction, Settings.When when)
	{
		bool flag = Vector3.Angle(insideVolume.direction, direction) < insideVolume.angleThreshold && insideVolume.when == when;
		bool flag2 = Vector3.Angle(outsideVolume.direction, direction) < outsideVolume.angleThreshold && outsideVolume.when == when;
		if (flag)
		{
			EnterCaveVolume();
		}
		if (flag2)
		{
			LeaveCaveVolume();
		}
	}

	public void EnterCaveVolume()
	{
		StartCoroutine(Blend(insideVolume));
	}

	public void LeaveCaveVolume()
	{
		StartCoroutine(Blend(outsideVolume));
	}

	private IEnumerator Blend(Settings volumeSettings)
	{
		float blendTimer = blendTime;
		float initialSunIntensity = fillLights[0].intensity;
		float initialShadowStrength = sun.shadowStrength;
		level.noClouds = !volumeSettings.clouds;
		do
		{
			blendTimer -= Time.deltaTime;
			float factor = 1f - blendTimer / blendTime;
			sun.shadowStrength = Mathf.Lerp(initialShadowStrength, volumeSettings.sunShadowStrength, factor);
			Light[] array = fillLights;
			foreach (Light light in array)
			{
				light.intensity = Mathf.Lerp(initialSunIntensity, volumeSettings.lightIntensity, factor);
			}
			yield return null;
		}
		while (blendTimer > 0f);
	}

	private void Start()
	{
		post = GetComponent<PostProcessVolume>();
		Human human = UnityEngine.Object.FindObjectOfType<Human>();
		SoftTrigger component = GetComponent<SoftTrigger>();
		component.target = human.transform;
	}
}
