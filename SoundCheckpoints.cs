using System.Collections.Generic;
using UnityEngine;

public class SoundCheckpoints : MonoBehaviour
{
	[SerializeField]
	private Transform root;

	private List<Transform> soundCheckpoints = new List<Transform>();

	private int currentCheckpoint = -1;

	private void Start()
	{
		if (root == null)
		{
			return;
		}
		foreach (Transform item in root.gameObject.transform)
		{
			soundCheckpoints.Add(item);
		}
	}

	public void GoPreviousCheckpoint()
	{
		if (soundCheckpoints.Count < 1)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Player(Clone)");
		if (!(gameObject == null))
		{
			Transform transform = gameObject.transform.Find("Ball");
			if (!(transform == null) && currentCheckpoint > 0)
			{
				transform.transform.position = soundCheckpoints[currentCheckpoint - 1].position;
				transform.transform.rotation = soundCheckpoints[currentCheckpoint - 1].rotation;
				currentCheckpoint--;
			}
		}
	}

	public void GoNextCheckpoint()
	{
		if (soundCheckpoints.Count < 1)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Player(Clone)");
		if (!(gameObject == null))
		{
			Transform transform = gameObject.transform.Find("Ball");
			if (!(transform == null) && currentCheckpoint + 1 < soundCheckpoints.Count)
			{
				transform.transform.position = soundCheckpoints[currentCheckpoint + 1].position;
				transform.transform.rotation = soundCheckpoints[currentCheckpoint + 1].rotation;
				currentCheckpoint++;
			}
		}
	}
}
