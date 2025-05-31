using UnityEngine;

public class SimpleCharacterController2 : MonoBehaviour
{
	public float speed = 20f;

	public float rotateSpeed = 3f;

	public void Update()
	{
		CharacterController component = GetComponent<CharacterController>();
		base.transform.Rotate(0f, Input.GetAxis("Horizontal") * rotateSpeed, 0f);
		Vector3 vector = base.transform.TransformDirection(Vector3.forward);
		float num = speed * Input.GetAxis("Vertical");
		component.SimpleMove(vector * num);
	}
}
