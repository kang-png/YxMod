using System;
using Multiplayer;
using UnityEngine;

public class MagneticPoint : MonoBehaviour
{
	public float magnetism;

	public float range = 1f;

	public Vector3 magneticPointOffset;

	[Range(0f, 360f)]
	public float angle = 360f;

	[NonSerialized]
	public MagneticBody magneticBody;

	[NonSerialized]
	public bool magnetActive;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		if (magneticBody == null)
		{
			Debug.LogError("Magnetics need a parent MagneticBody to function: " + base.gameObject.name, this);
			return;
		}
		Vector3 vector = base.transform.TransformPoint(magneticPointOffset);
		magnetActive = false;
		if (magnetism == 0f || !magneticBody.magnetActive)
		{
			return;
		}
		foreach (MagneticPoint nearbyMagnetic in magneticBody.NearbyMagnetics)
		{
			if (magneticBody == nearbyMagnetic.magneticBody)
			{
				continue;
			}
			Vector3 vector2 = nearbyMagnetic.transform.TransformPoint(nearbyMagnetic.magneticPointOffset);
			Vector3 vector3 = vector - vector2;
			if (vector3.magnitude > range)
			{
				continue;
			}
			Vector3 normalized = vector3.normalized;
			float value = Math.Abs(magnetism) + Math.Abs(nearbyMagnetic.magnetism);
			vector3 = normalized * Math.Abs(value) * (1f - vector3.magnitude / range);
			if (!((double)angle < 360.0) || !(57.29578f * Mathf.Acos(Vector3.Dot(-normalized, base.transform.forward)) > angle / 2f))
			{
				magnetActive = true;
				bool flag = false;
				if (Math.Sign(magnetism) == Math.Sign(nearbyMagnetic.magnetism))
				{
					flag = true;
				}
				if (flag)
				{
					nearbyMagnetic.magneticBody.Body.AddForceAtPosition(-vector3, vector2);
				}
				else if (!magneticBody.disableOnContact || !magneticBody.IsInContact(nearbyMagnetic.magneticBody.Body))
				{
					nearbyMagnetic.magneticBody.Body.AddForceAtPosition(vector3, vector2);
				}
			}
		}
	}
}
