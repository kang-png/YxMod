using System;
using System.Collections.Generic;
using UnityEngine;

public class MetalGratesController : MonoBehaviour, IReset
{
	public List<int> rockTypeMax = new List<int>();

	[NonSerialized]
	public List<int> rockCounter = new List<int>();

	private int totalMaxRocks;

	private void Start()
	{
		totalMaxRocks = 0;
		for (int i = 0; i < rockTypeMax.Count; i++)
		{
			rockCounter.Add(0);
			totalMaxRocks += rockTypeMax[i];
		}
	}

	public void AddRock(int rockTypeIndex)
	{
		rockCounter[rockTypeIndex]++;
		bool flag = true;
		for (int i = 0; i < rockCounter.Count; i++)
		{
			if (rockCounter[i] < rockTypeMax[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			OpenGrates();
		}
	}

	public void OpenGrates()
	{
		MetalGrate[] componentsInChildren = GetComponentsInChildren<MetalGrate>();
		foreach (MetalGrate metalGrate in componentsInChildren)
		{
			metalGrate.OpenGrate();
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		for (int i = 0; i < rockCounter.Count; i++)
		{
			rockCounter[i] = 0;
		}
		MetalGrate[] componentsInChildren = GetComponentsInChildren<MetalGrate>();
		foreach (MetalGrate metalGrate in componentsInChildren)
		{
			metalGrate.ResetGrate();
		}
	}
}
