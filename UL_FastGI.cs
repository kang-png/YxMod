using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UPGEN Lighting/UPGEN Fast GI")]
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public sealed class UL_FastGI : MonoBehaviour
{
	[Range(1f, 10f)]
	public float expand = 3f;

	[Range(0f, 1f)]
	public float intensity = 0.1f;

	public static readonly List<UL_FastGI> all = new List<UL_FastGI>();

	private Light _light;

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
		if (_light == null)
		{
			_light = GetComponent<Light>();
			if (_light == null)
			{
				return;
			}
		}
		Vector3 position;
		switch (_light.type)
		{
		default:
			return;
		case LightType.Spot:
			position = base.transform.position + base.transform.forward;
			break;
		case LightType.Point:
			position = base.transform.position;
			break;
		}
		UL_Renderer.Add(position, _light.range * expand, _light.intensity * intensity * _light.color.linear);
	}
}
