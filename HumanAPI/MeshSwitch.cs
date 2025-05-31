using System.Collections;
using UnityEngine;

namespace HumanAPI;

public class MeshSwitch : Node
{
	public NodeInput input;

	public NodeOutput output;

	public GameObject[] objectsToSwitch;

	public int switchCount;

	private bool firstCall = true;

	private bool canHit = true;

	private void Start()
	{
	}

	public override void Process()
	{
		base.Process();
		if (!canHit)
		{
			return;
		}
		if (firstCall)
		{
			switchCount = 0;
			firstCall = false;
			for (int i = 1; i < objectsToSwitch.Length; i++)
			{
				objectsToSwitch[i].SetActive(value: false);
			}
			return;
		}
		if (objectsToSwitch[switchCount + 1] == objectsToSwitch[objectsToSwitch.Length - 1])
		{
			output.SetValue(1f);
			Reset();
		}
		if (switchCount < objectsToSwitch.Length)
		{
			objectsToSwitch[switchCount].SetActive(value: false);
			objectsToSwitch[switchCount + 1].SetActive(value: true);
			switchCount++;
		}
		StartCoroutine(Cooldown());
	}

	private IEnumerator Cooldown()
	{
		canHit = false;
		yield return new WaitForSeconds(4f);
		canHit = true;
	}

	public void Reset()
	{
		firstCall = true;
		switchCount = 0;
	}
}
