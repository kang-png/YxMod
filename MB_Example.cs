using UnityEngine;

public class MB_Example : MonoBehaviour
{
	public MB3_MeshBaker meshbaker;

	public GameObject[] objsToCombine;

	private void Start()
	{
		meshbaker.AddDeleteGameObjects(objsToCombine, null, disableRendererInSource: true);
		meshbaker.Apply();
	}

	private void LateUpdate()
	{
		meshbaker.UpdateGameObjects(objsToCombine);
		meshbaker.Apply(triangles: false, vertices: true, normals: true, tangents: true, uvs: false, uv2: false, uv3: false, uv4: false, colors: false);
	}

	private void OnGUI()
	{
		GUILayout.Label("Dynamically updates the vertices, normals and tangents in combined mesh every frame.\nThis is similar to dynamic batching. It is not recommended to do this every frame.\nAlso consider baking the mesh renderer objects into a skinned mesh renderer\nThe skinned mesh approach is faster for objects that need to move independently of each other every frame.");
	}
}
