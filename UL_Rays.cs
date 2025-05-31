using System.Collections.Generic;
using UnityEngine;

public static class UL_Rays
{
	public class Ray
	{
		public bool hit;

		public Vector3 interpolatedPosition;

		public Vector3 position;

		public Color interpolatedColor;

		public Color color;

		private const float HIT_OFFSET = 1f;

		private const float HIT_OFFSET_MAX = 2f;

		private static readonly int _propertyColorId = Shader.PropertyToID("_Color");

		private static readonly int _propertyMainTexId = Shader.PropertyToID("_MainTex");

		private static readonly Dictionary<Renderer, Color> _cachedRendererColor = new Dictionary<Renderer, Color>();

		private static RenderTexture _renderTexture;

		private static Texture2D _tempTexture;

		public void Point(Vector3 pt, Vector3 dir)
		{
			hit = true;
			position = pt + dir;
			color = Color.white;
		}

		public void Trace(Vector3 pt, Vector3 dir, float range, Color lightColor, int layersToHit)
		{
			bool flag = hit;
			hit = false;
			if (!Physics.Raycast(pt, dir, out var hitInfo, range * 0.9f, layersToHit))
			{
				return;
			}
			Vector3 point = hitInfo.point;
			Vector3 normal = hitInfo.normal;
			Vector3 origin = point + normal * 0.001f;
			float distance = hitInfo.distance;
			if (distance < 0.1f)
			{
				return;
			}
			position = ((!Physics.Raycast(origin, normal, out var hitInfo2, 2f, layersToHit)) ? (point + 1f * normal) : (point + 0.5f * hitInfo2.distance * normal));
			if (Physics.CheckSphere(position, 0.2f, layersToHit))
			{
				position = ((!Physics.Raycast(origin, -dir, out hitInfo2, 2f, layersToHit)) ? (point - 1f * dir) : (point - 0.5f * hitInfo2.distance * dir));
				if (Physics.CheckSphere(position, 0.1f, layersToHit))
				{
					return;
				}
			}
			float num = distance / range;
			num *= num;
			num = 1f - num;
			color = GetRayHitColor(hitInfo.transform.GetComponent<Renderer>());
			color.r *= color.r * color.r * lightColor.r * num;
			color.g *= color.g * color.g * lightColor.g * num;
			color.b *= color.b * color.b * lightColor.b * num;
			hit = true;
			if (!flag)
			{
				interpolatedColor = color;
				interpolatedPosition = position;
			}
		}

		private static Color GetRayHitColor(Renderer r)
		{
			if (r == null)
			{
				return Color.white;
			}
			if (_cachedRendererColor.TryGetValue(r, out var value))
			{
				return value;
			}
			Material sharedMaterial = r.sharedMaterial;
			if (sharedMaterial == null)
			{
				_cachedRendererColor.Add(r, Color.white);
				return Color.white;
			}
			value = ((!sharedMaterial.HasProperty(_propertyColorId)) ? Color.white : sharedMaterial.GetColor(_propertyColorId));
			if (!sharedMaterial.HasProperty(_propertyMainTexId))
			{
				_cachedRendererColor.Add(r, value);
				return value;
			}
			Texture mainTexture = sharedMaterial.mainTexture;
			if (mainTexture == null)
			{
				_cachedRendererColor.Add(r, value);
				return value;
			}
			if (_renderTexture == null)
			{
				_renderTexture = new RenderTexture(1, 1, 0);
			}
			if (_tempTexture == null)
			{
				_tempTexture = new Texture2D(1, 1);
			}
			Graphics.Blit(mainTexture, _renderTexture);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = _renderTexture;
			_tempTexture.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0, recalculateMipMaps: false);
			_tempTexture.Apply();
			RenderTexture.active = active;
			value *= _tempTexture.GetPixel(0, 0);
			_cachedRendererColor.Add(r, value);
			return value;
		}
	}

	public static readonly Ray[] EMPTY_RAYS = new Ray[0];

	public static Ray[] GenerateRays(int count)
	{
		Ray[] array = new Ray[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new Ray();
		}
		return array;
	}
}
