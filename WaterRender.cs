using UnityEngine;

public class WaterRender : MonoBehaviour
{
	public float scale = 1f;

	[Space]
	public bool useTimeSinceLevelLoad;

	private Material material;

	private void Start()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			material = component.material;
			component.sharedMaterial = material;
		}
		SkinnedMeshRenderer component2 = GetComponent<SkinnedMeshRenderer>();
		if (component2 != null)
		{
			material = component2.material;
			component2.sharedMaterial = material;
		}
		material.SetFloat("_Scale", scale);
	}

	private void LateUpdate()
	{
		float value = ((!useTimeSinceLevelLoad) ? ReplayRecorder.time : Time.timeSinceLevelLoad);
		material.SetFloat("_Time2", value);
	}
}
