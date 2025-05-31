using UnityEngine;

public class IgnoreChildCollision : MonoBehaviour
{
	[SerializeField]
	private bool LateInit;

	private void Awake()
	{
		if (!LateInit)
		{
			Init();
		}
	}

	private void Start()
	{
		if (LateInit)
		{
			Init();
		}
	}

	private void Init()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = i; j < componentsInChildren.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[i], componentsInChildren[j]);
			}
		}
	}
}
