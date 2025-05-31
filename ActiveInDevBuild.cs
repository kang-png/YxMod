using UnityEngine;

public class ActiveInDevBuild : MonoBehaviour
{
	public bool activeInDevBuild = true;

	public void Awake()
	{
		base.gameObject.SetActive(activeInDevBuild && Debug.isDebugBuild);
	}
}
