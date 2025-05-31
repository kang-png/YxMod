using Multiplayer;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
	public Transform spawnPosition;

	public Collider uprightCollider;

	public Collider uprightSensor;

	private Rigidbody rigidBody;

	private NetBody netBody;

	private bool respawning;

	private float respawningTimer;

	private const float respawnDuration = 0.5f;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		netBody = GetComponent<NetBody>();
	}

	public bool IsInPlace()
	{
		return uprightCollider.bounds.Intersects(uprightSensor.bounds);
	}

	public void ResetPosition()
	{
		rigidBody.isKinematic = true;
		base.transform.position = spawnPosition.position;
		base.transform.rotation = Quaternion.identity;
		respawning = true;
		respawningTimer = 0f;
	}

	public void Hide()
	{
		netBody.SetVisible(visible: false);
	}

	public void Show()
	{
		netBody.SetVisible(visible: true);
		ResetPosition();
	}

	private void Update()
	{
		if (respawning)
		{
			respawningTimer += Time.deltaTime;
			if (respawningTimer >= 0.5f)
			{
				rigidBody.isKinematic = false;
				respawning = false;
			}
		}
	}
}
