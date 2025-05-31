using System.Runtime.CompilerServices;
using UnityEngine;

public class GrabDistanceOverride : MonoBehaviour
{
	[SerializeField]
	private float grabDistance = 0.5f;

	public float GrabDistance
	{
		[CompilerGenerated]
		get
		{
			return grabDistance;
		}
	}

	private void Awake()
	{
		GrabDistanceOverrideManager.Register(this);
	}

	private void OnDestroy()
	{
		GrabDistanceOverrideManager.Unregister(base.transform);
	}
}
