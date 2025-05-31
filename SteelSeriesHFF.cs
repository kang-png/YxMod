using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class SteelSeriesHFF : MonoBehaviour
{
	private enum SSActionsEnum
	{
		game_metadata,
		remove_game,
		bind_game_event,
		register_game_event,
		game_event,
		game_heartbeat
	}

	private enum HFFEventsEnum
	{
		LEFT_GRAB,
		RIGHT_GRAB,
		BOTH_GRAB,
		RESPAWN,
		HIT_CHECKPOINT
	}

	public enum HumanEvents
	{
		LeftArm,
		RightArm,
		BothArms,
		Respawning,
		Checkpoint
	}

	[Serializable]
	public class CoreProps
	{
		public string address;

		public string encrypted_address;

		public string gg_encrypted_address;

		public string mercstealth_address;
	}

	[Serializable]
	public class RemoveGame
	{
		public string game;
	}

	[Serializable]
	public class HeartBeat
	{
		public string game;
	}

	[Serializable]
	public class RegisterGame
	{
		public string game;

		public string game_display_name;

		public string developer;

		public int deinitialize_timer_length_ms;
	}

	[Serializable]
	public class SendEvent
	{
		public string game;

		public string @event;

		public Data data;
	}

	[Serializable]
	public class Data
	{
		public int value;
	}

	[Serializable]
	public class Color
	{
		public int red;

		public int green;

		public int blue;
	}

	[Serializable]
	public class Rate
	{
		public int frequency;

		public int repeat_limit;
	}

	[Serializable]
	public class Handler
	{
		public string DeviceType;

		public List<int> CustomZoneKeys;

		public string zone;

		public Color color;

		public string mode;

		public Rate rate;
	}

	[Serializable]
	public class BindEvent
	{
		public string game;

		public string @event;

		public int min_value;

		public int max_value;

		public int icon_id;

		public List<Handler> handlers;
	}

	public class HandlerToAdd
	{
		public string device;

		public string zone;
	}

	private string address = string.Empty;

	private string gameName = "HUMANFALLFLAT";

	private string gameDisplayName = "Human Fall Flat";

	private string developerName = "L42";

	private static bool isSteelSeriesActive;

	private float heartbeatLastBeatTime;

	private float heartbeatPulse = 10f;

	private bool lastLeftVal;

	private bool lastRightVal;

	private bool lastBothArmsVal;

	private bool lastRespawning;

	private bool leftVal;

	private bool rightVal;

	private bool respawning;

	private bool celebrating;

	private float celebrateStart;

	private float celebrateLength = 2f;

	private float celebrateDeadTime = 3f;

	private List<string> SSActions = new List<string> { "game_metadata", "remove_game", "bind_game_event", "register_game_event", "game_event", "game_heartbeat" };

	private List<string> HFFEvents = new List<string> { "LEFT_GRAB", "RIGHT_GRAB", "BOTH_GRAB", "RESPAWN", "HIT_CHECKPOINT" };

	private List<int> KB_Left_Keys = new List<int> { 53, 43, 57, 225, 224 };

	private List<int> KB_Right_Keys = new List<int> { 42, 40, 229, 228 };

	private List<int> KB_Both_Keys = new List<int>
	{
		58, 59, 60, 61, 62, 63, 64, 65, 66, 67,
		68, 69, 41
	};

	private List<int> KB_FullKeyboard = new List<int>
	{
		4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
		14, 15, 16, 17, 18, 19, 20, 21, 22, 23,
		24, 25, 26, 27, 28, 29, 30, 31, 32, 33,
		34, 35, 36, 37, 38, 39, 227, 101, 226, 44,
		230, 231, 45, 46, 47, 48, 49, 50, 51, 52,
		54, 55, 56
	};

	private void OnEnable()
	{
		address = GetAddress();
		checkIsActive();
		if (isSteelSeriesActive)
		{
			Register_Game();
			List<HandlerToAdd> list = new List<HandlerToAdd>();
			list.Add(AddHandlerToList("rgb-per-key-zones", null));
			Bind_Event(HFFEvents[4], list, 255, 255, 255, KB_FullKeyboard, 5, 11);
			HandlerToAdd handlerToAdd = new HandlerToAdd();
			list = new List<HandlerToAdd>();
			list.Add(AddHandlerToList("rgb-2-zone", "one"));
			list.Add(AddHandlerToList("rgb-8-zone", "left"));
			list.Add(AddHandlerToList("rgb-per-key-zones", null));
			Bind_Event(HFFEvents[0], list, 0, 255, 0, KB_Left_Keys);
			list = new List<HandlerToAdd>();
			handlerToAdd = new HandlerToAdd();
			list.Add(AddHandlerToList("rgb-2-zone", "two"));
			list.Add(AddHandlerToList("rgb-8-zone", "right"));
			list.Add(AddHandlerToList("rgb-per-key-zones", null));
			Bind_Event(HFFEvents[1], list, 0, 255, 0, KB_Right_Keys);
			list = new List<HandlerToAdd>();
			handlerToAdd = new HandlerToAdd();
			list.Add(AddHandlerToList("headset", "earcups"));
			list.Add(AddHandlerToList("rgb-8-zone", "one"));
			list.Add(AddHandlerToList("rgb-per-key-zones", null));
			Bind_Event(HFFEvents[2], list, 0, 0, 255, KB_Both_Keys);
			list = new List<HandlerToAdd>();
			handlerToAdd = new HandlerToAdd();
			list.Add(AddHandlerToList("rgb-8-zone", "two"));
			list.Add(AddHandlerToList("rgb-per-key-zones", "keypad"));
			Bind_Event(HFFEvents[3], list, 255, 0, 0, null, 5);
		}
	}

	private HandlerToAdd AddHandlerToList(string device, string zone)
	{
		HandlerToAdd handlerToAdd = new HandlerToAdd();
		handlerToAdd.device = device;
		handlerToAdd.zone = zone;
		return handlerToAdd;
	}

	private void Start()
	{
		if (isSteelSeriesActive)
		{
			Event_Toggle(HFFEvents[2], value: false);
			Event_Toggle(HFFEvents[0], value: false);
			Event_Toggle(HFFEvents[1], value: false);
			Event_Toggle(HFFEvents[3], value: false);
			Event_Toggle(HFFEvents[4], value: false);
			DoHeartbeat();
		}
	}

	private void CreateAnimEvent()
	{
	}

	public void SteelSeriesEvent_LeftArm(bool isLeftArmRaised)
	{
		if (!isSteelSeriesActive)
		{
			return;
		}
		leftVal = isLeftArmRaised;
		if (leftVal != lastLeftVal)
		{
			if (leftVal & (rightVal != lastBothArmsVal))
			{
				HumanEvent(HumanEvents.BothArms, leftVal & rightVal);
			}
			else
			{
				HumanEvent(HumanEvents.LeftArm, leftVal);
			}
		}
		lastLeftVal = leftVal;
		lastBothArmsVal = leftVal & rightVal;
	}

	public void SteelSeriesEvent_RightArm(bool isRightArmRaised)
	{
		if (!isSteelSeriesActive)
		{
			return;
		}
		rightVal = isRightArmRaised;
		if (rightVal != lastRightVal)
		{
			if (leftVal & (rightVal != lastBothArmsVal))
			{
				HumanEvent(HumanEvents.BothArms, leftVal & rightVal);
			}
			else
			{
				HumanEvent(HumanEvents.RightArm, rightVal);
			}
		}
		lastRightVal = rightVal;
		lastBothArmsVal = leftVal & rightVal;
	}

	public void SteelSeriesEvent_Respawning(bool isRespawning)
	{
		if (isSteelSeriesActive)
		{
			respawning = isRespawning;
			if (respawning != lastRespawning)
			{
				HumanEvent(HumanEvents.Respawning, respawning);
			}
			lastRespawning = respawning;
		}
	}

	public void SteelSeriesEvent_CheckpointHit()
	{
		if (isSteelSeriesActive && Time.time > celebrateStart + celebrateDeadTime)
		{
			celebrateStart = Time.time;
			celebrating = true;
			HumanEvent(HumanEvents.Checkpoint, value: true);
		}
	}

	private void Update()
	{
		if (isSteelSeriesActive)
		{
			if (Time.time > heartbeatLastBeatTime + heartbeatPulse)
			{
				DoHeartbeat();
			}
			if (celebrating && Time.time > celebrateStart + celebrateLength)
			{
				celebrating = false;
				HumanEvent(HumanEvents.Checkpoint, value: false);
			}
		}
	}

	private void HumanEvent(HumanEvents currentEvent, bool value)
	{
		switch (currentEvent)
		{
		case HumanEvents.LeftArm:
			Event_Toggle(HFFEvents[2], value: false);
			Event_Toggle(HFFEvents[0], value);
			break;
		case HumanEvents.RightArm:
			Event_Toggle(HFFEvents[2], value: false);
			Event_Toggle(HFFEvents[1], value);
			break;
		case HumanEvents.BothArms:
			Event_Toggle(HFFEvents[0], value: false);
			Event_Toggle(HFFEvents[1], value: false);
			Event_Toggle(HFFEvents[2], value);
			break;
		case HumanEvents.Respawning:
			Event_Toggle(HFFEvents[3], value);
			break;
		case HumanEvents.Checkpoint:
			Event_Toggle(HFFEvents[4], value);
			break;
		}
	}

	private void Bind_Event(string eventName, List<HandlerToAdd> handlersToAdd, int r, int g, int b, List<int> customZoneKeys = null, int frequency = 0, int repeatLimit = -1)
	{
		BindEvent bindEvent = new BindEvent();
		bindEvent.game = gameName;
		bindEvent.@event = eventName;
		bindEvent.min_value = 0;
		bindEvent.max_value = 1;
		bindEvent.icon_id = 0;
		List<Handler> list = new List<Handler>();
		foreach (HandlerToAdd item in handlersToAdd)
		{
			Handler handler = new Handler();
			handler.DeviceType = item.device;
			if (customZoneKeys != null && handler.DeviceType == "rgb-per-key-zones")
			{
				handler.CustomZoneKeys = customZoneKeys;
			}
			handler.zone = item.zone;
			handler.color = new Color();
			handler.color.red = r;
			handler.color.green = g;
			handler.color.blue = b;
			handler.mode = "color";
			handler.rate = new Rate();
			handler.rate.frequency = frequency;
			if (repeatLimit >= 0)
			{
				handler.rate.repeat_limit = repeatLimit;
			}
			list.Add(handler);
		}
		bindEvent.handlers = list;
		string json = JsonUtility.ToJson(bindEvent).Replace("DeviceType", "device-type").Replace("CustomZoneKeys", "custom-zone-keys")
			.Replace(",\"custom-zone-keys\":[]", string.Empty);
		SendJSON(json, SSActions[2]);
	}

	private void DoHeartbeat()
	{
		HeartBeat heartBeat = new HeartBeat();
		heartBeat.game = gameName;
		heartbeatLastBeatTime = Time.time;
		SendJSON(JsonUtility.ToJson(heartBeat), SSActions[5]);
	}

	private void Register_Game()
	{
		RegisterGame registerGame = new RegisterGame();
		registerGame.developer = developerName;
		registerGame.game = gameName;
		registerGame.game_display_name = gameDisplayName;
		registerGame.deinitialize_timer_length_ms = 60000;
		SendJSON(JsonUtility.ToJson(registerGame), SSActions[0]);
	}

	private void Remove_Game(string gameToRemove = null)
	{
		if (gameToRemove == null)
		{
			gameToRemove = gameName;
		}
		RemoveGame removeGame = new RemoveGame();
		removeGame.game = gameToRemove;
		SendJSON(JsonUtility.ToJson(removeGame), SSActions[1]);
	}

	public void Event_Toggle(string eventName, bool value)
	{
		SendEvent sendEvent = new SendEvent();
		sendEvent.game = gameName;
		sendEvent.@event = eventName;
		sendEvent.data = new Data();
		sendEvent.data.value = Convert.ToInt32(value);
		string text = JsonUtility.ToJson(sendEvent);
		SendJSON(JsonUtility.ToJson(sendEvent), SSActions[4]);
	}

	private void SendJSON(string json, string action)
	{
		if (!isSteelSeriesActive)
		{
			return;
		}
		try
		{
			Uri requestUri = new Uri("http://" + address + "/" + action);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = 500;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(json);
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			string text = streamReader.ReadToEnd();
		}
		catch (Exception)
		{
			isSteelSeriesActive = false;
		}
	}

	private string GetAddress()
	{
		string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SteelSeries\\SteelSeries Engine 3\\coreProps.json";
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			CoreProps coreProps = JsonUtility.FromJson<CoreProps>(json);
			isSteelSeriesActive = true;
			WriteDebug("GetAddress - " + coreProps.address);
			return coreProps.address;
		}
		isSteelSeriesActive = false;
		WriteDebug("GetAddress - false");
		return string.Empty;
	}

	private void checkIsActive()
	{
		if (!isSteelSeriesActive)
		{
			return;
		}
		Uri requestUri = new Uri("http://" + address + "/game_metadata");
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.Method = "POST";
		try
		{
			httpWebRequest.Timeout = 500;
			Stream requestStream = httpWebRequest.GetRequestStream();
			isSteelSeriesActive = true;
			WriteDebug("checkIsActive - true");
		}
		catch (Exception)
		{
			isSteelSeriesActive = false;
			WriteDebug("checkIsActive - false");
		}
	}

	private void WriteDebug(string debug)
	{
	}
}
