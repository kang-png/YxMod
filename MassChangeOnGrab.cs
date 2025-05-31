using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class MassChangeOnGrab : MonoBehaviour, IGrabbable
{
	public float massMultiplyOnGrab = 1f;

	public Rigidbody rigid;

	[Tooltip("If true, all children with a RigidBody will have the same mass change when this is grabbed")]
	public bool affectChildren;

	private float mass;

	private Dictionary<Rigidbody, float> childRigidMass = new Dictionary<Rigidbody, float>();

	private void OnEnable()
	{
		if (NetGame.isClient)
		{
			return;
		}
		if (rigid == null)
		{
			rigid = GetComponent<Rigidbody>();
		}
		mass = rigid.mass;
		if (affectChildren)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>(includeInactive: true);
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				childRigidMass[rigidbody] = rigidbody.mass;
			}
		}
	}

	public void OnGrab()
	{
		if (NetGame.isClient)
		{
			return;
		}
		rigid.mass = mass * massMultiplyOnGrab;
		if (!affectChildren)
		{
			return;
		}
		foreach (Rigidbody key in childRigidMass.Keys)
		{
			key.mass = childRigidMass[key] * massMultiplyOnGrab;
		}
	}

	public void OnRelease()
	{
		if (NetGame.isClient)
		{
			return;
		}
		rigid.mass = mass;
		if (!affectChildren)
		{
			return;
		}
		foreach (Rigidbody key in childRigidMass.Keys)
		{
			key.mass = childRigidMass[key];
		}
	}
}
