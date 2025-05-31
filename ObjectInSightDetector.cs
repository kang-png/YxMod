using System.Collections.Generic;
using HumanAPI;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectInSightDetector : Node
{
	public NodeOutput output;

	[SerializeField]
	private LayerMask layerMask;

	public Collider objectToTrack;

	private List<Human> humansInDetectionArea = new List<Human>();

	private void Update()
	{
		if (humansInDetectionArea.Count > 0 && output.value < 0.5f)
		{
			CheckObjectPosition();
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		foreach (Human item in Human.all)
		{
			if (!item.IsLocalPlayer || humansInDetectionArea.Contains(item) || !(item.GetComponent<Collider>() == col))
			{
				continue;
			}
			humansInDetectionArea.Add(item);
			break;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		for (int num = humansInDetectionArea.Count - 1; num >= 0; num--)
		{
			if (humansInDetectionArea[num].GetComponent<Collider>() == col)
			{
				humansInDetectionArea.RemoveAt(num);
				break;
			}
		}
	}

	private void CheckObjectPosition()
	{
		if (IsObjectInSight())
		{
			output.SetValue(1f);
		}
	}

	private bool IsObjectInSight()
	{
		foreach (Human item in humansInDetectionArea)
		{
			Camera gameCam = item.player.cameraController.gameCam;
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(gameCam);
			if (GeometryUtility.TestPlanesAABB(planes, objectToTrack.bounds))
			{
				Physics.Raycast(gameCam.transform.position, Vector3.Normalize(objectToTrack.transform.position - gameCam.transform.position), out var hitInfo, Vector3.Distance(objectToTrack.transform.position, gameCam.transform.position), layerMask.value);
				if (hitInfo.collider == objectToTrack)
				{
					return true;
				}
			}
		}
		return false;
	}
}
