using UnityEngine;

public class Rotation : MonoBehaviour
{
	[SerializeField]
	private float speed;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f));
	}
}
