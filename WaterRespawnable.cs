using System.Runtime.CompilerServices;
using Multiplayer;
using UnityEngine;

public class WaterRespawnable : MonoBehaviour
{
	[SerializeField]
	private NetBody bodyToRespawn;

	public NetBody BodyToRespawn
	{
		[CompilerGenerated]
		get
		{
			return bodyToRespawn;
		}
	}
}
