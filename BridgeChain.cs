using UnityEngine;

public class BridgeChain : MonoBehaviour
{
	public GameObject chainPrefab;

	public Transform origin;

	public Transform target;

	public int length = 5;

	public int minCount = 5;

	private int count;

	public float lowerHeight;

	public float topHeight;

	private void Start()
	{
		Transform child = base.transform;
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = Object.Instantiate(chainPrefab, base.transform);
			gameObject.transform.position = child.position;
			child = gameObject.transform.GetChild(0);
		}
	}

	private void FixedUpdate()
	{
		base.transform.position = origin.position;
		base.transform.LookAt(target);
	}

	private void Update()
	{
		float num = topHeight - lowerHeight;
		float num2 = num - (base.transform.position.y - lowerHeight);
		count = Mathf.Min(Mathf.FloorToInt(num2 / num * (float)length) + minCount, length);
		for (int i = 0; i < length; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(i < count);
		}
	}
}
