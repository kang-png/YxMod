using UnityEngine;

public class XmasLandingAchievementHelper : MonoBehaviour
{
	public XmasLandAchievement achiScript;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && SnowBoardManager.Instance.GetHumansBoard(other.GetComponent<Human>()) != null)
		{
			achiScript.SetJumpingWithSnowBoard(other.transform.parent.gameObject);
		}
	}
}
