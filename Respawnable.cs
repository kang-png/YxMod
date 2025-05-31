using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
	[Tooltip("Use this to respawn to a specific place in stead of its original start position.")]
	public bool respawnToSpecificLocation;

	public Transform resetPos;

	[Space]
	private Vector3 startPos;

	private Quaternion startRot;

	public float despawnHeight = -50f;

	public float respawnHeight;

	public bool resetRotation;

	private static Coroutine update;

	private static List<Respawnable> all = new List<Respawnable>();

	private void OnDisable()
	{
		all.Remove(this);
	}

	private void OnEnable()
	{
		all.Add(this);
		if (update == null)
		{
			update = Coroutines.StartGlobalCoroutine(ProcessUpdates());
		}
	}

	private void Awake()
	{
		if (!respawnToSpecificLocation)
		{
			startPos = base.transform.position;
			startRot = base.transform.rotation;
		}
	}

	public static IEnumerator ProcessUpdates()
	{
		int throttle = 0;
		while (all.Count > 0)
		{
			yield return null;
			for (int i = 0; i < all.Count; i++)
			{
				if (throttle++ == 50)
				{
					throttle = 0;
					yield return null;
				}
				if (i < all.Count)
				{
					all[i].UpdateInternal();
				}
			}
		}
		Coroutines.StopGlobalCoroutine(update);
		update = null;
	}

	private void UpdateInternal()
	{
		if (base.transform.position.y < despawnHeight)
		{
			Respawn();
		}
	}

	public void Respawn()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient || GrabManager.IsGrabbedAny(base.gameObject))
		{
			return;
		}
		RestartableRigid component = GetComponent<RestartableRigid>();
		if (component != null)
		{
			component.Reset(Vector3.up * respawnHeight);
			return;
		}
		if (!respawnToSpecificLocation)
		{
			base.transform.position = startPos + Vector3.up * respawnHeight;
			base.transform.rotation = startRot;
		}
		else
		{
			base.transform.position = resetPos.position;
			if (resetRotation)
			{
				base.transform.rotation = resetPos.rotation;
			}
		}
		Rigidbody component2 = GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.velocity = Vector3.zero;
			component2.angularVelocity = Vector3.zero;
		}
	}
}
