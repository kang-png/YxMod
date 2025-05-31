using UnityEngine;

public class MB2_LODManagerGUI : MonoBehaviour
{
	private string text;

	private void OnGUI()
	{
		MB2_LODManager component = GetComponent<MB2_LODManager>();
		if (GUI.Button(new Rect(0f, 0f, 100f, 20f), "LOD Stats"))
		{
			if (component != null)
			{
				text = component.GetStats();
			}
			else
			{
				text = "Could not find LODManager";
			}
			Debug.Log(text);
		}
		GUI.Label(new Rect(0f, 20f, 300f, 600f), text);
	}
}
