using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing;

public static class TextureFormatUtilities
{
	private static Dictionary<int, RenderTextureFormat> s_FormatAliasMap;

	private static Dictionary<int, bool> s_SupportedRenderTextureFormats;

	private static Dictionary<int, bool> s_SupportedTextureFormats;

	static TextureFormatUtilities()
	{
		s_FormatAliasMap = new Dictionary<int, RenderTextureFormat>
		{
			{
				1,
				RenderTextureFormat.ARGB32
			},
			{
				2,
				RenderTextureFormat.ARGB4444
			},
			{
				3,
				RenderTextureFormat.ARGB32
			},
			{
				4,
				RenderTextureFormat.ARGB32
			},
			{
				5,
				RenderTextureFormat.ARGB32
			},
			{
				7,
				RenderTextureFormat.RGB565
			},
			{
				9,
				RenderTextureFormat.RHalf
			},
			{
				10,
				RenderTextureFormat.ARGB32
			},
			{
				12,
				RenderTextureFormat.ARGB32
			},
			{
				13,
				RenderTextureFormat.ARGB4444
			},
			{
				14,
				RenderTextureFormat.ARGB32
			},
			{
				15,
				RenderTextureFormat.RHalf
			},
			{
				16,
				RenderTextureFormat.RGHalf
			},
			{
				17,
				RenderTextureFormat.ARGBHalf
			},
			{
				18,
				RenderTextureFormat.RFloat
			},
			{
				19,
				RenderTextureFormat.RGFloat
			},
			{
				20,
				RenderTextureFormat.ARGBFloat
			},
			{
				22,
				RenderTextureFormat.ARGBHalf
			},
			{
				26,
				RenderTextureFormat.R8
			},
			{
				27,
				RenderTextureFormat.RGHalf
			},
			{
				24,
				RenderTextureFormat.ARGBHalf
			},
			{
				25,
				RenderTextureFormat.ARGB32
			},
			{
				28,
				RenderTextureFormat.ARGB32
			},
			{
				29,
				RenderTextureFormat.ARGB32
			},
			{
				30,
				RenderTextureFormat.ARGB32
			},
			{
				31,
				RenderTextureFormat.ARGB32
			},
			{
				32,
				RenderTextureFormat.ARGB32
			},
			{
				33,
				RenderTextureFormat.ARGB32
			},
			{
				35,
				RenderTextureFormat.ARGB32
			},
			{
				36,
				RenderTextureFormat.ARGB32
			},
			{
				34,
				RenderTextureFormat.ARGB32
			},
			{
				45,
				RenderTextureFormat.ARGB32
			},
			{
				46,
				RenderTextureFormat.ARGB32
			},
			{
				47,
				RenderTextureFormat.ARGB32
			},
			{
				48,
				RenderTextureFormat.ARGB32
			},
			{
				49,
				RenderTextureFormat.ARGB32
			},
			{
				50,
				RenderTextureFormat.ARGB32
			},
			{
				51,
				RenderTextureFormat.ARGB32
			},
			{
				52,
				RenderTextureFormat.ARGB32
			},
			{
				53,
				RenderTextureFormat.ARGB32
			},
			{
				54,
				RenderTextureFormat.ARGB32
			},
			{
				55,
				RenderTextureFormat.ARGB32
			},
			{
				56,
				RenderTextureFormat.ARGB32
			},
			{
				57,
				RenderTextureFormat.ARGB32
			},
			{
				58,
				RenderTextureFormat.ARGB32
			},
			{
				59,
				RenderTextureFormat.ARGB32
			},
			{
				60,
				RenderTextureFormat.ARGB32
			},
			{
				61,
				RenderTextureFormat.ARGB32
			}
		};
		s_SupportedRenderTextureFormats = new Dictionary<int, bool>();
		Array values = Enum.GetValues(typeof(RenderTextureFormat));
		foreach (object item in values)
		{
			if ((int)item >= 0 && !IsObsolete(item))
			{
				bool value = SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)item);
				s_SupportedRenderTextureFormats[(int)item] = value;
			}
		}
		s_SupportedTextureFormats = new Dictionary<int, bool>();
		Array values2 = Enum.GetValues(typeof(TextureFormat));
		foreach (object item2 in values2)
		{
			if ((int)item2 >= 0 && !IsObsolete(item2))
			{
				bool value2 = SystemInfo.SupportsTextureFormat((TextureFormat)item2);
				s_SupportedTextureFormats[(int)item2] = value2;
			}
		}
	}

	private static bool IsObsolete(object value)
	{
		FieldInfo field = value.GetType().GetField(value.ToString());
		ObsoleteAttribute[] array = (ObsoleteAttribute[])field.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);
		return array != null && array.Length > 0;
	}

	public static RenderTextureFormat GetUncompressedRenderTextureFormat(Texture texture)
	{
		if (texture is RenderTexture)
		{
			return (texture as RenderTexture).format;
		}
		if (texture is Texture2D)
		{
			TextureFormat format = ((Texture2D)texture).format;
			if (!s_FormatAliasMap.TryGetValue((int)format, out var value))
			{
				throw new NotSupportedException("Texture format not supported");
			}
			return value;
		}
		return RenderTextureFormat.Default;
	}

	internal static bool IsSupported(this RenderTextureFormat format)
	{
		s_SupportedRenderTextureFormats.TryGetValue((int)format, out var value);
		return value;
	}

	internal static bool IsSupported(this TextureFormat format)
	{
		s_SupportedTextureFormats.TryGetValue((int)format, out var value);
		return value;
	}
}
