using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UPGEN Lighting/UPGEN Fast Light")]
[ExecuteInEditMode]
public sealed class UL_FastLight : MonoBehaviour
{
	public Color color = Color.white;

	public float intensity = 1f;

	public float range = 10f;

	public static readonly List<UL_FastLight> all = new List<UL_FastLight>();

	private void OnEnable()
	{
		all.Add(this);
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	internal void GenerateRenderData()
	{
		UL_Renderer.Add(base.transform.position, range, intensity * intensity * color.linear);
	}
}
