using HumanAPI;
using UnityEngine;

public class SignalTeleport : Node
{
	public NodeInput triggerTeleport;

	public Transform objectToTeleport;

	public Transform targetTransform;

	public bool teleportToInitialPos = true;

	private float prevValue;

	private Rigidbody body;

	private Vector3 InitialPosition;

	private Quaternion InitialRotation;

	private int InitialChildren;

	private Vector3[] ChildPosition;

	private Quaternion[] ChildRotation;

	private bool doTeleport;

	private int telePhase;

	private void Start()
	{
		body = objectToTeleport.gameObject.GetComponent<Rigidbody>();
		if (body != null)
		{
			InitialPosition = body.position;
			InitialRotation = body.rotation;
		}
		else
		{
			InitialPosition = objectToTeleport.transform.position;
			InitialRotation = objectToTeleport.transform.rotation;
		}
		InitialChildren = objectToTeleport.childCount;
		ChildPosition = new Vector3[InitialChildren];
		ChildRotation = new Quaternion[InitialChildren];
		for (int i = 0; i < InitialChildren; i++)
		{
			Transform child = objectToTeleport.transform.GetChild(i);
			Rigidbody component = child.gameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				ref Vector3 reference = ref ChildPosition[i];
				reference = component.position;
				ref Quaternion reference2 = ref ChildRotation[i];
				reference2 = component.rotation;
			}
			else
			{
				ref Vector3 reference3 = ref ChildPosition[i];
				reference3 = child.position;
				ref Quaternion reference4 = ref ChildRotation[i];
				reference4 = child.rotation;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!doTeleport)
		{
			return;
		}
		if (telePhase == 0)
		{
			if (teleportToInitialPos)
			{
				if (body != null)
				{
					body.MovePosition(InitialPosition);
					body.MoveRotation(InitialRotation);
				}
				objectToTeleport.transform.position = InitialPosition;
				objectToTeleport.transform.rotation = InitialRotation;
			}
			for (int i = 0; i < InitialChildren; i++)
			{
				Transform child = objectToTeleport.transform.GetChild(i);
				Rigidbody component = child.gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.MovePosition(ChildPosition[i]);
					component.MoveRotation(ChildRotation[i]);
				}
				child.position = ChildPosition[i];
				child.rotation = ChildRotation[i];
			}
			telePhase++;
		}
		else
		{
			if (body != null)
			{
				body.MovePosition(targetTransform.position);
				body.MoveRotation(targetTransform.rotation);
			}
			else
			{
				objectToTeleport.transform.position = targetTransform.position;
				objectToTeleport.transform.rotation = targetTransform.rotation;
			}
			doTeleport = false;
		}
	}

	public override void Process()
	{
		if (triggerTeleport.value >= 0.5f && prevValue < 0.5f)
		{
			doTeleport = true;
			telePhase = 0;
		}
		prevValue = triggerTeleport.value;
	}
}
