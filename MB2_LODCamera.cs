using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Mesh Baker/LOD Camera")]
public class MB2_LODCamera : MonoBehaviour
{
	private void Awake()
	{
		MB2_LODManager mB2_LODManager = MB2_LODManager.Manager();
		if (mB2_LODManager != null)
		{
			mB2_LODManager.AddCamera(this);
		}
	}

	private void OnDestroy()
	{
		MB2_LODManager mB2_LODManager = MB2_LODManager.Manager();
		if (mB2_LODManager != null)
		{
			mB2_LODManager.RemoveCamera(this);
		}
	}
}
