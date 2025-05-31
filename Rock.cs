using UnityEngine;

public class Rock : MonoBehaviour
{
	public enum RockType
	{
		A,
		B,
		C
	}

	public RockType type;

	private RockSpawnHandler spawner;

	public void SetSpawner(RockSpawnHandler spawner)
	{
		this.spawner = spawner;
	}

	public void Despawn()
	{
		spawner.ReturnToPool(base.gameObject);
	}
}
