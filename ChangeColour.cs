using UnityEngine;

public class ChangeColour : MonoBehaviour
{
	[Tooltip("The material that needs it colour changed.")]
	public Material materialToChange;

	public Color newColor;

	private Material materialInstance;

	private Color originalColor;

	private void Start()
	{
		if (!(materialToChange != null))
		{
			return;
		}
		Material[] materials = GetComponent<MeshRenderer>().materials;
		for (int i = 0; i < materials.Length; i++)
		{
			string text = materials[i].name.Replace(" (Instance)", string.Empty);
			if (text == materialToChange.name)
			{
				materialInstance = materials[i];
				originalColor = materialInstance.color;
			}
		}
	}

	public void ChangeColourMat()
	{
		if (materialInstance != null)
		{
			materialInstance.color = newColor;
		}
	}

	public void Reset()
	{
		if (materialInstance != null)
		{
			materialInstance.color = originalColor;
		}
	}
}
