using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNetwork : MonoBehaviour
{
    public delegate void EventCallback(Events code, object content, int senderId);

    public enum Error
    {
        FailedToConnectToPhoton,
        ConnectionFail,
        Disconnected,
        PhotonCreateRoomFailed,
        CustomAuthenticationFailed,
        PhotonJoinRoomFailed,
    }

    public enum Events : byte
    {
        Start = 1,
        KeepAlive = 2,
        Movement = 3,
        Rotation = 4,
    }

    #region Network Mono
    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnReceivedEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnReceivedEvent;
    }

    private void Start()
    {
        PhotonNetwork.PhotonServerSettings.ServerAddress = GlobalConfig.Photon.address;
        PhotonNetwork.networkingPeer.DisconnectTimeout = 2000;
        PhotonNetwork.BackgroundTimeout = 5;
    }
    #endregion



    #region Network Starting
    public void OnConnectedToMaster()
    {
        if (IsOffline == false)
        {
            Debug.LogWarning("countOfPlayers: " + PhotonNetwork.countOfPlayers);
            print("countOfPlayersInRooms: " + PhotonNetwork.countOfPlayersInRooms);
            print("countOfPlayersOnMaster: " + PhotonNetwork.countOfPlayersOnMaster);
            print("countOfRooms: " + PhotonNetwork.countOfRooms);

            eloScoreMaxGap = Mathf.Max(GlobalConfig.MatchMaking.eloScoreCount, Mathf.RoundToInt(GlobalConfig.MatchMaking.eloScoreParams.x * EloScore + GlobalConfig.MatchMaking.eloScoreParams.y));
            eloScoreGap = eloScoreMaxGap / GlobalConfig.MatchMaking.eloScoreCount;
            eloPowerMaxGap = Mathf.RoundToInt(GlobalConfig.MatchMaking.eloPowerParams.x * EloPower + GlobalConfig.MatchMaking.eloPowerParams.y);

            joinGap = eloScoreGap;
            JoinRoom();
        }
        else CreateRoom();
    }

    private void JoinRoom()
    {
        var sqlLobbyFilter = string.Format("C0 = \"{0}\" AND C1 >= {1} AND C1 <= {2} AND C2 >= {3} AND C2 <= {4}",
            VersionedName,
            EloScore - joinGap, EloScore + joinGap,
            EloPower - eloPowerMaxGap, EloPower + eloPowerMaxGap);
        TypedLobby sqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, sqlLobby, sqlLobbyFilter);
        Debug.LogFormat("{0} : JoinRoom eloScore[{1}] eloScoreGap[{2}] eloScoreMaxGap[{3}] eloPower[{4}] eloPowerMaxGap[{5}] sqlLobby[{6}]", name, EloScore, eloScoreGap, eloScoreMaxGap, EloPower, eloPowerMaxGap, sqlLobbyFilter);
    }

    public void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        if (joinGap < eloScoreMaxGap)
        {
            joinGap += eloScoreGap;
            JoinRoom();
        }
        else CreateRoom();
    }

    private void CreateRoom()
    {
        var roomProperies = new ExitGames.Client.Photon.Hashtable();
        roomProperies.Add("t", PhotonNetwork.time);
        roomProperies.Add("m", MapId);
        roomProperies.Add("C0", VersionedName);
        roomProperies.Add("C1", EloScore);
        roomProperies.Add("C2", EloPower);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MaxPlayerCount;
        roomOptions.PlayerTtl = 1;
        roomOptions.CustomRoomProperties = roomProperies;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0", "C1", "C2" };

        TypedLobby sqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
        PhotonNetwork.CreateRoom(null, roomOptions, sqlLobby);
    }

    public void OnJoinedRoom()
    {
        if (callbackConnect != null)
        {
            roomInitTime = (double)PhotonNetwork.room.CustomProperties["t"];
            MapId = (int)PhotonNetwork.room.CustomProperties["m"];
            callbackConnect();
            callbackConnect = null;
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.room.PlayerCount == MaxPlayerCount)
        {
            PhotonNetwork.room.IsVisible = false;
            PhotonNetwork.room.IsOpen = false;
        }
    }
    #endregion



    #region Network Ending
    public void OnDisconnectedFromPhoton()
    {
        if (callbackDisconnect != null)
        {
            callbackDisconnect();
            callbackDisconnect = null;
        }
        else if (callbackError != null) callbackError(Error.Disconnected);
    }
    #endregion



    #region Network Errors
    public void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        if (callbackError != null) callbackError(Error.FailedToConnectToPhoton);
    }

    public void OnConnectionFail(DisconnectCause cause)
    {
        if (callbackError != null) callbackError(Error.ConnectionFail);
    }

    public void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        if (callbackError != null) callbackError(Error.PhotonCreateRoomFailed);
    }

    public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        if (callbackError != null) callbackError(Error.PhotonJoinRoomFailed);
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        if (callbackError != null) callbackError(Error.CustomAuthenticationFailed);
    }
    #endregion



    #region Network Events
    private void OnReceivedEvent(byte eventCode, object content, int senderId)
    {
        if (callbackStart != null && eventCode == (int)Events.Start)
            InitPlayTime((double)content);
        else if (OnEventCall != null)
            OnEventCall((Events)eventCode, content, senderId);
    }
    #endregion




    ///////////////////////////////////////////////////////////////////////////////////
    //  STATIC MEMBERS
    ///////////////////////////////////////////////////////////////////////////////////
    private static int eloPowerMaxGap = 0;
    private static int eloScoreGap = 0;
    private static int eloScoreMaxGap = 0;
    private static int joinGap = 0;
    private static double roomInitTime = 0;
    private static double playInitTime = 0;
    private static string VersionedName { get { return GlobalConfig.Photon.name + GlobalConfig.Photon.version; } }

    public static event EventCallback OnEventCall;
    private static System.Action callbackConnect = null;
    private static System.Action callbackDisconnect = null;
    private static System.Action<double> callbackStart = null;
    private static System.Action<Error> callbackError = null;

    public static byte MaxPlayerCount { get; set; }
    public static int MapId { get; set; }
    public static int EloScore { get; set; }
    public static int EloPower { get; set; }
    public static bool IsOffline { get; set; }
    public static bool IsJoined { get { return PhotonNetwork.inRoom; } }
    public static bool IsMaster { get { return PhotonNetwork.isMasterClient; } }
    public static int PlayerId { get { return PhotonNetwork.player.ID; } }
    public static int PlayersCount { get { return PhotonNetwork.room == null ? 0 : PhotonNetwork.room.PlayerCount; } }

    public static long RoomSeed
    {
        get { return (long)roomInitTime; }
    }

    public static float RoomTime
    {
        get
        {
            var diff = PhotonNetwork.time - roomInitTime;
            if (diff < 0) roomInitTime += double.MaxValue;
            return (float)diff;
        }
    }

    public static float PlayTime
    {
        get
        {
            var diff = PhotonNetwork.time - playInitTime;
            if (diff < -999) playInitTime += double.MaxValue;
            return (float)diff;
        }
    }

#if OFF
    public static void Setup(bool offline, byte maxPlayerCount, int mapId, int eloScore, int eloScoreMaxGap, int eloPower, int eloPowerMaxGap)
    {
        IsOffline = offline;
        MaxPlayerCount = maxPlayerCount;
        MapId = mapId;
        EloScore = eloScore;
        EloScoreMaxGap = eloScoreMaxGap;
        EloPower = eloPower;
        EloPowerMaxGap = eloPowerMaxGap;
        scoreGap = 
    }
#endif

    public static void Connect(System.Action onConnected, System.Action<double> onStart, System.Action<Error> onError)
    {
        callbackConnect = onConnected;
        callbackStart = onStart;
        callbackError = onError;

        PhotonNetwork.AuthValues = new AuthenticationValues(Profile.UserId);
        if (IsOffline)
            PhotonNetwork.offlineMode = true;
        else
            PhotonNetwork.ConnectUsingSettings(GlobalConfig.Photon.version);
    }

    public static void Disconnect(System.Action onDisconnected)
    {
        callbackError = null;
        if (PhotonNetwork.connected)
        {
            callbackDisconnect = onDisconnected;
            PhotonNetwork.Disconnect();
        }
        else onDisconnected();
    }

    public static void Start(float nextTime)
    {
        if (PhotonNetwork.isMasterClient)
        {
            double timeValue = PhotonNetwork.time + nextTime;

            if (IsOffline)
            {
                InitPlayTime(timeValue);
            }
            else
            {
                PhotonNetwork.room.IsVisible = false;
                PhotonNetwork.room.IsOpen = false;
                SendEvent(Events.Start, timeValue, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });
            }
        }
    }

    public static void SendEvent(Events code, object content, bool reliable, RaiseEventOptions options = null)
    {
        PhotonNetwork.RaiseEvent((byte)code, content, reliable, options);
    }

    private static void InitPlayTime(double value)
    {
        playInitTime = value;
        if (callbackStart != null)
        {
            callbackStart(playInitTime);
            callbackStart = null;
        }
    }
}
