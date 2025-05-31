using System.Runtime.CompilerServices;
using HumanAPI;
using UnityEngine;

public class SignalRelease : Node
{
	public NodeInput release;

	[Tooltip("The object to be force released when input >= 0.5")]
	public GameObject toRelease;

	private float prevRelease;

	private bool ReleaseThisFrame
	{
		[CompilerGenerated]
		get
		{
			return release.value >= 0.5f && prevRelease < 0.5f;
		}
	}

	private void OnValidate()
	{
		if (toRelease == null)
		{
			toRelease = base.gameObject;
		}
	}

	public override void Process()
	{
		if (ReleaseThisFrame)
		{
			GrabManager.Release(toRelease);
		}
		prevRelease = release.value;
	}
}
