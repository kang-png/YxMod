using UnityEngine;

namespace HumanAPI.LightLevel;

public class ColorFilter : LightFilter
{
	public Color color;

	public override int priority => 0;

	public override void ApplyFilter(LightHitInfo info)
	{
		Color color = default(Color);
		color.r = Mathf.Min(info.source.color.r, this.color.r);
		color.g = Mathf.Min(info.source.color.g, this.color.g);
		color.b = Mathf.Min(info.source.color.b, this.color.b);
		Color color2 = color;
		if (consume.debugLog)
		{
			Debug.Log("Color");
		}
		foreach (LightBase output in info.outputs)
		{
			output.color = color2;
		}
	}
}
