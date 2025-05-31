using System;
using UnityEngine;

namespace DigitalOpus.MB.Core;

public class MB2_Version
{
	public static int GetMajorVersion()
	{
		return 4;
	}

	public static bool GetActive(GameObject go)
	{
		return go.activeInHierarchy;
	}

	public static void SetActive(GameObject go, bool isActive)
	{
		go.SetActive(isActive);
	}

	public static void SetActiveRecursively(GameObject go, bool isActive)
	{
		go.SetActive(isActive);
	}

	public static UnityEngine.Object[] FindSceneObjectsOfType(Type t)
	{
		return UnityEngine.Object.FindObjectsOfType(t);
	}
}
