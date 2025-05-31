using System.Collections.Generic;
using HumanAPI;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public LayerMask collisionLayers;

	private GrabManager grabManager;

	private Human human;

	private Ragdoll ragdoll;

	private float ballRadius;

	private Rigidbody rigidbody;

	private bool isUnderwater;

	private GameObject levelGameObject;

	private BuiltinLevel builtinLvl;

	private float checkDelay;

	private List<Collision> collisions = new List<Collision>();

	private List<Vector3> contacts = new List<Vector3>();

	public Vector3 lastImpulse;

	public Vector3 lastNonZeroImpulse;

	public float timeSinceLastNonzeroImpulse;

	private void OnEnable()
	{
		human = GetComponent<Human>();
		ragdoll = GetComponent<Ragdoll>();
		ballRadius = GetComponent<SphereCollider>().radius;
		grabManager = GetComponent<GrabManager>();
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		checkDelay += Time.deltaTime;
		if (!(checkDelay >= 5f))
		{
			return;
		}
		if (levelGameObject == null)
		{
			builtinLvl = null;
			levelGameObject = GameObject.Find("Level");
			if ((bool)levelGameObject)
			{
				builtinLvl = levelGameObject.GetComponent<BuiltinLevel>();
			}
		}
		checkDelay = 0f;
	}

	private void FixedUpdate()
	{
		ImpulseForce2();
		if (levelGameObject != null && builtinLvl != null && builtinLvl.underwaterGravity && !isUnderwater)
		{
			rigidbody.velocity += new Vector3(0f, -40f, 0f) * Time.deltaTime;
		}
		collisions.Clear();
		contacts.Clear();
	}

	private void ImpulseForce2()
	{
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts.Length != 0)
		{
			HandleCollision(collision);
			collisions.Add(collision);
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				contacts.Add(collision.contacts[i].point);
			}
		}
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "UnderwaterGravity")
		{
			isUnderwater = true;
		}
	}

	public void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "UnderwaterGravity")
		{
			isUnderwater = false;
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (collision.contacts.Length != 0)
		{
			HandleCollision(collision);
			collisions.Add(collision);
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				contacts.Add(collision.contacts[i].point);
			}
		}
	}

	private void HandleCollision(Collision collision)
	{
		Vector3 impulse = collision.GetImpulse();
		if (impulse.y > 0f && human.onGround)
		{
			timeSinceLastNonzeroImpulse = Time.time;
		}
		Vector3 walkDirection = human.controls.walkDirection;
		Debug.DrawRay(collision.contacts[0].point, impulse / 10f, Color.red, 0.5f);
		if (Vector3.Dot(impulse, walkDirection) >= 0f)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			Vector3 point = collision.contacts[i].point;
			Vector3 vector = point + walkDirection * 0.07f + Vector3.up * 0.07f;
			Vector3 vector2 = point - walkDirection * 0.07f - Vector3.up * 0.07f;
			Debug.DrawRay(vector, Vector3.down * 0.1f, Color.blue);
			if (Physics.Raycast(vector, Vector3.down, out var hitInfo, 0.1f, collisionLayers))
			{
				Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red);
			}
			Debug.DrawRay(vector2, walkDirection * 0.1f, Color.blue);
			if (Physics.Raycast(vector2, walkDirection, out hitInfo, 0.1f, collisionLayers))
			{
				Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red);
			}
			if (Physics.Raycast(vector, Vector3.down, out hitInfo, 0.1f, collisionLayers) && hitInfo.normal.y > 0.7f && Physics.Raycast(vector2, walkDirection, out hitInfo, 0.1f, collisionLayers) && hitInfo.normal.y < 0.4f)
			{
				Debug.DrawLine(base.transform.position, collision.contacts[i].point, Color.red);
				num = 1.5f;
				break;
			}
		}
		if (human.ragdoll.partLeftHand.sensor.grabJoint != null && human.ragdoll.partRightHand.sensor.grabJoint != null)
		{
			float num2 = ((!(human.ragdoll.partLeftHand.sensor.grabJoint != null)) ? 0f : Vector3.Dot(human.ragdoll.partLeftHand.transform.position - base.transform.position, walkDirection));
			float num3 = ((!(human.ragdoll.partRightHand.sensor.grabJoint != null)) ? 0f : Vector3.Dot(human.ragdoll.partRightHand.transform.position - base.transform.position, walkDirection));
			num = Mathf.Max(num, (num2 + num3) / 2f);
		}
		if (num > 0f)
		{
			Vector3 vector3 = impulse.ZeroY();
			impulse = Vector3.up * vector3.magnitude * num - vector3 / 2f;
			human.ragdoll.partBall.rigidbody.SafeAddForce(impulse, ForceMode.Impulse);
			human.groundManager.DistributeForce(-impulse / Time.fixedDeltaTime, base.transform.position);
		}
	}
}
