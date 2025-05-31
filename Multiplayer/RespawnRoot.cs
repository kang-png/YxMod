using UnityEngine;

namespace Multiplayer;

public class RespawnRoot : MonoBehaviour
{
	public void Respawn(Vector3 offset)
	{
		IRespawnable[] componentsInChildren = base.transform.GetComponentsInChildren<IRespawnable>();
		foreach (IRespawnable respawnable in componentsInChildren)
		{
			respawnable.Respawn(offset);
		}
	}
}
