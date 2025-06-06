namespace InControl;

[AutoDiscover]
public class PlayStation2WinProfile : UnityInputDeviceProfile
{
	public PlayStation2WinProfile()
	{
		base.Name = "PlayStation DualShock 2 Controller";
		base.Meta = "Compatible with PlayStation 2 Controller to USB adapter.";
		base.DeviceClass = InputDeviceClass.Controller;
		base.DeviceStyle = InputDeviceStyle.PlayStation2;
		base.IncludePlatforms = new string[1] { "Windows" };
		JoystickNames = new string[1] { "Twin USB Joystick" };
		base.ButtonMappings = new InputControlMapping[12]
		{
			new InputControlMapping
			{
				Handle = "Cross",
				Target = InputControlType.Action1,
				Source = UnityInputDeviceProfile.Button2
			},
			new InputControlMapping
			{
				Handle = "Circle",
				Target = InputControlType.Action2,
				Source = UnityInputDeviceProfile.Button1
			},
			new InputControlMapping
			{
				Handle = "Square",
				Target = InputControlType.Action3,
				Source = UnityInputDeviceProfile.Button3
			},
			new InputControlMapping
			{
				Handle = "Triangle",
				Target = InputControlType.Action4,
				Source = UnityInputDeviceProfile.Button0
			},
			new InputControlMapping
			{
				Handle = "Left Bumper",
				Target = InputControlType.LeftBumper,
				Source = UnityInputDeviceProfile.Button6
			},
			new InputControlMapping
			{
				Handle = "Right Bumper",
				Target = InputControlType.RightBumper,
				Source = UnityInputDeviceProfile.Button7
			},
			new InputControlMapping
			{
				Handle = "Left Trigger",
				Target = InputControlType.LeftTrigger,
				Source = UnityInputDeviceProfile.Button4
			},
			new InputControlMapping
			{
				Handle = "Right Trigger",
				Target = InputControlType.RightTrigger,
				Source = UnityInputDeviceProfile.Button5
			},
			new InputControlMapping
			{
				Handle = "Select",
				Target = InputControlType.Select,
				Source = UnityInputDeviceProfile.Button8
			},
			new InputControlMapping
			{
				Handle = "Start",
				Target = InputControlType.Start,
				Source = UnityInputDeviceProfile.Button9
			},
			new InputControlMapping
			{
				Handle = "Left Stick Button",
				Target = InputControlType.LeftStickButton,
				Source = UnityInputDeviceProfile.Button10
			},
			new InputControlMapping
			{
				Handle = "Right Stick Button",
				Target = InputControlType.RightStickButton,
				Source = UnityInputDeviceProfile.Button11
			}
		};
		base.AnalogMappings = new InputControlMapping[12]
		{
			UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
			UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
			UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
			UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
			UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog3),
			UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog3),
			UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog2),
			UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog2),
			UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
		};
	}
}
