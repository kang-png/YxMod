using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(UPGEN_Lighting_Renderer), PostProcessEvent.BeforeTransparent, "Custom/UPGEN Lighting", true)]
public sealed class UPGEN_Lighting : PostProcessEffectSettings
{
	[Range(0f, 5f)]
	[Tooltip("Global effect intensity.")]
	public FloatParameter intensity = new FloatParameter
	{
		value = 1f
	};

	public override bool IsEnabledAndSupported(PostProcessRenderContext context)
	{
		return enabled.value && intensity.value > 0f;
	}
}
