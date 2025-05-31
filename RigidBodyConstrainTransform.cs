using UnityEngine;

public class RigidBodyConstrainTransform : MonoBehaviour
{
	private Rigidbody mRigidBody;

	private RigidbodyConstraints mConstraints;

	public bool FreezePositionAtStart = true;

	public bool FreezeRotationAtStart = true;

	public bool ConstrainPosition
	{
		get
		{
			if (mRigidBody != null)
			{
				return ((mRigidBody.constraints & RigidbodyConstraints.FreezePosition) != 0) ? true : false;
			}
			return false;
		}
		set
		{
			if (value)
			{
				mRigidBody.constraints |= RigidbodyConstraints.FreezePosition;
			}
			else
			{
				mRigidBody.constraints &= (RigidbodyConstraints)(-15);
			}
		}
	}

	public bool ConstrainRotation
	{
		get
		{
			if (mRigidBody != null)
			{
				return ((mRigidBody.constraints & RigidbodyConstraints.FreezeRotation) != 0) ? true : false;
			}
			return false;
		}
		set
		{
			if (value)
			{
				mRigidBody.constraints |= RigidbodyConstraints.FreezeRotation;
			}
			else
			{
				mRigidBody.constraints &= (RigidbodyConstraints)(-113);
			}
		}
	}

	private void Awake()
	{
		mRigidBody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		ConstrainPosition = FreezePositionAtStart;
		ConstrainRotation = FreezeRotationAtStart;
	}
}
