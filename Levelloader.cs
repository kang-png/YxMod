using UnityEngine;

public class Levelloader : MonoBehaviour
{
	private void OnGUI()
	{
		GUI.Box(new Rect(10f, 10f, 140f, 310f), "Loader Menu");
		if (GUI.Button(new Rect(20f, 40f, 120f, 30f), "Blue Sky"))
		{
			Application.LoadLevel(0);
		}
		if (GUI.Button(new Rect(20f, 70f, 120f, 30f), "Blue Sky 02"))
		{
			Application.LoadLevel(1);
		}
		if (GUI.Button(new Rect(20f, 100f, 120f, 30f), "Bright Morning"))
		{
			Application.LoadLevel(2);
		}
		if (GUI.Button(new Rect(20f, 130f, 120f, 30f), "Golden Horizon"))
		{
			Application.LoadLevel(3);
		}
		if (GUI.Button(new Rect(20f, 160f, 120f, 30f), "Moody Sunrise"))
		{
			Application.LoadLevel(4);
		}
		if (GUI.Button(new Rect(20f, 190f, 120f, 30f), "Moody Sunrise 02"))
		{
			Application.LoadLevel(5);
		}
		if (GUI.Button(new Rect(20f, 220f, 120f, 30f), "Pink Sunset"))
		{
			Application.LoadLevel(6);
		}
		if (GUI.Button(new Rect(20f, 250f, 120f, 30f), "Pink Sunset 02"))
		{
			Application.LoadLevel(7);
		}
		if (GUI.Button(new Rect(20f, 280f, 120f, 30f), "Stormy Sky"))
		{
			Application.LoadLevel(8);
		}
	}
}
