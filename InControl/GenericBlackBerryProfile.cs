namespace InControl;

[AutoDiscover]
public class GenericBlackBerryProfile : UnityInputDeviceProfile
{
	public GenericBlackBerryProfile()
	{
		base.Name = "Generic Gamepad";
		base.Meta = "Generic Gamepad on BlackBerry";
		base.DeviceClass = InputDeviceClass.Controller;
		base.IncludePlatforms = new string[1] { "Blackberry" };
		JoystickNames = new string[1] { "BlackBerry Gamepad :: SteelSeries FREE" };
		base.ButtonMappings = new InputControlMapping[10]
		{
			new InputControlMapping
			{
				Handle = "A",
				Target = InputControlType.Action1,
				Source = UnityInputDeviceProfile.Button0
			},
			new InputControlMapping
			{
				Handle = "B",
				Target = InputControlType.Action2,
				Source = UnityInputDeviceProfile.Button1
			},
			new InputControlMapping
			{
				Handle = "X",
				Target = InputControlType.Action3,
				Source = UnityInputDeviceProfile.Button2
			},
			new InputControlMapping
			{
				Handle = "Y",
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
				Handle = "Left Stick Button",
				Target = InputControlType.LeftStickButton,
				Source = UnityInputDeviceProfile.Button8
			},
			new InputControlMapping
			{
				Handle = "Right Stick Button",
				Target = InputControlType.RightStickButton,
				Source = UnityInputDeviceProfile.Button9
			},
			new InputControlMapping
			{
				Handle = "Back",
				Target = InputControlType.Select,
				Source = UnityInputDeviceProfile.Button6
			},
			new InputControlMapping
			{
				Handle = "Start",
				Target = InputControlType.Start,
				Source = UnityInputDeviceProfile.Button7
			}
		};
		base.AnalogMappings = new InputControlMapping[14]
		{
			UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
			UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
			UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
			UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
			UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog3),
			UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog3),
			UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog6),
			UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog6),
			new InputControlMapping
			{
				Handle = "Left Trigger",
				Target = InputControlType.LeftTrigger,
				Source = UnityInputDeviceProfile.Analog8,
				SourceRange = InputRange.ZeroToMinusOne,
				TargetRange = InputRange.ZeroToOne
			},
			new InputControlMapping
			{
				Handle = "Right Trigger",
				Target = InputControlType.RightTrigger,
				Source = UnityInputDeviceProfile.Analog9,
				SourceRange = InputRange.ZeroToMinusOne,
				TargetRange = InputRange.ZeroToOne
			}
		};
	}
}
