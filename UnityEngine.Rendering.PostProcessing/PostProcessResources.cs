using System;

namespace UnityEngine.Rendering.PostProcessing;

public sealed class PostProcessResources : ScriptableObject
{
	[Serializable]
	public sealed class Shaders
	{
		public Shader bloom;

		public Shader copy;

		public Shader copyStd;

		public Shader discardAlpha;

		public Shader depthOfField;

		public Shader finalPass;

		public Shader grainBaker;

		public Shader motionBlur;

		public Shader temporalAntialiasing;

		public Shader subpixelMorphologicalAntialiasing;

		public Shader texture2dLerp;

		public Shader uber;

		public Shader lut2DBaker;

		public Shader lightMeter;

		public Shader gammaHistogram;

		public Shader waveform;

		public Shader vectorscope;

		public Shader debugOverlays;

		public Shader deferredFog;

		public Shader scalableAO;

		public Shader multiScaleAO;

		public Shader screenSpaceReflections;

		public Shaders Clone()
		{
			return (Shaders)MemberwiseClone();
		}
	}

	[Serializable]
	public sealed class ComputeShaders
	{
		public ComputeShader autoExposure;

		public ComputeShader exposureHistogram;

		public ComputeShader lut3DBaker;

		public ComputeShader texture3dLerp;

		public ComputeShader gammaHistogram;

		public ComputeShader waveform;

		public ComputeShader vectorscope;

		public ComputeShader multiScaleAODownsample1;

		public ComputeShader multiScaleAODownsample2;

		public ComputeShader multiScaleAORender;

		public ComputeShader multiScaleAOUpsample;

		public ComputeShader gaussianDownsample;

		public ComputeShaders Clone()
		{
			return (ComputeShaders)MemberwiseClone();
		}
	}

	[Serializable]
	public sealed class SMAALuts
	{
		public Texture2D area;

		public Texture2D search;
	}

	public Texture2D[] blueNoise64;

	public Texture2D[] blueNoise256;

	public SMAALuts smaaLuts;

	public Shaders shaders;

	public ComputeShaders computeShaders;
}
