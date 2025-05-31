using Multiplayer;
using UnityEngine;

public class DisableLightsWhenFarAway : MonoBehaviour
{
	public Light[] light;

	public bool[] isActive;

	private bool areLightsOn = true;

	public NetGame game;

	private Collider selfCollider;

	private void Start()
	{
		selfCollider = GetComponent<BoxCollider>();
		isActive = new bool[light.Length];
		for (int i = 0; i < light.Length; i++)
		{
			if (light[i] == null)
			{
				isActive[i] = false;
			}
			else
			{
				isActive[i] = light[i].enabled;
			}
		}
	}

	public void FixedUpdate()
	{
		if (game == null)
		{
			GameObject gameObject = GameObject.Find("NetGame");
			if (gameObject == null)
			{
				return;
			}
			game = gameObject.GetComponent<NetGame>();
			if (game == null)
			{
				return;
			}
		}
		bool flag = false;
		foreach (NetPlayer player in game.players)
		{
			if (!player.isLocalPlayer)
			{
				continue;
			}
			Transform child = player.gameObject.transform.GetChild(1);
			if (!(child == null))
			{
				Collider component = child.GetComponent<SphereCollider>();
				if (!(component == null) && Physics.ComputePenetration(component, child.position, child.rotation, selfCollider, base.transform.position, base.transform.rotation, out var _, out var _))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			EnableLights();
		}
		else
		{
			DisableLights();
		}
	}

	private void EnableLights()
	{
		if (areLightsOn)
		{
			return;
		}
		for (int i = 0; i < light.Length; i++)
		{
			if (!(light[i] == null) && isActive[i])
			{
				light[i].enabled = true;
			}
		}
		areLightsOn = true;
	}

	private void DisableLights()
	{
		if (!areLightsOn)
		{
			return;
		}
		for (int i = 0; i < light.Length; i++)
		{
			if (!(light[i] == null) && isActive[i])
			{
				light[i].enabled = false;
			}
		}
		areLightsOn = false;
	}
}
