namespace InControl;

[AutoDiscover]
public class PlayStation4LinuxProfile : UnityInputDeviceProfile
{
	public PlayStation4LinuxProfile()
	{
		base.Name = "PlayStation 4 Controller";
		base.Meta = "PlayStation 4 Controller on Linux";
		base.DeviceClass = InputDeviceClass.Controller;
		base.DeviceStyle = InputDeviceStyle.PlayStation4;
		base.IncludePlatforms = new string[1] { "Linux" };
		JoystickNames = new string[1] { "Sony Computer Entertainment Wireless Controller" };
		base.ButtonMappings = new InputControlMapping[12]
		{
			new InputControlMapping
			{
				Handle = "Cross",
				Target = InputControlType.Action1,
				Source = UnityInputDeviceProfile.Button1
			},
			new InputControlMapping
			{
				Handle = "Circle",
				Target = InputControlType.Action2,
				Source = UnityInputDeviceProfile.Button2
			},
			new InputControlMapping
			{
				Handle = "Square",
				Target = InputControlType.Action3,
				Source = UnityInputDeviceProfile.Button0
			},
			new InputControlMapping
			{
				Handle = "Triangle",
				Target = InputControlType.Action4,
				Source = UnityInputDeviceProfile.Button3
			},
			new InputControlMapping
			{
				Handle = "Left Bumper",
				Target = InputControlType.LeftBumper,
				Source = UnityInputDeviceProfile.Button4
			},
			new InputControlMapping
			{
				Handle = "Right Bumper",
				Target = InputControlType.RightBumper,
				Source = UnityInputDeviceProfile.Button5
			},
			new InputControlMapping
			{
				Handle = "Options",
				Target = InputControlType.Options,
				Source = UnityInputDeviceProfile.Button9
			},
			new InputControlMapping
			{
				Handle = "Left Trigger",
				Target = InputControlType.LeftTrigger,
				Source = UnityInputDeviceProfile.Button6
			},
			new InputControlMapping
			{
				Handle = "Right Trigger",
				Target = InputControlType.RightTrigger,
				Source = UnityInputDeviceProfile.Button7
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
			},
			new InputControlMapping
			{
				Handle = "TouchPad Button",
				Target = InputControlType.TouchPadButton,
				Source = UnityInputDeviceProfile.Button13
			}
		};
		base.AnalogMappings = new InputControlMapping[12]
		{
			UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
			UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
			UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
			UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
			UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog2),
			UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog2),
			UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog6),
			UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog6),
			UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog7),
			UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog7)
		};
	}
}
