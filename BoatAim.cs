using UnityEngine;

public class BoatAim : MonoBehaviour
{
	public Rigidbody boat;

	public Rigidbody paddle1;

	public Rigidbody paddle2;

	public Vector3 alignAxis;

	public float strength = 5f;

	public float maxTorque = 1000f;

	private void FixedUpdate()
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.grabManager.IsGrabbed(paddle1.gameObject) && human.grabManager.IsGrabbed(paddle2.gameObject))
			{
				Vector3 vector = base.transform.TransformDirection(alignAxis);
				Vector3 vector2 = Quaternion.Euler(0f, human.controls.targetYawAngle, 0f) * Vector3.forward;
				float num = Math2d.SignedAngle(vector2.To2D(), vector.To2D());
				num *= Vector3.Dot(vector.ZeroY(), vector2);
				float num2 = Mathf.Abs((paddle1.angularVelocity - boat.angularVelocity).y) + Mathf.Abs((paddle2.angularVelocity - boat.angularVelocity).y);
				boat.AddTorque(Vector3.up * Mathf.Clamp(num * strength * num2, 0f - maxTorque, maxTorque));
			}
		}
	}
}
