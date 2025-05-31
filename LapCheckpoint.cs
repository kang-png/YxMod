using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LapCheckpoint : MonoBehaviour
{
	[SerializeField]
	private ThreeLapsAchievement threeLapsAchievement;

	public LapCheckpoint Next;

	public LapCheckpoint Previous;

	private Collider col;

	private void Awake()
	{
		col = GetComponent<Collider>();
		col.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		threeLapsAchievement.CheckpointTriggered(this);
	}
}
