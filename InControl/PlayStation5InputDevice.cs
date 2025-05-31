using UnityEngine;

namespace InControl;

public class PlayStation5InputDevice : InputDevice
{
	private const string RegistrationMark = "®";

	private const float LowerDeadZone = 0.2f;

	private const float UpperDeadZone = 0.9f;

	private const int MaxAnalogs = 8;

	private const int MaxButtons = 10;

	private string[] analogQueries;

	private string[] buttonQueries;

	public int JoystickId { get; private set; }

	public bool IsConnected => false;

	public PlayStation5InputDevice(int joystickId)
		: base("DualSense® wireless controller")
	{
		JoystickId = joystickId;
		base.SortOrder = joystickId;
		base.Meta = "PlayStation 5 Device #" + joystickId;
		base.DeviceClass = InputDeviceClass.Controller;
		base.DeviceStyle = InputDeviceStyle.PlayStation4;
		SetupAnalogQueries();
		SetupButtonQueries();
		AddControl(InputControlType.LeftStickLeft, "left stick left", 0.2f, 0.9f);
		AddControl(InputControlType.LeftStickRight, "left stick right", 0.2f, 0.9f);
		AddControl(InputControlType.LeftStickUp, "left stick up", 0.2f, 0.9f);
		AddControl(InputControlType.LeftStickDown, "left stick down", 0.2f, 0.9f);
		AddControl(InputControlType.RightStickLeft, "right stick left", 0.2f, 0.9f);
		AddControl(InputControlType.RightStickRight, "right stick right", 0.2f, 0.9f);
		AddControl(InputControlType.RightStickUp, "right stick up", 0.2f, 0.9f);
		AddControl(InputControlType.RightStickDown, "right stick down", 0.2f, 0.9f);
		AddControl(InputControlType.LeftTrigger, "L2 button", 0.2f, 0.9f);
		AddControl(InputControlType.RightTrigger, "R2 button", 0.2f, 0.9f);
		AddControl(InputControlType.DPadUp, "up button", 0.2f, 0.9f);
		AddControl(InputControlType.DPadDown, "down button", 0.2f, 0.9f);
		AddControl(InputControlType.DPadLeft, "left button", 0.2f, 0.9f);
		AddControl(InputControlType.DPadRight, "right button", 0.2f, 0.9f);
		AddControl(InputControlType.Action1, "cross button");
		AddControl(InputControlType.Action2, "circle button");
		AddControl(InputControlType.Action3, "square button");
		AddControl(InputControlType.Action4, "triangle button");
		AddControl(InputControlType.LeftBumper, "L1 button");
		AddControl(InputControlType.RightBumper, "R1 button");
		AddControl(InputControlType.TouchPadButton, "touch pad button");
		AddControl(InputControlType.Options, "OPTIONS button");
		AddControl(InputControlType.LeftStickButton, "L3 button");
		AddControl(InputControlType.RightStickButton, "R3 button");
	}

	public override void Update(ulong updateTick, float deltaTime)
	{
		Vector2 value = new Vector2(GetAnalogValue(0), 0f - GetAnalogValue(1));
		UpdateLeftStickWithValue(value, updateTick, deltaTime);
		Vector2 value2 = new Vector2(GetAnalogValue(3), 0f - GetAnalogValue(4));
		UpdateRightStickWithValue(value2, updateTick, deltaTime);
		UpdateWithState(InputControlType.DPadLeft, GetAnalogValue(5) < 0f, updateTick, deltaTime);
		UpdateWithState(InputControlType.DPadRight, GetAnalogValue(5) > 0f, updateTick, deltaTime);
		UpdateWithState(InputControlType.DPadUp, GetAnalogValue(6) > 0f, updateTick, deltaTime);
		UpdateWithState(InputControlType.DPadDown, GetAnalogValue(6) < 0f, updateTick, deltaTime);
		UpdateWithValue(InputControlType.LeftTrigger, GetAnalogValue(7), updateTick, deltaTime);
		UpdateWithValue(InputControlType.RightTrigger, GetAnalogValue(2), updateTick, deltaTime);
		UpdateWithState(InputControlType.Action1, GetButtonState(0), updateTick, deltaTime);
		UpdateWithState(InputControlType.Action2, GetButtonState(1), updateTick, deltaTime);
		UpdateWithState(InputControlType.Action3, GetButtonState(2), updateTick, deltaTime);
		UpdateWithState(InputControlType.Action4, GetButtonState(3), updateTick, deltaTime);
		UpdateWithState(InputControlType.LeftBumper, GetButtonState(4), updateTick, deltaTime);
		UpdateWithState(InputControlType.RightBumper, GetButtonState(5), updateTick, deltaTime);
		UpdateWithState(InputControlType.TouchPadButton, GetButtonState(6), updateTick, deltaTime);
		UpdateWithState(InputControlType.Options, GetButtonState(7), updateTick, deltaTime);
		UpdateWithState(InputControlType.LeftStickButton, GetButtonState(8), updateTick, deltaTime);
		UpdateWithState(InputControlType.RightStickButton, GetButtonState(9), updateTick, deltaTime);
	}

	public override void Vibrate(float leftMotor, float rightMotor)
	{
	}

	public void Vibrate(float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
	{
		Vibrate(leftMotor, rightMotor);
	}

	public override void SetLightColor(float red, float green, float blue)
	{
	}

	private void SetupAnalogQueries()
	{
		analogQueries = new string[8];
		for (int i = 0; i < 8; i++)
		{
			analogQueries[i] = "joystick " + (JoystickId + 1) + " analog " + i;
		}
	}

	private void SetupButtonQueries()
	{
		buttonQueries = new string[10];
		for (int i = 0; i < 10; i++)
		{
			buttonQueries[i] = "joystick " + (JoystickId + 1) + " button " + i;
		}
	}

	public float GetAnalogValue(int index)
	{
		if (index < 8)
		{
			return Input.GetAxisRaw(analogQueries[index]);
		}
		return 0f;
	}

	public bool GetButtonState(int index)
	{
		if (index < 10)
		{
			return Input.GetKey(buttonQueries[index]);
		}
		return false;
	}
}
