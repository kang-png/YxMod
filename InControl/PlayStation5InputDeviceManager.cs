using System.Collections.Generic;

namespace InControl;

public class PlayStation5InputDeviceManager : InputDeviceManager
{
	private const int maxDevices = 4;

	private bool[] deviceConnected = new bool[4];

	public PlayStation5InputDeviceManager()
	{
		for (int i = 0; i < 4; i++)
		{
			devices.Add(new PlayStation5InputDevice(i));
		}
		UpdateInternal(0uL, 0f);
	}

	private void UpdateInternal(ulong updateTick, float deltaTime)
	{
		for (int i = 0; i < 4; i++)
		{
			PlayStation5InputDevice playStation5InputDevice = devices[i] as PlayStation5InputDevice;
			if (playStation5InputDevice.IsConnected != deviceConnected[i])
			{
				if (playStation5InputDevice.IsConnected)
				{
					InputManager.AttachDevice(playStation5InputDevice);
				}
				else
				{
					InputManager.DetachDevice(playStation5InputDevice);
				}
				deviceConnected[i] = playStation5InputDevice.IsConnected;
			}
		}
	}

	public override void Update(ulong updateTick, float deltaTime)
	{
		UpdateInternal(updateTick, deltaTime);
	}

	public override void Destroy()
	{
	}

	public static bool CheckPlatformSupport(ICollection<string> errors)
	{
		return true;
	}

	internal static bool Enable()
	{
		List<string> list = new List<string>();
		if (CheckPlatformSupport(list))
		{
			InputManager.AddDeviceManager<PlayStation5InputDeviceManager>();
			return true;
		}
		foreach (string item in list)
		{
			Logger.LogError(item);
		}
		return false;
	}
}
