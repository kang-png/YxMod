using UnityEngine;

public class ResetLeversPos : MonoBehaviour
{
	public Vector3 leverMoveLRStart;

	public Vector3 leverAimLRStart;

	public Vector3 leverAimUDStart;

	public GameObject leverMoveLR;

	public GameObject leverAimLR;

	public GameObject leverAimUD;

	public void ResetStartPos()
	{
		leverMoveLR.transform.localPosition = leverMoveLRStart;
		leverAimLR.transform.localPosition = leverAimLRStart;
		leverAimUD.transform.localPosition = leverAimUDStart;
	}

	private void Awake()
	{
	}
}
