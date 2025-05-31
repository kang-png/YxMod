using HumanAPI;
using I2.Loc;
using Multiplayer;
using Steamworks;
using UnityEngine;

public class SteamRichPresence : MonoBehaviour
{
	private const string GAME_MODE_KEY = "steam_display";

	private const string PLAYER_GROUP_KEY = "steam_player_group";

	private const string ROOM_SIZE_KEY = "steam_player_group_size";

	private int levelNumber;

	private WorkshopItemSource levelType;

	private bool verbose = true;

	private AppSate previousState;

	private int cachedOnlinePlayerCount = -1;

	private int cachedMaxPlayerCount = -1;

	private int currentMaxPlayerCount = 8;

	private bool isRegisteredForLobbyUpdates;

	private void Start()
	{
	}

	private void Update()
	{
		AppSate state = App.state;
		if (!(Game.instance == null) && !(WorkshopRepository.instance == null) && (previousState != state || (IsOnline(state) && (cachedOnlinePlayerCount != GetOnlinePlayerCount() || cachedMaxPlayerCount != currentMaxPlayerCount))))
		{
			levelNumber = Game.instance.currentLevelNumber;
			levelType = Game.instance.currentLevelType;
			currentMaxPlayerCount = ((!IsHost(state)) ? currentMaxPlayerCount : Options.lobbyMaxPlayers);
			RegisterForLobbyDataIfNecessary(state);
			SetGameModeTitle(state);
			SetDataToGroupFriends(state);
			previousState = state;
			cachedOnlinePlayerCount = GetOnlinePlayerCount();
			cachedMaxPlayerCount = currentMaxPlayerCount;
		}
	}

	private void SetGameModeTitle(AppSate state)
	{
		switch (state)
		{
		case AppSate.Startup:
		case AppSate.Menu:
		case AppSate.Customize:
		case AppSate.LoadLevel:
			SetGameMode("#Menu");
			break;
		case AppSate.PlayLevel:
			if (levelNumber < 0)
			{
				break;
			}
			if (levelNumber != -1)
			{
				if (levelType != 0 && levelType != WorkshopItemSource.EditorPick)
				{
					break;
				}
				WorkshopRepository.instance.levelRepo.GetLevel((ulong)levelNumber, levelType, delegate(WorkshopLevelMetadata metadata)
				{
					if (metadata != null && metadata.cachedThumbnail != null)
					{
						SetLevelName(metadata.cachedThumbnail.name);
						SetGameMode("#Local_Level");
					}
					else
					{
						Debug.LogError($"[Steamworks:RichPresence] Level meta data not found for level number {levelNumber} of type {levelType}");
					}
				});
			}
			else if (levelType == WorkshopItemSource.Subscription || levelType == WorkshopItemSource.LocalWorkshop)
			{
				SetLevelName(ScriptLocalization.Get("SRP/WorkshopContent"));
				SetGameMode("#Local_Level");
			}
			else
			{
				Debug.LogError($"[Steamworks:RichPresence] Invalid state {levelType} in {state}");
			}
			break;
		case AppSate.ServerHost:
		case AppSate.ClientJoin:
			SetGameMode("#Online_Joining");
			break;
		case AppSate.ServerLoadLobby:
		case AppSate.ServerLobby:
		case AppSate.ClientLoadLobby:
		case AppSate.ClientLobby:
			if (NetGame.instance == null)
			{
				Debug.LogError("[Steamworks:RichPresence] Invalid netgame instance");
			}
			else if (Game.multiplayerLobbyLevel < 128)
			{
				string lobbyNameEnglish = WorkshopRepository.GetLobbyNameEnglish(Game.multiplayerLobbyLevel);
				if (!string.IsNullOrEmpty(lobbyNameEnglish))
				{
					SetLevelName(lobbyNameEnglish);
					SetPlayerCount(GetOnlinePlayerCount(), currentMaxPlayerCount);
					SetGameMode("#Online_Lobby");
				}
				else
				{
					Debug.LogError("[Steamworks:RichPresence] Lobby name not found for Game.multiplayerLobbyLevel");
				}
			}
			else
			{
				SetLevelName(ScriptLocalization.Get("SRP/WorkshopContent"));
				SetPlayerCount(GetOnlinePlayerCount(), currentMaxPlayerCount);
				SetGameMode("#Online_Lobby");
			}
			break;
		case AppSate.ServerLoadLevel:
		case AppSate.ServerPlayLevel:
		case AppSate.ClientLoadLevel:
		case AppSate.ClientWaitServerLoad:
		case AppSate.ClientPlayLevel:
			if (levelType == WorkshopItemSource.BuiltIn || levelType == WorkshopItemSource.EditorPick)
			{
				if (levelNumber == -1)
				{
					break;
				}
				WorkshopRepository.instance.levelRepo.GetLevel((ulong)levelNumber, levelType, delegate(WorkshopLevelMetadata metadata)
				{
					if (metadata != null && metadata.cachedThumbnail != null)
					{
						SetLevelName(metadata.cachedThumbnail.name);
						SetPlayerCount(GetOnlinePlayerCount(), currentMaxPlayerCount);
						SetGameMode("#Online_Level");
					}
					else
					{
						Debug.LogError($"[Steamworks:RichPresence] Level meta data not found for level number {levelNumber} of type {levelType}");
					}
				});
			}
			else if (levelType == WorkshopItemSource.Subscription || levelType == WorkshopItemSource.LocalWorkshop)
			{
				SetLevelName(ScriptLocalization.Get("SRP/WorkshopContent"));
				SetPlayerCount(GetOnlinePlayerCount(), currentMaxPlayerCount);
				SetGameMode("#Online_Level");
			}
			else
			{
				Debug.LogError($"[Steamworks:RichPresence] Invalid state {levelType} in {state}");
			}
			break;
		default:
			Debug.LogError("[Steamworks:RichPresence] App state unclear");
			break;
		}
	}

	private void SetDataToGroupFriends(AppSate state)
	{
		if (IsOnline(state))
		{
			if (NetGame.instance != null)
			{
				NetTransportSteam netTransportSteam = (NetTransportSteam)NetGame.instance.transport;
				if (Human.Localplayer == null || Human.Localplayer.player == null || netTransportSteam == null)
				{
					Debug.LogError("[Steamworks:RichPresence] Invalid assumption??");
				}
				else
				{
					SetGroupInfo(netTransportSteam.lobbyID.ToString(), GetOnlinePlayerCount());
				}
			}
			else
			{
				ClearMultiplayerInfo();
			}
		}
		else
		{
			ClearMultiplayerInfo();
		}
	}

	private void RegisterForLobbyDataIfNecessary(AppSate state)
	{
		if (IsOnline(state))
		{
			if (!isRegisteredForLobbyUpdates)
			{
				NetGame.instance.transport.RegisterForLobbyData(OnLobbyDataUpdate);
				isRegisteredForLobbyUpdates = true;
			}
		}
		else if (isRegisteredForLobbyUpdates)
		{
			NetGame.instance.transport.UnregisterForLobbyData(OnLobbyDataUpdate);
			isRegisteredForLobbyUpdates = false;
		}
	}

	private void OnLobbyDataUpdate(object lobbyID, NetTransport.LobbyDisplayInfo dispInfo, bool error)
	{
		if (error)
		{
			Debug.LogError("[Steamworks:RichPresence] Invalid data from Lobby update ??");
		}
		else
		{
			currentMaxPlayerCount = (int)dispInfo.MaxPlayers;
		}
	}

	private bool IsOnline(AppSate state)
	{
		switch (state)
		{
		case AppSate.ServerLoadLobby:
		case AppSate.ServerLobby:
		case AppSate.ServerLoadLevel:
		case AppSate.ServerPlayLevel:
		case AppSate.ClientLoadLobby:
		case AppSate.ClientLobby:
		case AppSate.ClientLoadLevel:
		case AppSate.ClientWaitServerLoad:
		case AppSate.ClientPlayLevel:
			return true;
		default:
			return false;
		}
	}

	private bool IsHost(AppSate state)
	{
		switch (state)
		{
		case AppSate.ServerLoadLobby:
		case AppSate.ServerLobby:
		case AppSate.ServerLoadLevel:
		case AppSate.ServerPlayLevel:
			return true;
		default:
			return false;
		}
	}

	private int GetOnlinePlayerCount()
	{
		return (NetGame.instance != null) ? NetGame.instance.players.Count : 0;
	}

	private void SetLevelName(string englishValue)
	{
		SteamFriends.SetRichPresence("LevelName", englishValue);
		if (verbose)
		{
			Debug.Log(string.Format("[Steamworks:RichPresence] {0}:{1}", "LevelName", englishValue));
		}
	}

	private void SetGameMode(string token)
	{
		SteamFriends.SetRichPresence("steam_display", token);
		if (verbose)
		{
			Debug.Log(string.Format("[Steamworks:RichPresence] {0}:{1}", "steam_display", token));
		}
	}

	private void SetPlayerCount(int current, int max)
	{
		string text = $"({current}/{max})";
		SteamFriends.SetRichPresence("PlayerCount", text);
		if (verbose)
		{
			Debug.Log(string.Format("[Steamworks:RichPresence] {0}:{1}", "PlayerCount", text));
		}
	}

	private void SetGroupInfo(string lobbyID, int playerCount)
	{
		SteamFriends.SetRichPresence("steam_player_group", lobbyID);
		SteamFriends.SetRichPresence("steam_player_group_size", playerCount.ToString());
		if (verbose)
		{
			Debug.Log(string.Format("[Steamworks:RichPresence] {0}:{1};{2}:{3}", "steam_player_group", lobbyID, "steam_player_group_size", playerCount));
		}
	}

	private void ClearMultiplayerInfo()
	{
		SteamFriends.SetRichPresence("steam_player_group", null);
		SteamFriends.SetRichPresence("steam_player_group_size", null);
	}
}
