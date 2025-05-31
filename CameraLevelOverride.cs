using UnityEngine;

public class CameraLevelOverride : MonoBehaviour
{
	private Camera mainCamera;

	public float overrideCameraClipFarPlane = 300f;

	private float initialCameraClipFarPlane;

	private void Start()
	{
		mainCamera = Camera.main;
		initialCameraClipFarPlane = mainCamera.farClipPlane;
		mainCamera.farClipPlane = overrideCameraClipFarPlane;
	}

	private void OnDestroy()
	{
		Camera.main.farClipPlane = initialCameraClipFarPlane;
	}
}
