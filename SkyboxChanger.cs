using UnityEngine;
using UnityEngine.UI;

public class SkyboxChanger : MonoBehaviour
{
	public Material[] Skyboxes;

	private Dropdown _dropdown;

	public void Awake()
	{
		_dropdown = GetComponent<Dropdown>();
	}

	public void ChangeSkybox()
	{
		RenderSettings.skybox = Skyboxes[_dropdown.value];
		RenderSettings.skybox.SetFloat("_Rotation", 0f);
	}
}
