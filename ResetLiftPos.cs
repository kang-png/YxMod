using UnityEngine;

public class ResetLiftPos : MonoBehaviour
{
	public Vector3 topStart;

	public Vector3 entranceStart;

	public Vector3 gateStart;

	public Vector3 liftStart;

	public string topName;

	public string entranceName;

	public string gateName;

	public string liftName;

	private GameObject topLevel;

	private GameObject entranceLevel;

	private GameObject gateLevel;

	private GameObject liftLevel;

	public void ResetStartPos()
	{
		topLevel.transform.localPosition = topStart;
		entranceLevel.transform.localPosition = entranceStart;
		gateLevel.transform.localPosition = gateStart;
		liftLevel.transform.localPosition = liftStart;
	}

	private void Awake()
	{
		topLevel = GameObject.Find(topName);
		entranceLevel = topLevel.transform.Find(entranceName).gameObject;
		gateLevel = entranceLevel.transform.Find(gateName).gameObject;
		liftLevel = entranceLevel.transform.Find(liftName).gameObject;
		ResetStartPos();
	}
}
