using UnityEngine;

public class MB2_LODSoloChecker : MonoBehaviour
{
	private MB2_LOD lod;

	private void Start()
	{
		lod = GetComponent<MB2_LOD>();
	}

	private void Update()
	{
		if (!(lod == null))
		{
			lod.CheckIfLODsNeedToChange();
		}
	}
}
