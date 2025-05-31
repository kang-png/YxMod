using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmasLandAchievement : MonoBehaviour
{
	public List<GameObject> playersJumping = new List<GameObject>();

	private bool haveSnowBoard;

	public void SetJumpingWithSnowBoard(GameObject user)
	{
		StartCoroutine(JumpingWithSnowBoard(user));
	}

	private IEnumerator JumpingWithSnowBoard(GameObject user)
	{
		playersJumping.Add(user);
		yield return new WaitForSeconds(3f);
		playersJumping.Remove(user);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && playersJumping.Contains(other.transform.parent.gameObject))
		{
			Debug.LogError("Got achievement!");
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_XMAS_SKI_LAND);
		}
	}
}
