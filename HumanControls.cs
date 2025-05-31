using System;
using System.Collections.Generic;
using CurveGame;
using InControl;
using UnityEngine;

public class HumanControls : MonoBehaviour
{
	private PlayerActions controllerBindings;

	[NonSerialized]
	public InputDevice device;

	public bool allowMouse;

	public SteelSeriesHFF steelSeriesHFF;

	private Human humanScript;

	private Game gameScript;

	private const InputAction leftStickX = InputAction.HorizontalAxis0;

	private const InputAction leftStickY = InputAction.VerticalAxis0;

	public static bool freezeMouse;

	private Vector2 keyLookCache;

	public float leftExtend;

	public float rightExtend;

	private float leftExtendCache;

	private float rightExtendCache;

	public bool leftGrab;

	public bool rightGrab;

	public bool unconscious;

	public bool holding;

	public bool jump;

	public bool shootingFirework;

	public VerticalLookMode verticalLookMode;

	public MouseLookMode mouseLookMode;

	public bool mouseControl = true;

	public float cameraPitchAngle;

	public float cameraYawAngle;

	public float targetPitchAngle;

	public float targetYawAngle;

	public Vector3 walkLocalDirection;

	public Vector3 walkDirection;

	public float unsmoothedWalkSpeed;

	public float walkSpeed;

	private List<float> mouseInputX = new List<float>();

	private List<float> mouseInputY = new List<float>();

	private Vector3 stickDirection;

	private Vector3 oldStickDirection;

	private float previousLeftExtend;

	private float previousRightExtend;

	private float shootCooldown;

	public Vector2 calc_joyLook
	{
		get
		{
			if (controllerBindings != null)
			{
				Vector2 result = new Vector2(controllerBindings.LookX, controllerBindings.LookY);
				if (Options.controllerInvert > 0)
				{
					result.y *= -1f;
				}
				float a = 0.1f + Mathf.Abs(result.y) * 0.15f;
				if (result.x < 0f)
				{
					result.x = 0f - Mathf.InverseLerp(a, 1f, 0f - result.x);
				}
				else
				{
					result.x = Mathf.InverseLerp(a, 1f, result.x);
				}
				result.x *= controllerBindings.HScale;
				result.y *= controllerBindings.VScale;
				return result;
			}
			return Vector2.zero;
		}
	}

	public Vector2 calc_keyLook
	{
		get
		{
			if (allowMouse)
			{
				if (!freezeMouse)
				{
					keyLookCache = new Vector2(Options.keyboardBindings.LookX, Options.keyboardBindings.LookY);
				}
				return keyLookCache;
			}
			return Vector2.zero;
		}
	}

	public Vector3 calc_joyWalk
	{
		get
		{
			if (controllerBindings != null)
			{
				Vector3 result = new Vector3(controllerBindings.Move.X, 0f, controllerBindings.Move.Y);
				float num = 0.1f + Mathf.Abs(result.y) * 0.1f;
				if (Mathf.Abs(result.x) < num)
				{
					result.x = 0f;
				}
				return result;
			}
			return Vector3.zero;
		}
	}

	public Vector3 calc_keyWalk
	{
		get
		{
			if (allowMouse)
			{
				return new Vector3(Options.keyboardBindings.Move.X, 0f, Options.keyboardBindings.Move.Y);
			}
			return Vector3.zero;
		}
	}

	public float lookYnormalized => (controllerBindings == null) ? 0f : controllerBindings.LookYNormalized;

	public float vScale => (controllerBindings == null) ? 0f : controllerBindings.VScale;

	private void OnEnable()
	{
		SetAny();
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		steelSeriesHFF = GetComponent<SteelSeriesHFF>();
		humanScript = base.transform.Find("Ball").GetComponent<Human>();
		gameScript = GameObject.Find("Game(Clone)").GetComponent<Game>();
	}

	private float GetLeftExtend()
	{
		if (!freezeMouse)
		{
			leftExtendCache = ((!allowMouse) ? 0f : Options.keyboardBindings.LeftHand.Value);
		}
		return Mathf.Max((controllerBindings == null) ? 0f : controllerBindings.LeftHand.Value, leftExtendCache);
	}

	private float GetRightExtend()
	{
		if (!freezeMouse)
		{
			rightExtendCache = ((!allowMouse) ? 0f : Options.keyboardBindings.RightHand.Value);
		}
		return Mathf.Max((controllerBindings == null) ? 0f : controllerBindings.RightHand.Value, rightExtendCache);
	}

	private bool GetUnconscious()
	{
		return (controllerBindings != null && controllerBindings.Unconscious.IsPressed) || (allowMouse && Options.keyboardBindings.Unconscious.IsPressed);
	}

	public bool ControllerJumpPressed()
	{
		return controllerBindings != null && controllerBindings.Jump.IsPressed;
	}

	public bool ControllerFireworksPressed()
	{
		return controllerBindings != null && controllerBindings.ShootFireworks.IsPressed;
	}

	private bool GetJump()
	{
		return ControllerJumpPressed() || (allowMouse && Options.keyboardBindings.Jump.IsPressed);
	}

	private bool GetFireworks()
	{
		return ControllerFireworksPressed() || (allowMouse && Options.keyboardBindings.ShootFireworks.IsPressed);
	}

	public void ReadInput(out float walkForward, out float walkRight, out float cameraPitch, out float cameraYaw, out float leftExtend, out float rightExtend, out bool jump, out bool playDead, out bool shooting)
	{
		Vector2 vector = calc_joyLook;
		Vector2 vector2 = calc_keyLook;
		Vector3 vector3 = calc_joyWalk;
		Vector3 vector4 = calc_keyWalk;
		if (vector.sqrMagnitude > vector2.sqrMagnitude)
		{
			mouseControl = false;
		}
		if (vector2.sqrMagnitude > vector.sqrMagnitude)
		{
			mouseControl = true;
		}
		if (vector3.sqrMagnitude > vector4.sqrMagnitude)
		{
			mouseControl = false;
		}
		if (vector4.sqrMagnitude > vector3.sqrMagnitude)
		{
			mouseControl = true;
		}
		cameraPitch = cameraPitchAngle;
		cameraYaw = cameraYawAngle;
		if (mouseControl)
		{
			cameraYaw += Smoothing.SmoothValue(mouseInputX, vector2.x);
			cameraPitch -= Smoothing.SmoothValue(mouseInputY, vector2.y);
			cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
			if (mouseLookMode == MouseLookMode.SpringBackNoGrab && !leftGrab && !rightGrab)
			{
				int num = 0;
				float num2 = 0.25f;
				cameraPitch = Mathf.Lerp(cameraPitch, num, num2 * 5f * Time.fixedDeltaTime);
				cameraPitch = Mathf.MoveTowards(cameraPitch, num, num2 * 30f * Time.fixedDeltaTime);
			}
		}
		else
		{
			cameraYaw += vector.x * Time.deltaTime * 120f;
			if (verticalLookMode == VerticalLookMode.Relative)
			{
				if (Options.controllerInvert > 0)
				{
					vector.y = 0f - vector.y;
				}
				cameraPitch -= vector.y * Time.deltaTime * 120f * 2f;
				cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
			}
			else
			{
				float num3 = -80f * lookYnormalized;
				bool flag = num3 * cameraPitch < 0f || Mathf.Abs(num3) > Mathf.Abs(cameraPitch);
				float num4 = ((!leftGrab && !rightGrab) ? 1f : Mathf.Abs(lookYnormalized));
				float num5 = ((!leftGrab && !rightGrab) ? 0.25f : 0.0125f);
				float num6 = ((!flag) ? num5 : num4);
				cameraPitch = Mathf.Lerp(cameraPitch, num3, num6 * 5f * Time.fixedDeltaTime * vScale);
				cameraPitch = Mathf.MoveTowards(cameraPitch, num3, num6 * 30f * Time.fixedDeltaTime * vScale);
			}
		}
		Vector3 vector5 = ((!(vector3.sqrMagnitude > vector4.sqrMagnitude)) ? vector4 : vector3);
		walkForward = vector5.z;
		walkRight = vector5.x;
		if (MenuSystem.keyboardState == KeyboardState.None)
		{
			leftExtend = GetLeftExtend();
			rightExtend = GetRightExtend();
			jump = GetJump();
			playDead = GetUnconscious();
			previousLeftExtend = leftExtend;
			previousRightExtend = rightExtend;
			shooting = GetFireworks();
		}
		else
		{
			leftExtend = previousLeftExtend;
			rightExtend = previousRightExtend;
			jump = false;
			playDead = false;
			shooting = false;
		}
	}

	public void HandleInput(float walkForward, float walkRight, float cameraPitch, float cameraYaw, float leftExtend, float rightExtend, bool jump, bool playDead, bool holding, bool shooting)
	{
		walkLocalDirection = new Vector3(walkRight, 0f, walkForward);
		cameraPitchAngle = cameraPitch;
		cameraYawAngle = cameraYaw;
		this.leftExtend = leftExtend;
		this.rightExtend = rightExtend;
		leftGrab = leftExtend > 0f;
		rightGrab = rightExtend > 0f;
		if (steelSeriesHFF != null)
		{
			steelSeriesHFF.SteelSeriesEvent_LeftArm(leftExtend > 0f);
			steelSeriesHFF.SteelSeriesEvent_RightArm(rightExtend > 0f);
			steelSeriesHFF.SteelSeriesEvent_Respawning(humanScript.spawning);
			if (gameScript.passedCheckpoint_ForSteelSeriesEvent)
			{
				gameScript.passedCheckpoint_ForSteelSeriesEvent = false;
				steelSeriesHFF.SteelSeriesEvent_CheckpointHit();
			}
		}
		this.jump = jump;
		unconscious = playDead;
		this.holding = holding;
		shootingFirework = shooting;
		targetPitchAngle = Mathf.MoveTowards(targetPitchAngle, cameraPitchAngle, 180f * Time.fixedDeltaTime / 0.1f);
		targetYawAngle = cameraYawAngle;
		Quaternion quaternion = Quaternion.Euler(0f, cameraYawAngle, 0f);
		Vector3 vector = quaternion * walkLocalDirection;
		unsmoothedWalkSpeed = vector.magnitude;
		vector = new Vector3(FilterAxisAcceleration(oldStickDirection.x, vector.x), 0f, FilterAxisAcceleration(oldStickDirection.z, vector.z));
		walkSpeed = vector.magnitude;
		if (walkSpeed > 0f)
		{
			walkDirection = vector;
		}
		oldStickDirection = vector;
	}

	private float FilterAxisAcceleration(float currentValue, float desiredValue)
	{
		float num = Time.fixedDeltaTime / 1f;
		float num2 = 0.2f;
		if (currentValue * desiredValue <= 0f)
		{
			currentValue = 0f;
		}
		if (Mathf.Abs(currentValue) > Mathf.Abs(desiredValue))
		{
			currentValue = desiredValue;
		}
		if (Mathf.Abs(currentValue) < num2)
		{
			num = Mathf.Max(num, num2 - Mathf.Abs(currentValue));
		}
		if (Mathf.Abs(currentValue) > 0.8f)
		{
			num /= 3f;
		}
		return Mathf.MoveTowards(currentValue, desiredValue, num);
	}

	public void SetDevice(InputDevice inputDevice)
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = inputDevice;
		controllerBindings = Options.CreateInput(inputDevice);
		allowMouse = false;
	}

	public void SetController()
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = null;
		controllerBindings = Options.controllerBindings;
		allowMouse = false;
	}

	public void SetMouse()
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = null;
		controllerBindings = null;
		allowMouse = true;
	}

	public void SetAny()
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = null;
		controllerBindings = Options.controllerBindings;
		allowMouse = true;
	}
}
