using UnityEngine;

public class CounterweightStretch : MonoBehaviour
{
	public Transform target;

	private void Start()
	{
	}

	private void Update()
	{
		float y = (target.transform.position.y - base.transform.position.y) / 2f;
		base.transform.localScale = new Vector3(1f, y, 1f);
	}
}
