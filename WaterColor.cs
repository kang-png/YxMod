using UnityEngine;

public class WaterColor : MonoBehaviour
{
	public Color color;

	private void Start()
	{
		CaveRender component = GameObject.Find("GameCamera(Clone)").GetComponent<CaveRender>();
		if ((bool)component)
		{
			component.waterFogColor = color;
		}
	}
}
