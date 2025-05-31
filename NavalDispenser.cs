using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavalDispenser : MonoBehaviour, IReset
{
	private enum DispenserState
	{
		ReadyToDispense,
		CoolingDown
	}

	public bool debug;

	public Transform debrisPool;

	public float cooldownInterval = 3f;

	public int amountPerSpawn = 1;

	public AudioSource leverAudioSource;

	private SignalBroadcastAngle lever;

	private float cooldownTimer;

	private List<GameObject> debrisInstances;

	private List<GameObject> debrisActiveInstances;

	private DispenserState dispenserState;

	private void SpawnDebris(int amount = 1)
	{
		Action action = delegate
		{
			GameObject gameObject = debrisInstances.Find((GameObject rb) => !rb.gameObject.activeInHierarchy);
			if (!(gameObject == null))
			{
				SetupInstance(gameObject);
			}
		};
		if (dispenserState != 0)
		{
			return;
		}
		int count = debrisActiveInstances.Count;
		for (int i = 0; i < amount; i++)
		{
			if (count + i < debrisInstances.Count)
			{
				action();
			}
			else
			{
				DestroyOldestDebris(action);
			}
		}
	}

	private void SetupInstance(GameObject debrisInstance)
	{
		dispenserState = DispenserState.CoolingDown;
		cooldownTimer = cooldownInterval;
		GameObject gameObject = debrisInstance.gameObject;
		gameObject.SetActive(value: true);
		FloatingMesh component = debrisInstance.GetComponent<FloatingMesh>();
		if (component != null)
		{
			component.enabled = true;
			WaterSensor component2 = debrisInstance.GetComponent<WaterSensor>();
			if (component2 != null)
			{
				component2.waterBody.ForceLeaveCollider(component2);
			}
		}
		else
		{
			Debug.LogWarning("no floater associated with this instance of debri");
		}
		debrisActiveInstances.Add(debrisInstance);
	}

	private IEnumerator UnsetupInstance(GameObject garbageInstance, Action onFinished = null)
	{
		GameObject gameObject = garbageInstance.gameObject;
		Transform transform = garbageInstance.transform;
		FloatingMesh floater = garbageInstance.GetComponent<FloatingMesh>();
		if (floater != null)
		{
			floater.enabled = false;
		}
		else
		{
			Debug.LogWarning("no floater associated with this instance of debri");
		}
		yield return new WaitForSeconds(0.5f);
		ResetInstance(gameObject, transform);
		yield return null;
		onFinished();
	}

	private void OnLeverValueChanged(float leverValue)
	{
		if (leverValue >= 1f)
		{
			leverAudioSource.Play();
			SpawnDebris(amountPerSpawn);
		}
	}

	private void Start()
	{
		lever = GetComponentInChildren<SignalBroadcastAngle>();
		lever.onValueChanged += OnLeverValueChanged;
		Transform[] componentsInChildren = debrisPool.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
		debrisInstances = (from t in componentsInChildren
			select t.gameObject into go
			where go != debrisPool.gameObject
			select go).ToList();
		debrisActiveInstances = new List<GameObject>();
	}

	private void Update()
	{
		if (debug)
		{
			OnLeverValueChanged(1f);
		}
		if (dispenserState == DispenserState.CoolingDown)
		{
			cooldownTimer -= Time.deltaTime;
			if (cooldownTimer <= 0f)
			{
				dispenserState = DispenserState.ReadyToDispense;
			}
		}
	}

	private void DestroyOldestDebris(Action onFinished = null)
	{
		int num = 0;
		foreach (GameObject debrisActiveInstance in debrisActiveInstances)
		{
			if (debrisActiveInstance.GetComponent<WaterSensor>().waterBody != null)
			{
				debrisActiveInstances.RemoveAt(num);
				StartCoroutine(UnsetupInstance(debrisActiveInstance, onFinished));
				break;
			}
			num++;
		}
	}

	private void ResetInstance(GameObject gameObject, Transform transform)
	{
		gameObject.SetActive(value: false);
		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.identity;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		foreach (GameObject debrisActiveInstance in debrisActiveInstances)
		{
			ResetInstance(debrisActiveInstance.gameObject, debrisActiveInstance.transform);
		}
		debrisActiveInstances.Clear();
	}
}
