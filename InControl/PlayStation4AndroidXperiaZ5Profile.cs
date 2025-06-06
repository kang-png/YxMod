namespace InControl;

[AutoDiscover]
public class PlayStation4AndroidXperiaZ5Profile : UnityInputDeviceProfile
{
	public PlayStation4AndroidXperiaZ5Profile()
	{
		base.Name = "PlayStation 4 Controller";
		base.Meta = "PlayStation 4 Controller on Android";
		base.DeviceClass = InputDeviceClass.Controller;
		base.DeviceStyle = InputDeviceStyle.PlayStation4;
		base.IncludePlatforms = new string[1] { "Android" };
		base.ExcludePlatforms = new string[1] { "Amazon AFT" };
		JoystickNames = new string[1] { "Wireless Controller" };
		base.ButtonMappings = new InputControlMapping[12]
		{
			new InputControlMapping
			{
				Handle = "Cross",
				Target = InputControlType.Action1,
				Source = UnityInputDeviceProfile.Button0
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
				Source = UnityInputDeviceProfile.Button2
			},
			new InputControlMapping
			{
				Handle = "Triangle",
				Target = InputControlType.Action4,
				Source = UnityInputDeviceProfile.Button3
			},
			new InputControlMapping
			{
				Handle = "L1",
				Target = InputControlType.LeftBumper,
				Source = UnityInputDeviceProfile.Button4
			},
			new InputControlMapping
			{
				Handle = "L2",
				Target = InputControlType.LeftTrigger,
				Source = UnityInputDeviceProfile.Button6
			},
			new InputControlMapping
			{
				Handle = "R1",
				Target = InputControlType.RightBumper,
				Source = UnityInputDeviceProfile.Button5
			},
			new InputControlMapping
			{
				Handle = "R2",
				Target = InputControlType.RightTrigger,
				Source = UnityInputDeviceProfile.Button7
			},
			new InputControlMapping
			{
				Handle = "Share",
				Target = InputControlType.Share,
				Source = UnityInputDeviceProfile.Button11
			},
			new InputControlMapping
			{
				Handle = "Options",
				Target = InputControlType.Options,
				Source = UnityInputDeviceProfile.Button10
			},
			new InputControlMapping
			{
				Handle = "L3",
				Target = InputControlType.LeftStickButton,
				Source = UnityInputDeviceProfile.Button8
			},
			new InputControlMapping
			{
				Handle = "R3",
				Target = InputControlType.RightStickButton,
				Source = UnityInputDeviceProfile.Button9
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
			UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog3),
			UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog3),
			UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
		};
	}
}
