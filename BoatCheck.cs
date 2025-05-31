using UnityEngine;

public class BoatCheck : MonoBehaviour
{
	public BoatStats boat1;

	public BoatStats boat2;

	public DonutAchievement scr;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Player") && boat1.collisions == 0 && boat2.collisions == 0)
		{
			scr.output.SetValue(1f);
		}
	}
}
