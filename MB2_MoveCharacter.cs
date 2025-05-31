using UnityEngine;

public class MB2_MoveCharacter : MonoBehaviour
{
	private CharacterController characterController;

	public float speed = 5f;

	public Transform target;

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (Time.frameCount % 500 != 0)
		{
			Vector3 vector = target.position - base.transform.position;
			vector.Normalize();
			characterController.Move(vector * speed * Time.deltaTime);
		}
	}
}
