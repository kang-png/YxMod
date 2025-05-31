using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class UL_GUI_Examples : MonoBehaviour
{
	public enum AnimationKind
	{
		AnimateX,
		AnimateZ
	}

	[Multiline]
	public string description;

	public string prevScene;

	public string nextScene;

	[Header("References")]
	public Transform target;

	public AnimationKind animationKind;

	public PostProcessProfile volumeProfile;

	private Vector3 _initialTargetPosition;

	private Vector3 _deltaTargetPosition;

	private float _nextUpdate;

	private int _fpsCounter;

	private int _fps;

	private void Start()
	{
		if ((bool)volumeProfile && volumeProfile.TryGetSettings<UPGEN_Lighting>(out var outSetting))
		{
			outSetting.intensity.value = 1f;
		}
		if (!(target == null))
		{
			_initialTargetPosition = target.transform.position;
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			_fpsCounter++;
			float unscaledTime = Time.unscaledTime;
			if (!(unscaledTime < _nextUpdate))
			{
				_nextUpdate = unscaledTime + 1f;
				_fps = _fpsCounter;
				_fpsCounter = 0;
			}
		}
	}

	private void LateUpdate()
	{
		if (target == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			switch (animationKind)
			{
			case AnimationKind.AnimateX:
				_deltaTargetPosition.x = 3f * Mathf.Sin(Time.unscaledTime);
				break;
			case AnimationKind.AnimateZ:
				_deltaTargetPosition.z = 7f * Mathf.Sin(Time.unscaledTime * 0.5f);
				break;
			}
		}
		if (_initialTargetPosition == Vector3.zero)
		{
			_initialTargetPosition = target.transform.position;
		}
		else
		{
			target.transform.position = _initialTargetPosition + _deltaTargetPosition;
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width - 200, 0f, 200f, Screen.height));
		if (_fps > 0)
		{
			GUILayout.Label($"FPS: <b>{_fps}</b>", GUI.skin.box);
		}
		GUILayout.EndArea();
		OnGUI_Tools();
		OnGUI_Scene();
	}

	private void OnGUI_Tools()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, 200f, Screen.height));
		GUILayout.FlexibleSpace();
		if ((bool)volumeProfile && volumeProfile.TryGetSettings<UPGEN_Lighting>(out var outSetting))
		{
			float value = outSetting.intensity.value;
			float num = UL_GUI_Utils.Slider("Intensity", value, 0f, 2f);
			if (num != value)
			{
				outSetting.intensity.value = num;
			}
		}
		if ((bool)target && Application.isPlaying)
		{
			if (animationKind != 0)
			{
				_deltaTargetPosition.x = UL_GUI_Utils.Slider("X", _deltaTargetPosition.x, -2f, 2f);
			}
			_deltaTargetPosition.y = UL_GUI_Utils.Slider("Y", _deltaTargetPosition.y, -2f, 2f);
			if (animationKind != AnimationKind.AnimateZ)
			{
				_deltaTargetPosition.z = UL_GUI_Utils.Slider("Z", _deltaTargetPosition.z, -3f, 3f);
			}
			UL_RayTracedGI component = target.GetComponent<UL_RayTracedGI>();
			if ((bool)component)
			{
				component.raysMatrixSize = (int)UL_GUI_Utils.Slider("Rays", component.raysMatrixSize, 2f, 15f);
			}
		}
		GUILayout.EndArea();
	}

	private void OnGUI_Scene()
	{
		if (!string.IsNullOrEmpty(description))
		{
			Scene activeScene = SceneManager.GetActiveScene();
			GUILayout.BeginArea(new Rect((float)(Screen.width - 500) * 0.5f, Screen.height - 164, 500f, 36f));
			GUILayout.BeginHorizontal();
			if (!string.IsNullOrEmpty(prevScene) && SceneManager.sceneCountInBuildSettings > 1 && Application.isPlaying && GUILayout.Button("<size=24><b>◄</b></size>", GUILayout.Width(32f), GUILayout.Height(34f)))
			{
				SceneManager.LoadScene(prevScene);
			}
			GUILayout.Label($"<size=24><b>{activeScene.name}</b></size>", GUI.skin.box);
			if (!string.IsNullOrEmpty(nextScene) && SceneManager.sceneCountInBuildSettings > 1 && Application.isPlaying && GUILayout.Button("<size=24><b>►</b></size>", GUILayout.Width(32f), GUILayout.Height(34f)))
			{
				SceneManager.LoadScene(nextScene);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.Box(new Rect((float)(Screen.width - 1200) * 0.5f, Screen.height - 100, 1200f, 60f), "<size=20>" + description + "</size>");
		}
	}
}
