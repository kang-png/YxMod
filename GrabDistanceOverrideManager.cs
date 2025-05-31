using System.Collections.Generic;
using UnityEngine;

public static class GrabDistanceOverrideManager
{
	private static Dictionary<Transform, GrabDistanceOverride> overridesLookup = new Dictionary<Transform, GrabDistanceOverride>();

	public static void Register(GrabDistanceOverride objectToRegister)
	{
		overridesLookup[objectToRegister.transform] = objectToRegister;
	}

	public static void Unregister(Transform objectToRemove)
	{
		overridesLookup.Remove(objectToRemove);
	}

	public static float? GetValueFor(Transform transform)
	{
		if (overridesLookup.ContainsKey(transform))
		{
			return overridesLookup[transform].GrabDistance;
		}
		return null;
	}
}
