using UnityEngine;

public class MoveCluster : MonoBehaviour
{
	public Transform world;

	private void LateUpdate()
	{
		if (Time.frameCount % 300 == 0)
		{
			MB2_LODManager mB2_LODManager = (MB2_LODManager)Object.FindObjectOfType(typeof(MB2_LODManager));
			Vector3 vector = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
			Vector3 vector2 = vector - world.position;
			world.position += vector2;
			mB2_LODManager.TranslateWorld(vector2);
			Debug.Log("Moving World To " + vector);
		}
	}
}
