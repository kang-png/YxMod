using System.Collections.Generic;
using UnityEngine;

namespace InControl;

public class TestInputManager : MonoBehaviour
{
	public Font font;

	private GUIStyle style = new GUIStyle();

	private List<LogMessage> logMessages = new List<LogMessage>();

	private bool isPaused;

	private void OnEnable()
	{
		isPaused = false;
		Time.timeScale = 1f;
		Logger.OnLogMessage += delegate(LogMessage logMessage)
		{
			logMessages.Add(logMessage);
		};
		InputManager.OnDeviceAttached += delegate(InputDevice inputDevice)
		{
			Debug.Log("Attached: " + inputDevice.Name);
		};
		InputManager.OnDeviceDetached += delegate(InputDevice inputDevice)
		{
			Debug.Log("Detached: " + inputDevice.Name);
		};
		InputManager.OnActiveDeviceChanged += delegate(InputDevice inputDevice)
		{
			Debug.Log("Active device changed to: " + inputDevice.Name);
		};
		InputManager.OnUpdate += HandleInputUpdate;
	}

	private void HandleInputUpdate(ulong updateTick, float deltaTime)
	{
		CheckForPauseButton();
		int count = InputManager.Devices.Count;
		for (int i = 0; i < count; i++)
		{
			InputDevice inputDevice = InputManager.Devices[i];
			inputDevice.Vibrate(inputDevice.LeftTrigger, inputDevice.RightTrigger);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Utility.LoadScene("TestInputManager");
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			InputManager.Enabled = !InputManager.Enabled;
		}
	}

	private void CheckForPauseButton()
	{
		if (Input.GetKeyDown(KeyCode.P) || InputManager.CommandWasPressed)
		{
			Time.timeScale = ((!isPaused) ? 0f : 1f);
			isPaused = !isPaused;
		}
	}

	private void SetColor(Color color)
	{
		style.normal.textColor = color;
	}

	private void OnGUI()
	{
		int num = Mathf.FloorToInt(Screen.width / Mathf.Max(1, InputManager.Devices.Count));
		int num2 = 10;
		int num3 = 10;
		int num4 = 15;
		GUI.skin.font = font;
		SetColor(Color.white);
		string text = "Devices:";
		text = text + " (Platform: " + InputManager.Platform + ")";
		text = text + " " + InputManager.ActiveDevice.Direction.Vector;
		if (isPaused)
		{
			SetColor(Color.red);
			text = "+++ PAUSED +++";
		}
		GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text, style);
		SetColor(Color.white);
		foreach (InputDevice device in InputManager.Devices)
		{
			bool flag = InputManager.ActiveDevice == device;
			Color color = ((!flag) ? Color.white : Color.yellow);
			num3 = 35;
			if (device.IsUnknown)
			{
				SetColor(Color.red);
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "Unknown Device", style);
			}
			else
			{
				SetColor(color);
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), device.Name, style);
			}
			num3 += num4;
			SetColor(color);
			if (device.IsUnknown)
			{
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), device.Meta, style);
				num3 += num4;
			}
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "Style: " + device.DeviceStyle, style);
			num3 += num4;
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "GUID: " + device.GUID, style);
			num3 += num4;
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "SortOrder: " + device.SortOrder, style);
			num3 += num4;
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "LastChangeTick: " + device.LastChangeTick, style);
			num3 += num4;
			if (device is NativeInputDevice nativeInputDevice)
			{
				string text2 = $"VID = 0x{nativeInputDevice.Info.vendorID:x}, PID = 0x{nativeInputDevice.Info.productID:x}, VER = 0x{nativeInputDevice.Info.versionNumber:x}";
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text2, style);
				num3 += num4;
			}
			num3 += num4;
			foreach (InputControl control in device.Controls)
			{
				if (control != null && !Utility.TargetIsAlias(control.Target))
				{
					string arg = ((!device.IsKnown) ? control.Handle : $"{control.Target} ({control.Handle})");
					SetColor((!control.State) ? color : Color.green);
					string text3 = string.Format("{0} {1}", arg, (!control.State) ? string.Empty : ("= " + control.Value));
					GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text3, style);
					num3 += num4;
				}
			}
			num3 += num4;
			color = ((!flag) ? Color.white : new Color(1f, 0.7f, 0.2f));
			if (device.IsKnown)
			{
				InputControl command = device.Command;
				SetColor((!command.State) ? color : Color.green);
				string text4 = string.Format("{0} {1}", "Command", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				command = device.LeftStickX;
				SetColor((!command.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "Left Stick X", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				command = device.LeftStickY;
				SetColor((!command.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "Left Stick Y", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				SetColor((!device.LeftStick.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "Left Stick A", (!device.LeftStick.State) ? string.Empty : ("= " + device.LeftStick.Angle));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				command = device.RightStickX;
				SetColor((!command.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "Right Stick X", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				command = device.RightStickY;
				SetColor((!command.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "Right Stick Y", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				SetColor((!device.RightStick.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "Right Stick A", (!device.RightStick.State) ? string.Empty : ("= " + device.RightStick.Angle));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				command = device.DPadX;
				SetColor((!command.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "DPad X", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
				command = device.DPadY;
				SetColor((!command.State) ? color : Color.green);
				text4 = string.Format("{0} {1}", "DPad Y", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text4, style);
				num3 += num4;
			}
			SetColor(Color.cyan);
			InputControl anyButton = device.AnyButton;
			if ((bool)anyButton)
			{
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "AnyButton = " + anyButton.Handle, style);
			}
			num2 += num;
		}
		Color[] array = new Color[3]
		{
			Color.gray,
			Color.yellow,
			Color.white
		};
		SetColor(Color.white);
		num2 = 10;
		num3 = Screen.height - (10 + num4);
		for (int num5 = logMessages.Count - 1; num5 >= 0; num5--)
		{
			LogMessage logMessage = logMessages[num5];
			if (logMessage.type != 0)
			{
				SetColor(array[(int)logMessage.type]);
				string[] array2 = logMessage.text.Split('\n');
				foreach (string text5 in array2)
				{
					GUI.Label(new Rect(num2, num3, Screen.width, num3 + 10), text5, style);
					num3 -= num4;
				}
			}
		}
	}

	private void DrawUnityInputDebugger()
	{
		int num = 300;
		int num2 = Screen.width / 2;
		int num3 = 10;
		int num4 = 20;
		SetColor(Color.white);
		string[] joystickNames = Input.GetJoystickNames();
		int num5 = joystickNames.Length;
		for (int i = 0; i < num5; i++)
		{
			string text = joystickNames[i];
			int num6 = i + 1;
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "Joystick " + num6 + ": \"" + text + "\"", style);
			num3 += num4;
			string text2 = "Buttons: ";
			for (int j = 0; j < 20; j++)
			{
				string text3 = "joystick " + num6 + " button " + j;
				if (Input.GetKey(text3))
				{
					string text4 = text2;
					text2 = text4 + "B" + j + "  ";
				}
			}
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text2, style);
			num3 += num4;
			string text5 = "Analogs: ";
			for (int k = 0; k < 20; k++)
			{
				string axisName = "joystick " + num6 + " analog " + k;
				float axisRaw = Input.GetAxisRaw(axisName);
				if (Utility.AbsoluteIsOverThreshold(axisRaw, 0.2f))
				{
					string text4 = text5;
					text5 = text4 + "A" + k + ": " + axisRaw.ToString("0.00") + "  ";
				}
			}
			GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text5, style);
			num3 += num4;
			num3 += 25;
		}
	}

	private void OnDrawGizmos()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		Vector2 vector = activeDevice.Direction.Vector;
		Gizmos.color = Color.blue;
		Vector2 vector2 = new Vector2(-3f, -1f);
		Vector2 vector3 = vector2 + vector * 2f;
		Gizmos.DrawSphere(vector2, 0.1f);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawSphere(vector3, 1f);
		Gizmos.color = Color.red;
		Vector2 vector4 = new Vector2(3f, -1f);
		Vector2 vector5 = vector4 + activeDevice.RightStick.Vector * 2f;
		Gizmos.DrawSphere(vector4, 0.1f);
		Gizmos.DrawLine(vector4, vector5);
		Gizmos.DrawSphere(vector5, 1f);
	}
}
