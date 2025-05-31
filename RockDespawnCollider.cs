using UnityEngine;

public class RockDespawnCollider : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject = other.gameObject;
		Rock component = other.GetComponent<Rock>();
		if (component != null)
		{
			component.Despawn();
		}
	}
}
