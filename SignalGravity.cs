using System.Runtime.CompilerServices;
using HumanAPI;
using UnityEngine;

public class SignalGravity : Node
{
	[Tooltip("Default gravity is (X:0, Y:-9.81, Z:0")]
	public NodeInput gravityX;

	[Tooltip("Default gravity is (X:0, Y:-9.81, Z:0")]
	public NodeInput gravityY;

	[Tooltip("Default gravity is (X:0, Y:-9.81, Z:0")]
	public NodeInput gravityZ;

	public NodeInput setDefault;

	private float prevX;

	private float prevY;

	private float prevZ;

	private float prevSetDefault;

	private Vector3 defaultGravity = new Vector3(0f, -9.81f, 0f);

	public override string Title
	{
		[CompilerGenerated]
		get
		{
			return $"Gravity: {Physics.gravity}";
		}
	}

	private bool ResetThisFrame
	{
		[CompilerGenerated]
		get
		{
			return setDefault.value >= 0.5f && prevSetDefault < 0.5f;
		}
	}

	private bool valueChanged
	{
		[CompilerGenerated]
		get
		{
			return prevX != gravityX.value || prevY != gravityY.value || prevZ != gravityZ.value;
		}
	}

	public override void Process()
	{
		if (valueChanged || ResetThisFrame)
		{
			Physics.gravity = ((!ResetThisFrame) ? new Vector3(gravityX.value, gravityY.value, gravityZ.value) : defaultGravity);
		}
		prevSetDefault = setDefault.value;
		prevX = gravityX.value;
		prevY = gravityY.value;
		prevZ = gravityZ.value;
	}
}
