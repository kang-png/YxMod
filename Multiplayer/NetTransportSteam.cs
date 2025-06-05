using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Steamworks;
using UnityEngine;

namespace Multiplayer;

public class NetTransportSteam : NetTransport
{
    private struct QueuedData
    {
        public CSteamID steamIDRemote;

        public byte[] buffer;

        public int bufferTier;

        public int bytesRead;
    }

    public class FriendInfo : ILobbyEntry
    {
        [Flags]
        public enum LobbyFlags
        {
            kJoinGameInProgress = 1,
            kLockLevel = 2,
            kInviteOnly = 4,
            kStarted = 8,
            kFull = 0x10
        }

        public const string kLobbyDataNet = "net";

        public const string kLobbyDataName = "name";

        public const string kLobbyDataVersion = "version";

        public const string kLobbyDataPlayersCurrent = "cp";

        public const string kLobbyDataPlayersMax = "mp";

        public const string kLobbyDataInviteOnly = "io";

        public const string kLobbyDataLevelID = "li";

        public const string kLobbyDataLevelType = "lt";

        public const string kLobbyDataLobbyTitle = "ll";

        public const string kLobbyDataFlags = "fl";

        internal string _name;

        public CSteamID steamId;

        internal CSteamID _lobbyId;

        public string version;

        public uint netcode;

        public uint playersCurrent;

        public uint playersMax;

        public ulong levelID;

        public bool inviteOnly;

        public WorkshopItemSource levelType;

        public string lobbyTitle;

        public uint flags;

        public const uint SupportedAttrs = 4160749568u;

        public const uint SupportedFlags = 31u;

        public string name()
        {
            return _name;
        }

        public object lobbyId()
        {
            return _lobbyId;
        }

        public bool isSameLobbyID(object otherlobbyID)
        {
            if (otherlobbyID == null)
            {
                return false;
            }
            return (CSteamID)otherlobbyID == _lobbyId;
        }

        public bool getDisplayInfo(out LobbyDisplayInfo info)
        {
            info = default(LobbyDisplayInfo);
            info.FeaturesMask = 4160749599u;
            info.Flags = flags & 0x1Fu;
            info.NumPlayersForDisplay = playersCurrent + 1;
            info.MaxPlayers = playersMax;
            info.LevelID = levelID;
            info.InviteOnly = inviteOnly;
            info.LevelType = levelType;
            info.LobbyTitle = ((lobbyTitle == null) ? string.Empty : lobbyTitle);
            return true;
        }

        public void setDisplayInfo(ref LobbyDisplayInfo info)
        {
            uint num = info.NumPlayersForDisplay - 1;
            uint num2 = info.FeaturesMask & 0x1Fu;
            flags = (flags & ~num2) | (info.Flags & num2);
            if ((info.FeaturesMask & 0x80000000u) != 0)
            {
                playersCurrent = num;
            }
            if ((info.FeaturesMask & 0x40000000u) != 0)
            {
                playersMax = info.MaxPlayers;
            }
            if ((info.FeaturesMask & 0x20000000u) != 0)
            {
                levelID = info.LevelID;
            }
            if ((info.FeaturesMask & 0x10000000u) != 0)
            {
                levelType = info.LevelType;
            }
            if ((info.FeaturesMask & 0x8000000u) != 0)
            {
                lobbyTitle = info.LobbyTitle;
            }
        }
    }

    private bool isServer;

    public CSteamID lobbyID;

    private volatile bool mKillThread;

    private const int kMaxMaxPacketSize = 2048;

    protected Callback<LobbyCreated_t> m_LobbyCreated;

    protected Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;

    protected Callback<LobbyEnter_t> m_LobbyEnter;

    protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;

    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    protected Callback<P2PSessionConnectFail_t> m_OnSessionConnectFail;

    protected Callback<P2PSessionRequest_t> m_OnSessionRequest;

    protected CallResult<LobbyMatchList_t> m_LobbyList;

    protected Callback<SteamServersDisconnected_t> m_SteamServersDisconnected;

    protected Callback<SteamServersConnected_t> m_SteamServersConnected;

    protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;

    private Thread thread;

    private Queue<QueuedData> reliableQueue = new Queue<QueuedData>();

    private object reliableQueueLock = new object();

    private uint multipartBufferOffset;

    private int multipartBufferTier;

    private byte[] multipartBuffer;

    private OnStartHostDelegate startHostCallback;

    private const string kLobbyIDPref = "LastLobby";

    private bool lobbyStatus;

    private List<CSteamID> remoteUsers = new List<CSteamID>();

    private const uint kPlayerLeftFlags = 6u;

    private CSteamID joiningServerAddress;

    private OnJoinGameDelegate joinGameCallback;

    private Action<List<ILobbyEntry>> onListLobbies;

    public OnLobbyDataUpdateDelegate lobbyDataCallback;

    public List<OnLobbyDataUpdateDelegate2> lobbyDataCallback2 = new List<OnLobbyDataUpdateDelegate2>();

    public OnGameOverlayActivationDelegate gameOverlayCallback;

    [NonSerialized]
    public static bool sSteamServersConnected = true;

    public static void RemoveOldPackets()
    {
        byte[] pubDest = new byte[2048];
        CSteamID psteamIDRemote = default(CSteamID);
        for (int i = 0; i < 2; i++)
        {
            uint pcubMsgSize = 0u;
            if (SteamNetworking.IsP2PPacketAvailable(out pcubMsgSize, i))
            {
                SteamNetworking.ReadP2PPacket(pubDest, pcubMsgSize, out var _, out psteamIDRemote, i);
            }
        }
    }

    private CSteamID GetConnectionID(NetHost host)
    {
        return (CSteamID)host.connection;
    }

    public override string GetMyName()
    {
        return SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID());
    }

    public override string getUserId(int localPlayerIndex)
    {
        return SteamUser.GetSteamID().ToString();
    }

    public override bool ConnectionEquals(object connection, NetHost host)
    {
        return GetConnectionID(host).Equals((CSteamID)connection);
    }

    public override object FetchLaunchInvitation()
    {
        return CheckCommandLineJoin();
    }

    public override void Init()
    {
        lobbyDataCallback = DataUpdateBridge;
        DestroyPreviousLobby();
        Coroutines.StartGlobalCoroutine(InitCoroutine());
    }

    private IEnumerator InitCoroutine()
    {
        m_LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        m_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        m_LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        m_LobbyList = CallResult<LobbyMatchList_t>.Create(OnLobbyList);
        m_SteamServersDisconnected = Callback<SteamServersDisconnected_t>.Create(OnSteamServersDisconnected);
        m_SteamServersConnected = Callback<SteamServersConnected_t>.Create(OnSteamServersConnected);
        m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivation);
        m_OnSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnSessionConnectFail);
        m_OnSessionRequest = Callback<P2PSessionRequest_t>.Create(OnSessionRequest);
        while (!SteamManager.Initialized)
        {
            yield return null;
        }
    }

    public override void StartThread()
    {
        lock (reliableQueueLock)
        {
            reliableQueue.Clear();
        }
        if (NetGame.instance.multithreading)
        {
            thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Start();
        }
    }

    public override void StopThread()
    {
        if (thread != null)
        {
            mKillThread = true;
            if (!thread.Join(500))
            {
                thread.Abort();
                thread.Join(1000);
            }
            thread = null;
        }
    }

    private void Worker()
    {
        try
        {
            while (NetGame.isNetStarted || NetGame.isNetStarting)
            {
                try
                {
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Debug.LogError("worker sleep: " + ex);
                }
                ReadPackets(multithreaded: true);
                if (mKillThread)
                {
                    break;
                }
            }
            thread = null;
            mKillThread = false;
        }
        catch (Exception ex2)
        {
            Debug.LogError("worker: " + ex2);
        }
    }

    private void ReadPackets(bool multithreaded)
    {
        for (int i = 0; i < 2; i++)
        {
            uint pcubMsgSize = 0u;
            while (SteamNetworking.IsP2PPacketAvailable(out pcubMsgSize, i) && (NetGame.isNetStarting || NetGame.isNetStarted))
            {
                int num = NetStream.CalculateTierForSize((int)pcubMsgSize);
                byte[] array = NetStream.AllocateBuffer(num);
                uint pcubMsgSize2 = 0u;
                if (SteamNetworking.ReadP2PPacket(array, pcubMsgSize, out pcubMsgSize2, out var psteamIDRemote, i))
                {
                    if (array[0] == 0)
                    {
                        int num2 = array[1] * 256 * 256 + array[2] * 256 + array[3];
                        if (multipartBuffer == null)
                        {
                            int num3 = NetStream.CalculateSizeForTier(0) - 4;
                            multipartBufferTier = NetStream.CalculateTierForSize((num2 + 1) * num3);
                            multipartBuffer = NetStream.AllocateBuffer(multipartBufferTier);
                            multipartBufferOffset = 0u;
                        }
                        Array.Copy(array, 4L, multipartBuffer, multipartBufferOffset, pcubMsgSize2 - 4);
                        multipartBufferOffset += pcubMsgSize2 - 4;
                        NetStream.ReleaseBuffer(num, array);
                        if (num2 == 0)
                        {
                            num = multipartBufferTier;
                            array = multipartBuffer;
                            pcubMsgSize2 = multipartBufferOffset;
                            multipartBuffer = null;
                        }
                        else
                        {
                            array = null;
                        }
                    }
                    if (array == null)
                    {
                        continue;
                    }
                    if (multithreaded && i == 0)
                    {
                        lock (reliableQueueLock)
                        {
                            reliableQueue.Enqueue(new QueuedData
                            {
                                steamIDRemote = psteamIDRemote,
                                buffer = array,
                                bufferTier = num,
                                bytesRead = (int)pcubMsgSize2
                            });
                        }
                    }
                    else
                    {
                        NetGame.instance.OnData(psteamIDRemote, array, num, (int)pcubMsgSize2);
                        NetStream.ReleaseBuffer(num, array);
                    }
                }
                else
                {
                    NetStream.ReleaseBuffer(num, array);
                }
            }
        }
        NetGame.instance.Flush();
    }

    private void HandleReliableQueue()
    {
        while (NetGame.isNetStarted || NetGame.isNetStarting)
        {
            QueuedData queuedData;
            lock (reliableQueueLock)
            {
                if (reliableQueue.Count == 0)
                {
                    break;
                }
                queuedData = reliableQueue.Dequeue();
            }
            NetGame.instance.OnData(queuedData.steamIDRemote, queuedData.buffer, queuedData.bufferTier, queuedData.bytesRead);
            NetStream.ReleaseBuffer(queuedData.bufferTier, queuedData.buffer);
        }
    }

    public static object CheckCommandLineJoin()
    {
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        if (commandLineArgs == null)
        {
            return null;
        }
        for (int i = 0; i < commandLineArgs.Length; i++)
        {
            if (commandLineArgs[i].Equals("+connect_lobby") && i + 1 < commandLineArgs.Length)
            {
                ulong ulSteamID = ulong.Parse(commandLineArgs[i + 1]);
                return new CSteamID(ulSteamID);
            }
        }
        return null;
    }

    public void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t param)
    {
        if (!lobbyID.Equals(param.m_steamIDLobby))
        {
            App.instance.AcceptInvite(param.m_steamIDLobby);
        }
    }

    public override bool CanSendInvite()
    {
        return lobbyID.m_SteamID != 0;
    }

    public override void SendInvite()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
    }

    private ELobbyType CalculateLobbyType()
    {
        return NetGame.friendly ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePublic;
    }

    public override void StartServer(OnStartHostDelegate callback, object sessionArgs)
    {
        isServer = true;
        startHostCallback = callback;
        Application.runInBackground = true;
        SteamMatchmaking.CreateLobby(CalculateLobbyType(), Options.lobbyMaxPlayers);
    }

    public override void SetJoinable(bool joinable, bool haveStarted)
    {
        if (lobbyID.IsValid())
        {
            SteamMatchmaking.SetLobbyJoinable(lobbyID, joinable);
        }
    }

    public override void UpdateLobbyType()
    {
        if (lobbyID.IsValid())
        {
            SteamMatchmaking.SetLobbyType(lobbyID, CalculateLobbyType());
            SteamMatchmaking.SetLobbyData(lobbyID, "io", Options.lobbyInviteOnly.ToString());
        }
    }

    public override void UpdateLobbyPlayers()
    {
        if (lobbyID.IsValid())
        {
            SteamMatchmaking.SetLobbyMemberLimit(lobbyID, Options.lobbyMaxPlayers);
        }
    }

    private void DestroyPreviousLobby()
    {
        try
        {
            string @string = PlayerPrefs.GetString("LastLobby", null);
            if (!string.IsNullOrEmpty(@string))
            {
                ulong ulSteamID = ulong.Parse(@string);
                CSteamID steamIDLobby = new CSteamID(ulSteamID);
                SteamMatchmaking.LeaveLobby(steamIDLobby);
                RemoveLobby();
            }
        }
        catch (Exception)
        {
        }
    }

    private void RemoveLobby()
    {
        PlayerPrefs.DeleteKey("LastLobby");
    }

    private void SavePreviousLobby(ulong lobbyID)
    {
        PlayerPrefs.SetString("LastLobby", lobbyID.ToString());
    }

    //public void UpdateUsersLobbyData(int userCount)
    //{
    //	SteamMatchmaking.SetLobbyData(lobbyID, "cp", userCount.ToString());
    //}
    public void UpdateUsersLobbyData(int userCount)
    {
        SteamMatchmaking.SetLobbyData(lobbyID, "cp", (UI_SheZhi.xujiarishu + userCount).ToString());//userCount.ToString()  ///虚假房间人数
        SteamMatchmaking.SetLobbyData(lobbyID, "name", $"★{UI_SheZhi.fangming}");///修改房名
        SteamMatchmaking.SetLobbyData(lobbyID, "ll", $"{UI_SheZhi.datingming}");
        SteamMatchmaking.SetLobbyData(lobbyID, "mp", (UI_SheZhi.xujiarishu + Options.lobbyMaxPlayers).ToString());///虚假最大人数
    }

    public override void UpdateServerLevel(ulong levelID, WorkshopItemSource levelType)
    {
        SteamMatchmaking.SetLobbyData(lobbyID, "li", levelID.ToString());
        CSteamID steamIDLobby = lobbyID;
        uint num = (uint)levelType;
        SteamMatchmaking.SetLobbyData(steamIDLobby, "lt", num.ToString());
    }

    public override void SetLobbyStatus(bool status)
    {
        lobbyStatus = status;
        UpdateOptionsLobbyData();
    }

    public override void UpdateLobbyTitle()
    {
        SteamMatchmaking.SetLobbyData(lobbyID, "ll", NetGame.instance.lobbyTitle);
    }

    public override int BuildLobbyFlagsFromOptions()
    {
        int num = 0;
        if (Options.lobbyLockLevel > 0)
        {
            num |= 2;
        }
        if (Options.lobbyJoinInProgress > 0)
        {
            num |= 1;
        }
        if (Options.lobbyInviteOnly > 0)
        {
            num |= 4;
        }
        if (lobbyStatus)
        {
            num |= 8;
        }
        return num;
    }

    public override void UpdateOptionsLobbyData()
    {
        int num = BuildLobbyFlagsFromOptions();
        if ((object)lobbyID != null && lobbyID.IsValid())
        {
            //SteamMatchmaking.SetLobbyData(lobbyID, "mp", Options.lobbyMaxPlayers.ToString());
            SteamMatchmaking.SetLobbyData(lobbyID, "mp", (UI_SheZhi.xujiarishu + Options.lobbyMaxPlayers).ToString());//Options.lobbyMaxPlayers.ToString()
            SteamMatchmaking.SetLobbyData(lobbyID, "fl", num.ToString());
            SteamMatchmaking.SetLobbyData(lobbyID, "io", Options.lobbyInviteOnly.ToString());
        }
    }

    public void OnLobbyCreated(LobbyCreated_t param)
    {
        if (startHostCallback != null)
        {
            if (param.m_eResult == EResult.k_EResultOK)
            {
                SavePreviousLobby(param.m_ulSteamIDLobby);
                lobbyID = new CSteamID(param.m_ulSteamIDLobby);
                //SteamMatchmaking.SetLobbyData(lobbyID, "name", SteamFriends.GetPersonaName());
                SteamMatchmaking.SetLobbyData(lobbyID, "name", $"★{UI_SheZhi.fangming}");////修改房名
				SteamMatchmaking.SetLobbyData(lobbyID, "ll", $"{UI_SheZhi.datingming}");
                SteamMatchmaking.SetLobbyData(lobbyID, "version", VersionDisplay.fullVersion);
                SteamMatchmaking.SetLobbyData(lobbyID, "net", VersionDisplay.netCode.ToString());
                UpdateOptionsLobbyData();
                //SteamMatchmaking.SetLobbyData(lobbyID, "cp", "0");
                SteamMatchmaking.SetLobbyData(lobbyID, "cp", $"{UI_SheZhi.xujiarishu}"); //UI_SheZhi.xujiarishu
                SteamMatchmaking.SetLobbyData(lobbyID, "li", NetGame.instance.currentLevel.ToString());
                CSteamID steamIDLobby = lobbyID;
                uint currentLevelType = (uint)NetGame.instance.currentLevelType;
                SteamMatchmaking.SetLobbyData(steamIDLobby, "lt", currentLevelType.ToString());
                // SteamMatchmaking.SetLobbyData(lobbyID, "ll", NetGame.instance.lobbyTitle);
                startHostCallback(null);
            }
            else
            {
                startHostCallback(param.m_eResult.ToString());
            }
            startHostCallback = null;
        }
    }

    public override int GetNumberRemoteUsers()
    {
        return remoteUsers.Count;
    }

    private void CheckForClientBecomingHost(LobbyChatUpdate_t param)
    {
        if (!NetGame.isServer && (param.m_rgfChatMemberStateChange & 6u) != 0)
        {
            CSteamID steamIDLobby = new CSteamID(param.m_ulSteamIDLobby);
            CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner(steamIDLobby);
            CSteamID steamID = SteamUser.GetSteamID();
            if (lobbyOwner == steamID)
            {
                SteamMatchmaking.LeaveLobby(steamIDLobby);
            }
        }
    }

    public void OnLobbyChatUpdate(LobbyChatUpdate_t param)
    {
        CheckForClientBecomingHost(param);
        CSteamID cSteamID = new CSteamID(param.m_ulSteamIDUserChanged);
        if (cSteamID == SteamUser.GetSteamID())
        {
            return;
        }
        EChatMemberStateChange rgfChatMemberStateChange = (EChatMemberStateChange)param.m_rgfChatMemberStateChange;
        if (rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            if (isServer && !remoteUsers.Contains(cSteamID))
            {
                NetGame.instance.OnConnect(cSteamID);
                remoteUsers.Add(cSteamID);
                UpdateUsersLobbyData(remoteUsers.Count);
            }
        }
        else if (remoteUsers.Contains(cSteamID))
        {
            remoteUsers.Remove(cSteamID);
            NetGame.instance.OnDisconnect(cSteamID);
            if (isServer)
            {
                UpdateUsersLobbyData(remoteUsers.Count);
            }
        }
    }

    public void OnSessionRequest(P2PSessionRequest_t param)
    {
        foreach (object kickedUser in NetGame.kickedUsers)
        {
            if (((CSteamID)kickedUser).Equals(param.m_steamIDRemote))
            {
                SteamNetworking.CloseP2PSessionWithUser(param.m_steamIDRemote);
                return;
            }
        }
        SteamNetworking.AcceptP2PSessionWithUser(param.m_steamIDRemote);
    }

    public override void JoinGame(object serverAddress, OnJoinGameDelegate calback)
    {
        isServer = false;
        joinGameCallback = calback;
        Application.runInBackground = true;
        RemoveOldPackets();
        SteamMatchmaking.JoinLobby((CSteamID)serverAddress);
        joiningServerAddress = (CSteamID)serverAddress;
    }

    public override void LobbyConnectedFixup()
    {
        if (joinGameCallback != null && joiningServerAddress.IsValid())
        {
            lobbyID = joiningServerAddress;
            joiningServerAddress = default(CSteamID);
        }
    }

    private bool HostIsInLobby(CSteamID lobbyID, CSteamID ownerID)
    {
        int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        for (int i = 0; i < numLobbyMembers; i++)
        {
            CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
            if (lobbyMemberByIndex == ownerID)
            {
                return true;
            }
        }
        return false;
    }

    public void OnLobbyEnter(LobbyEnter_t param)
    {
        if (joinGameCallback == null)
        {
            EChatRoomEnterResponse eChatRoomEnterResponse = (EChatRoomEnterResponse)param.m_EChatRoomEnterResponse;
            Debug.LogFormat("OnLobbyEnter:" + eChatRoomEnterResponse);
            return;
        }
        joiningServerAddress = default(CSteamID);
        if (param.m_EChatRoomEnterResponse == 1)
        {
            CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner(new CSteamID(param.m_ulSteamIDLobby));
            if (lobbyOwner != SteamUser.GetSteamID())
            {
                lobbyID = new CSteamID(param.m_ulSteamIDLobby);
                if (HostIsInLobby(lobbyID, lobbyOwner))
                {
                    remoteUsers.Add(lobbyOwner);
                    joinGameCallback(lobbyOwner, null);
                    joinGameCallback = null;
                    UpdateUsersLobbyData(remoteUsers.Count);
                    return;
                }
                param.m_EChatRoomEnterResponse = 2u;
            }
            else
            {
                param.m_EChatRoomEnterResponse = 2u;
            }
        }
        OnJoinGameDelegate onJoinGameDelegate = joinGameCallback;
        EChatRoomEnterResponse eChatRoomEnterResponse2 = (EChatRoomEnterResponse)param.m_EChatRoomEnterResponse;
        onJoinGameDelegate(null, eChatRoomEnterResponse2.ToString());
        joinGameCallback = null;
    }

    public override void StopServer()
    {
        LeaveGame();
    }

    private void LeaveLobby(ref CSteamID lobby)
    {
        SteamMatchmaking.LeaveLobby(lobby);
        RemoveLobby();
        lobby = default(CSteamID);
    }

    public override void LeaveGame()
    {
        try
        {
            StopThread();
        }
        catch
        {
        }
        if ((object)lobbyID != null && lobbyID.IsValid())
        {
            Physics.autoSimulation = true;
            LeaveLobby(ref lobbyID);
        }
        if ((object)joiningServerAddress != null && joiningServerAddress.IsValid())
        {
            LeaveLobby(ref joiningServerAddress);
        }
        remoteUsers.Clear();
        joinGameCallback = null;
        startHostCallback = null;
    }

    public override bool IsRelayed(NetHost host)
    {
        if (SteamNetworking.GetP2PSessionState(GetConnectionID(host), out var pConnectionState) && pConnectionState.m_bConnectionActive != 0)
        {
            return pConnectionState.m_bUsingRelay != 0;
        }
        return false;
    }

    public override void SendReliable(NetHost host, byte[] data, int len)
    {
        int num = NetStream.CalculateSizeForTier(0);
        if (len <= num)
        {
            SteamNetworking.SendP2PPacket(GetConnectionID(host), data, (uint)len, EP2PSend.k_EP2PSendReliable);
            return;
        }
        int num2 = 0;
        int num3 = num - 4;
        int num4 = len / num3;
        if (num4 * num3 < len)
        {
            num4++;
        }
        byte[] array = NetStream.AllocateBuffer(0);
        for (int i = 0; i < num4; i++)
        {
            int num5 = num4 - i - 1;
            array[0] = 0;
            array[1] = (byte)(num5 / 256 / 256);
            array[2] = (byte)(num5 / 256 % 256);
            array[3] = (byte)(num5 % 256);
            if (num2 + num3 > len)
            {
                num3 = len - num2;
            }
            Array.Copy(data, num2, array, 4, num3);
            if (!SteamNetworking.SendP2PPacket(GetConnectionID(host), array, (uint)(num3 + 4), EP2PSend.k_EP2PSendReliable))
            {
                Debug.LogErrorFormat("Failed to send packet {0} of {1}", i, num4);
            }
            num2 += num3;
        }
        NetStream.ReleaseBuffer(0, array);
    }

    public override void SendUnreliable(NetHost host, byte[] data, int len)
    {
        if (!NetGame.instance.multithreading)
        {
        }
        if (!SteamNetworking.SendP2PPacket(GetConnectionID(host), data, (uint)len, EP2PSend.k_EP2PSendUnreliableNoDelay, 1))
        {
            Debug.LogError("SendP2PPacket failed - packet too big?");
        }
        if (NetGame.instance.multithreading)
        {
        }
    }

    private void OnSessionConnectFail(P2PSessionConnectFail_t param)
    {
        Debug.LogError("OnSessionConnectFail ( " + param.m_steamIDRemote.ToString() + " ) " + (EP2PSessionError)param.m_eP2PSessionError);
        if (SteamNetworking.GetP2PSessionState(param.m_steamIDRemote, out var pConnectionState))
        {
            Debug.LogError("OnSessionConnectFail details : ");
            Debug.LogError(" *     m_bConnectionActive : " + pConnectionState.m_bConnectionActive);
            Debug.LogError(" *           m_bConnecting : " + pConnectionState.m_bConnecting);
            Debug.LogError(" *           m_bUsingRelay : " + pConnectionState.m_bUsingRelay);
            Debug.LogError(" *   m_nBytesQueuedForSend : " + pConnectionState.m_nBytesQueuedForSend);
            Debug.LogError(" * m_nPacketsQueuedForSend : " + pConnectionState.m_nPacketsQueuedForSend);
            Debug.LogError(" *             m_nRemoteIP : " + pConnectionState.m_nRemoteIP);
            Debug.LogError(" *           m_nRemotePort : " + pConnectionState.m_nRemotePort);
        }
        else
        {
            Debug.LogError("OnSessionConnectFail details : FAILED TO GET CONNECTION STATE ");
        }
        if (NetGame.isClient)
        {
            //App.instance.OnLostConnection();
            Debug.Log("与服务器的连接已丢失");/////修改
        }
    }

    public override void OnUpdate()
    {
        if (NetGame.instance.multithreading)
        {
            HandleReliableQueue();
        }
        else
        {
            ReadPackets(multithreaded: false);
        }
    }

    public override uint GetSupportedLobbyData()
    {
        return 4160749599u;
    }

    public override void RequestLobbyDataRefresh(ILobbyEntry lobbyEntry, bool inSession)
    {
        if (lobbyEntry is FriendInfo friendInfo)
        {
            SteamMatchmaking.RequestLobbyData((CSteamID)friendInfo.lobbyId());
        }
        else
        {
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    public override float GetLobbyDataRefreshThrottleTime()
    {
        return 2f;
    }

    public override bool SupportsLobbyListings()
    {
        return true;
    }

    public override void ListLobbies(Action<List<ILobbyEntry>> onListLobbies)
    {
        if (NetGame.friendly)
        {
            this.onListLobbies = null;
            CGameID cGameID = new CGameID(SteamUtils.GetAppID());
            List<ILobbyEntry> list = new List<ILobbyEntry>();
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                if (SteamFriends.GetFriendGamePlayed(friendByIndex, out var pFriendGameInfo) && !(pFriendGameInfo.m_gameID != cGameID) && pFriendGameInfo.m_steamIDLobby.IsValid())
                {
                    FriendInfo friendInfo = new FriendInfo();
                    friendInfo.steamId = friendByIndex;
                    friendInfo._lobbyId = pFriendGameInfo.m_steamIDLobby;
                    friendInfo._name = SteamFriends.GetFriendPersonaName(friendByIndex);
                    FriendInfo friend = friendInfo;
                    SteamMatchmaking.RequestLobbyData(pFriendGameInfo.m_steamIDLobby);
                    GetNewLobbyData(pFriendGameInfo.m_steamIDLobby, ref friend);
                    if (!friend.inviteOnly)
                    {
                        list.Add(friend);
                    }
                }
            }
            onListLobbies(list);
        }
        else
        {
            this.onListLobbies = onListLobbies;
            SteamAPICall_t hAPICall = SteamMatchmaking.RequestLobbyList();
            m_LobbyList.Set(hAPICall);
        }
    }

    public void StopList()
    {
        m_LobbyList.Cancel();
    }

    private void GetNewLobbyData(CSteamID lobbyID, ref FriendInfo friend)
    {
        uint result = 0u;
        uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "mp"), out result);
        uint result2 = 0u;
        uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "cp"), out result2);
        ulong result3 = 0uL;
        ulong.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "li"), out result3);
        uint result4 = 0u;
        uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "io"), out result4);
        WorkshopItemSource workshopItemSource = WorkshopItemSource.BuiltIn;
        uint result5 = 0u;
        uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "lt"), out result5);
        workshopItemSource = (WorkshopItemSource)result5;
        uint result6 = 0u;
        uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "fl"), out result6);
        friend.playersMax = result;
        friend.playersCurrent = result2;
        friend.levelID = result3;
        friend.inviteOnly = result4 == 1;
        friend.levelType = workshopItemSource;
        friend.lobbyTitle = SteamMatchmaking.GetLobbyData(lobbyID, "ll");
        friend.flags = result6;
    }

    public void OnLobbyList2(LobbyMatchList_t param, bool bIOFailure)
    {
        if (onListLobbies == null)
        {
            return;
        }
        List<ILobbyEntry> list = new List<ILobbyEntry>();
        for (int i = 0; i < param.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
            uint result = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyByIndex, "net"), out result);
            uint result2 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyByIndex, "io"), out result2);
            if (result == VersionDisplay.netCode)
            {
                CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner(lobbyByIndex);
                string lobbyData = SteamMatchmaking.GetLobbyData(lobbyByIndex, "name");
                string lobbyData2 = SteamMatchmaking.GetLobbyData(lobbyByIndex, "version");
                FriendInfo friendInfo = new FriendInfo();
                friendInfo.steamId = lobbyOwner;
                friendInfo._lobbyId = lobbyByIndex;
                friendInfo._name = lobbyData;
                friendInfo.inviteOnly = result2 == 1;
                friendInfo.version = lobbyData2;
                friendInfo.netcode = result;
                FriendInfo friend = friendInfo;
                GetNewLobbyData(lobbyByIndex, ref friend);
                if (!friend.inviteOnly)
                {
                    list.Add(friend);
                }
            }
        }
        onListLobbies(list);
        onListLobbies = null;
    }
    public void OnLobbyList(LobbyMatchList_t param, bool bIOFailure)
    {
        if (onListLobbies == null)
        {
            return;
        }
        //好友房
        List<ILobbyEntry> haoyoulist = new List<ILobbyEntry>();
        if (UI_SheZhi.danyexianshi)
        {
            //this.onListLobbies = null;
            CGameID cGameID = new CGameID(SteamUtils.GetAppID());

            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                if (SteamFriends.GetFriendGamePlayed(friendByIndex, out var pFriendGameInfo) && !(pFriendGameInfo.m_gameID != cGameID) && pFriendGameInfo.m_steamIDLobby.IsValid())
                {
                    FriendInfo friendInfo = new FriendInfo();
                    friendInfo.steamId = friendByIndex;
                    friendInfo._lobbyId = pFriendGameInfo.m_steamIDLobby;
                    friendInfo._name = (UI_SheZhi.haoyoufangjian ? "@" : "") + SteamFriends.GetFriendPersonaName(friendByIndex);
                    FriendInfo friend = friendInfo;
                    SteamMatchmaking.RequestLobbyData(pFriendGameInfo.m_steamIDLobby);
                    GetNewLobbyData(pFriendGameInfo.m_steamIDLobby, ref friend);
                    //if (!friend.inviteOnly)
                    //{
                    haoyoulist.Add(friend);
                    //}
                }
            }
            if (UI_SheZhi.dangqianrenshupaixu)
            {
                // 对yxmodlist进行排序
                haoyoulist.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
            }


            //onListLobbies(list);
            //好友房
        }



        List<ILobbyEntry> list = new List<ILobbyEntry>();
        List<ILobbyEntry> similist = new List<ILobbyEntry>();///增加私密房间列表
		List<ILobbyEntry> yxmodlist = new List<ILobbyEntry>();///
        for (int i = 0; i < param.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
            uint result = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyByIndex, "net"), out result);
            uint result2 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyByIndex, "io"), out result2);
            if (result == VersionDisplay.netCode)
            {
                CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner(lobbyByIndex);
                string lobbyData = SteamMatchmaking.GetLobbyData(lobbyByIndex, "name");
                string lobbyData2 = SteamMatchmaking.GetLobbyData(lobbyByIndex, "version");
                string lobbyData3 = SteamMatchmaking.GetLobbyData(lobbyByIndex, "cp");
                FriendInfo friendInfo = new FriendInfo();
                friendInfo.steamId = lobbyOwner;
                friendInfo._lobbyId = lobbyByIndex;
                friendInfo._name = (UI_SheZhi.simifangjian ? (result2 == 1 ? "*" : "") : "") + lobbyData; ////修改
                //friendInfo._name = lobbyData;
                friendInfo.inviteOnly = result2 == 1;
                friendInfo.version = lobbyData2;
                friendInfo.netcode = result;
                FriendInfo friend = friendInfo;
                GetNewLobbyData(lobbyByIndex, ref friend);
                //friend.playersCurrent
                if (friend._name.StartsWith("★") || friend._name.StartsWith("*★"))
                {
                    yxmodlist.Add(friend);
                }

                else if (!friend.inviteOnly) ////正常房间
                {
                    list.Add(friend);
                }
                else if (friend.inviteOnly)////仅限邀请房间
                {
                    similist.Add(friend);
                }
            }
        }
        if (UI_SheZhi.dangqianrenshupaixu)
        {
            // 对yxmodlist进行排序
            yxmodlist.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
            // 对list进行排序
            list.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
            // 对similist进行排序
            similist.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
        }

        List<ILobbyEntry> combinedList = new List<ILobbyEntry>();
        //yxmodlist = yxmodlist.OrderByDescending(friend => friend.playersCurrent).ToList();
        combinedList.AddRange(yxmodlist); // 添加yxmodlist中的所有元素到combinedList
        if (UI_SheZhi.danyexianshi)
        {
            combinedList.AddRange(haoyoulist); // 添加yxmodlist中的所有元素到combinedList
        }

        combinedList.AddRange(list);      // 添加list中的所有元素到combinedList
        if (UI_SheZhi.simifangjian)
        {
            combinedList.AddRange(similist); // 添加similist中的所有元素到combinedList
        }


        onListLobbies(combinedList);
        onListLobbies = null;
    }
    private void OnSteamServersConnected(SteamServersConnected_t pCallback)
    {
        Debug.Log("[" + 101 + " - SteamServersConnected] - " + pCallback);
        sSteamServersConnected = true;
    }

    private void OnSteamServersDisconnected(SteamServersDisconnected_t pCallback)
    {
        Debug.Log("[" + 103 + " - SteamServersDisconnected] - " + pCallback.m_eResult);
        sSteamServersConnected = false;
        if (NetGame.isServer)
        {
            App.instance.OnLostConnection();
        }
    }

    public override void RegisterForGameOverlayActivation(OnGameOverlayActivationDelegate callback)
    {
        base.RegisterForGameOverlayActivation(callback);
        gameOverlayCallback = callback;
    }

    public override void RegisterForLobbyData(OnLobbyDataUpdateDelegate2 callback)
    {
        if (callback != null)
        {
            lobbyDataCallback2.Add(callback);
        }
    }

    public override void UnregisterForLobbyData(OnLobbyDataUpdateDelegate2 callback)
    {
        if (lobbyDataCallback2.Contains(callback))
        {
            lobbyDataCallback2.Remove(callback);
        }
    }

    private void DataUpdateBridge(object lobbyID, uint playersMax, uint playersCurrent, ulong levelID, WorkshopItemSource levelType, string lobbyTitle, uint flags, bool error)
    {
        if (lobbyDataCallback2 == null || lobbyDataCallback2.Count == 0)
        {
            return;
        }
        LobbyDisplayInfo dispInfo = default(LobbyDisplayInfo);
        dispInfo.InitBlank();
        if (!error)
        {
            dispInfo.FeaturesMask = 4160749599u;
            dispInfo.Flags = flags & 0x1Fu;
            dispInfo.NumPlayersForDisplay = playersCurrent + 1;
            dispInfo.MaxPlayers = playersMax;
            dispInfo.LevelID = levelID;
            dispInfo.LevelType = levelType;
            dispInfo.LobbyTitle = ((lobbyTitle == null) ? string.Empty : lobbyTitle);
        }
        foreach (OnLobbyDataUpdateDelegate2 item in lobbyDataCallback2)
        {
            item?.Invoke(lobbyID, dispInfo, error);
        }
    }

    private void OnGameOverlayActivation(GameOverlayActivated_t activated)
    {
        if (gameOverlayCallback != null)
        {
            gameOverlayCallback(activated.m_bActive);
        }
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t lobbyData)
    {
        uint result = 0u;
        uint result2 = 0u;
        ulong result3 = 0uL;
        WorkshopItemSource levelType = WorkshopItemSource.BuiltIn;
        string levelTitle = string.Empty;
        uint result4 = 0u;
        bool error = true;
        CSteamID cSteamID = new CSteamID(lobbyData.m_ulSteamIDLobby);
        if (lobbyData.m_bSuccess > 0)
        {
            result = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(cSteamID, "mp"), out result);
            result2 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(cSteamID, "cp"), out result2);
            result3 = 0uL;
            ulong.TryParse(SteamMatchmaking.GetLobbyData(cSteamID, "li"), out result3);
            uint result5 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(cSteamID, "lt"), out result5);
            levelType = (WorkshopItemSource)result5;
            result4 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(cSteamID, "fl"), out result4);
            levelTitle = SteamMatchmaking.GetLobbyData(cSteamID, "ll");
            error = false;
        }
        if (lobbyDataCallback != null)
        {
            lobbyDataCallback(cSteamID, result, result2, result3, levelType, levelTitle, result4, error);
        }
    }
}
