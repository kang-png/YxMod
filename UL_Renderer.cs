using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UL_Renderer
{
	private class Light
	{
		public float score;

		public Vector4 position;

		public Vector4 color;
	}

	private const int MAX_LIGHTS_COUNT = 128;

	private static int _lightsCount;

	private static readonly Vector4[] _lightsPositions = new Vector4[128];

	private static readonly Color[] _lightsColors = new Color[128];

	private static readonly Stack<Light> _lightsPool = new Stack<Light>();

	private static readonly List<Light> _lightsUsed = new List<Light>();

	private static Vector3 _cameraPosition;

	private static Vector3 _cameraForward;

	private const float _cameraFOVAngle = 50f;

	private static readonly float _cameraFOVCos = Mathf.Cos(0.87266463f);

	private static readonly float _cameraFOVSin = Mathf.Sin(0.87266463f);

	private static Texture2D TestTex;

	private static Color[] colors;

	public static bool HasLightsToRender
	{
		[CompilerGenerated]
		get
		{
			return UL_FastLight.all.Count > 0 || UL_FastGI.all.Count > 0 || UL_RayTracedGI.all.Count > 0;
		}
	}

	public static int RenderedLightsCount
	{
		[CompilerGenerated]
		get
		{
			return _lightsCount;
		}
	}

	public static int MaxRenderingLightsCount
	{
		[CompilerGenerated]
		get
		{
			return 128;
		}
	}

	private static int ScoresComparison(Light x, Light y)
	{
		if (x.score < y.score)
		{
			return -1;
		}
		if (x.score > y.score)
		{
			return 1;
		}
		return 0;
	}

	public static void Add(Vector3 position, float range, Color color)
	{
		if (color.maxColorComponent < 0.001f)
		{
			return;
		}
		Vector3 vector = position - _cameraPosition;
		float num = Vector3.Dot(vector, _cameraForward);
		float num2 = _cameraFOVCos * Mathf.Sqrt(Vector3.Dot(vector, vector) - num * num) - num * _cameraFOVSin;
		if (!(num2 >= 0f) || !(Mathf.Abs(num2) >= range))
		{
			Light light = ((_lightsPool.Count <= 0) ? new Light() : _lightsPool.Pop());
			light.score = vector.sqrMagnitude - (2f - num) * range;
			light.position.x = position.x;
			light.position.y = position.y;
			light.position.z = position.z;
			light.position.w = range;
			light.color.x = color.r;
			light.color.y = color.g;
			light.color.z = color.b;
			_lightsUsed.Add(light);
			if (color.r < 0f || color.g < 0f || color.b < 0f)
			{
				Debug.LogError($"Add color: {color}");
			}
		}
	}

	public static void SetupForCamera(Camera camera, MaterialPropertyBlock properties)
	{
		CreateTexture();
		Transform transform = camera.transform;
		_cameraPosition = transform.position;
		_cameraForward = transform.forward;
		for (int num = UL_FastLight.all.Count - 1; num >= 0; num--)
		{
			UL_FastLight.all[num].GenerateRenderData();
		}
		for (int num2 = UL_FastGI.all.Count - 1; num2 >= 0; num2--)
		{
			UL_FastGI.all[num2].GenerateRenderData();
		}
		for (int num3 = UL_RayTracedGI.all.Count - 1; num3 >= 0; num3--)
		{
			UL_RayTracedGI.all[num3].GenerateRenderData();
		}
		_lightsCount = Mathf.Min(_lightsUsed.Count, 128);
		_lightsUsed.Sort(ScoresComparison);
		for (int num4 = _lightsCount - 1; num4 >= 0; num4--)
		{
			Light light = _lightsUsed[num4];
			ref Vector4 reference = ref _lightsPositions[num4];
			reference = light.position;
			ref Color reference2 = ref _lightsColors[num4];
			reference2 = light.color;
			ref Color reference3 = ref colors[num4];
			reference3 = light.color;
			if (light.color.x < 0f || light.color.y < 0f || light.color.z < 0f || light.color.w < 0f)
			{
				Debug.LogError($"SetupForCamera: {light.color}");
			}
			_lightsPool.Push(light);
		}
		for (int num5 = _lightsUsed.Count - 1; num5 >= _lightsCount; num5--)
		{
			_lightsPool.Push(_lightsUsed[num5]);
		}
		_lightsUsed.Clear();
		properties.SetFloat("_LightsCount", _lightsCount);
		properties.SetVectorArray("_LightsPositions", _lightsPositions);
		TestTex.SetPixels(colors);
		TestTex.Apply();
		if (camera.stereoEnabled)
		{
			Matrix4x4 inverse = GL.GetGPUProjectionMatrix(camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left), renderIntoTexture: true).inverse;
			inverse[1, 1] *= -1f;
			properties.SetMatrix("_LeftViewFromScreen", inverse);
			properties.SetMatrix("_LeftWorldFromView", camera.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse);
			Matrix4x4 inverse2 = GL.GetGPUProjectionMatrix(camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right), renderIntoTexture: true).inverse;
			inverse2[1, 1] *= -1f;
			properties.SetMatrix("_RightViewFromScreen", inverse2);
			properties.SetMatrix("_RightWorldFromView", camera.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse);
		}
		else
		{
			Matrix4x4 inverse3 = GL.GetGPUProjectionMatrix(camera.projectionMatrix, renderIntoTexture: true).inverse;
			inverse3[1, 1] *= -1f;
			properties.SetMatrix("_LeftViewFromScreen", inverse3);
			properties.SetMatrix("_LeftWorldFromView", camera.cameraToWorldMatrix);
		}
	}

	private static void CreateTexture()
	{
		if (!(TestTex != null))
		{
			bool mipmap = false;
			bool linear = true;
			TextureFormat format = TextureFormat.RGBAFloat;
			TestTex = new Texture2D(128, 1, format, mipmap, linear);
			TestTex.filterMode = FilterMode.Point;
			TestTex.wrapMode = TextureWrapMode.Clamp;
			colors = new Color[128];
			for (int i = 0; i < 128; i++)
			{
				ref Color reference = ref colors[i];
				reference = Color.red;
			}
			TestTex.SetPixels(colors);
			TestTex.Apply();
			Shader.SetGlobalTexture("_Colors", TestTex);
		}
	}
}
