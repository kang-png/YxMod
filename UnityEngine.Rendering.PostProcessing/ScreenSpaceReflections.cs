using System;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ScreenSpaceReflectionsRenderer), "Unity/Screen-space reflections", true)]
public sealed class ScreenSpaceReflections : PostProcessEffectSettings
{
	[Tooltip("Choose a quality preset, or use \"Custom\" to fine tune it. Don't use a preset higher than \"Medium\" if you care about performances on consoles.")]
	public ScreenSpaceReflectionPresetParameter preset = new ScreenSpaceReflectionPresetParameter
	{
		value = ScreenSpaceReflectionPreset.Medium
	};

	[Range(0f, 256f)]
	[Tooltip("Maximum iteration count.")]
	public IntParameter maximumIterationCount = new IntParameter
	{
		value = 16
	};

	[Tooltip("Changes the size of the SSR buffer. Downsample it to maximize performances or supersample it to get slow but higher quality results.")]
	public ScreenSpaceReflectionResolutionParameter resolution = new ScreenSpaceReflectionResolutionParameter
	{
		value = ScreenSpaceReflectionResolution.Downsampled
	};

	[Range(1f, 64f)]
	[Tooltip("Ray thickness. Lower values are more expensive but allow the effect to detect smaller details.")]
	public FloatParameter thickness = new FloatParameter
	{
		value = 8f
	};

	[Tooltip("Maximum distance to traverse after which it will stop drawing reflections.")]
	public FloatParameter maximumMarchDistance = new FloatParameter
	{
		value = 100f
	};

	[Range(0f, 1f)]
	[Tooltip("Fades reflections close to the near planes.")]
	public FloatParameter distanceFade = new FloatParameter
	{
		value = 0.5f
	};

	[Range(0f, 1f)]
	[Tooltip("Fades reflections close to the screen edges.")]
	public FloatParameter vignette = new FloatParameter
	{
		value = 0.5f
	};

	public override bool IsEnabledAndSupported(PostProcessRenderContext context)
	{
		return (bool)enabled && context.camera.actualRenderingPath == RenderingPath.DeferredShading && SystemInfo.supportsMotionVectors && SystemInfo.supportsComputeShaders && SystemInfo.copyTextureSupport > CopyTextureSupport.None && (bool)context.resources.shaders.screenSpaceReflections && context.resources.shaders.screenSpaceReflections.isSupported && (bool)context.resources.computeShaders.gaussianDownsample;
	}
}
