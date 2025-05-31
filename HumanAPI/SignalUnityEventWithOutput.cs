using UnityEngine;
using UnityEngine.Events;

namespace HumanAPI;

[AddNodeMenuItem]
public class SignalUnityEventWithOutput : Node, IReset
{
	public NodeInput input;

	public bool onlyTriggerOnce;

	public UnityEvent triggerEvent;

	public UnityEvent resetEvent;

	public NodeOutput output;

	private bool hasTriggered;

	private float prevInput;

	private Collider currentCollider;

	public GameObject[] collidersToCheck;

	public GameObject[] fixedToShow;

	public GameObject[] lightsToShowWhenConnected;

	public override void Process()
	{
		if (!onlyTriggerOnce || !hasTriggered)
		{
			if (input.value >= 0.5f && prevInput < 0.5f)
			{
				triggerEvent.Invoke();
				hasTriggered = true;
				ShowClosestFixedPipe();
				output.SetValue(1f);
			}
			prevInput = input.value;
		}
	}

	private void ShowClosestFixedPipe()
	{
		float num = 9999f;
		int num2 = -1;
		int num3 = -1;
		GameObject[] array = collidersToCheck;
		foreach (GameObject gameObject in array)
		{
			num3++;
			float num4 = Vector3.Distance(gameObject.transform.position, base.gameObject.transform.position);
			if (num4 < num)
			{
				num2 = num3;
				num = num4;
			}
		}
		if (num2 >= 0)
		{
			MeshRenderer[] componentsInChildren = fixedToShow[num2].GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = true;
			}
			MeshCollider[] componentsInChildren2 = fixedToShow[num2].GetComponentsInChildren<MeshCollider>();
			foreach (MeshCollider meshCollider in componentsInChildren2)
			{
				meshCollider.enabled = true;
			}
			lightsToShowWhenConnected[num2].GetComponentInChildren<AudioSource>().Play();
			lightsToShowWhenConnected[num2].GetComponentInChildren<Light>().enabled = true;
			lightsToShowWhenConnected[num2].GetComponentInChildren<NodeGraph>().inputs[0].inputSocket.SetValue(1f);
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		GameObject[] array = fixedToShow;
		foreach (GameObject gameObject in array)
		{
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
			MeshCollider[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshCollider>();
			foreach (MeshCollider meshCollider in componentsInChildren2)
			{
				meshCollider.enabled = false;
			}
		}
		GameObject[] array2 = lightsToShowWhenConnected;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.GetComponentInChildren<AudioSource>().Stop();
			gameObject2.GetComponentInChildren<Light>().enabled = false;
			gameObject2.GetComponentInChildren<NodeGraph>().inputs[0].inputSocket.SetValue(0f);
		}
		prevInput = 0f;
		hasTriggered = false;
		output.SetValue(0f);
		resetEvent.Invoke();
	}
}
