using UnityEngine;

public class MB_SwitchBakedObjectsTexture : MonoBehaviour
{
	public MeshRenderer targetRenderer;

	public Material[] materials;

	public MB3_MeshBaker meshBaker;

	public void OnGUI()
	{
		GUILayout.Label("Press space to switch the material on one of the cubes. This scene reuses the Texture Bake Result from the SceneBasic example.");
	}

	public void Start()
	{
		meshBaker.AddDeleteGameObjects(meshBaker.GetObjectsToCombine().ToArray(), null, disableRendererInSource: true);
		meshBaker.Apply();
	}

	public void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Space))
		{
			return;
		}
		Material sharedMaterial = targetRenderer.sharedMaterial;
		int num = -1;
		for (int i = 0; i < materials.Length; i++)
		{
			if (materials[i] == sharedMaterial)
			{
				num = i;
			}
		}
		num++;
		if (num >= materials.Length)
		{
			num = 0;
		}
		if (num != -1)
		{
			targetRenderer.sharedMaterial = materials[num];
			Debug.Log("Updating Material to: " + targetRenderer.sharedMaterial);
			GameObject[] gos = new GameObject[1] { targetRenderer.gameObject };
			meshBaker.UpdateGameObjects(gos, recalcBounds: false, updateVertices: false, updateNormals: false, updateTangents: false, updateUV: true, updateUV1: false, updateUV2: false, updateColors: false, updateSkinningInfo: false);
			meshBaker.Apply();
		}
	}
}
