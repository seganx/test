using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_FindOpponents : GameState
{
    private enum State { Waiting, StartCounting, Counting, Started }

    [SerializeField] private LocalText countDownText = null;
    [SerializeField] private LocalText pingLabel = null;
    [SerializeField] private LocalText tipsLabel = null;
    [SerializeField] private GameObject searchbar = null;
    [SerializeField] private GameObject playersInfoBar = null;
    [SerializeField] private UiFindOpponentsPlayerInfo playerInfo = null;
    [SerializeField] private UiFindOpponentsPlayerInfo opponentInfo = null;


    private PlayerData playerData = null;
    private int grade = 0;
    private float waitTime = 0;
    private State state = State.Waiting;
    private int lastPlayersCount = 0;
    private bool waitFirst = true;

    private IEnumerator Start()
    {
        UiHeader.Hide();

        PlayNetwork.Connect(() =>
        {
            var seed = (int)(PlayNetwork.RoomSeed % 4) + PlayNetwork.PlayersCount;
            grade = 10 - seed % 4;
        },
        StartGame,
        OnNetworkError);

        searchbar.SetActive(PlayModel.IsOnline);
        playersInfoBar.SetActive(false);

        playerData = new PlayerData(Profile.Name, Profile.Score, Profile.Position, Profile.CurrentRacer);
        playerInfo.Setup(playerData);

        // entring loop
        int tipsCounter = 1;
        while (true)
        {
            int ping = PhotonNetwork.GetPing();
            countDownText.SetText(Mathf.Max(0, Mathf.RoundToInt(GlobalConfig.MatchMaking.joinTimeout - waitTime)).ToString());
            pingLabel.SetFormatedText(ping);
            pingLabel.target.color = ping < 100 ? Color.green : (ping < 300 ? Color.yellow : Color.red);
            tipsLabel.SetFormatedText(LocalizationService.Get(111020 + (TipsNumber % 9)));
            if (tipsCounter++ % 15 == 0) TipsNumber++;
            yield return new WaitForSeconds(1);
        }
    }

    private void LateUpdate()
    {
        switch (state)
        {
            case State.Waiting:
                {
                    waitTime += Time.deltaTime;
                    if (PlayModel.IsOnline == false || waitTime > GlobalConfig.MatchMaking.joinTimeout || PlayNetwork.PlayersCount == PlayModel.maxPlayerCount)
                    {
                        if (waitFirst && WaitMore && PlayNetwork.PlayersCount < 2)
                        {
                            countDownText.target.color = Color.yellow;
                            waitFirst = false;
                            waitTime = 0;
                        }
                        else if (PlayNetwork.IsMaster)
                        {
                            state = State.StartCounting;
                            PlayNetwork.Start(10);
                        }
                    }

                    // check to wait more on joining new player
                    if (lastPlayersCount != PlayNetwork.PlayersCount)
                    {
                        if (waitFirst && WaitMore && PlayNetwork.PlayersCount == 2)
                        {
                            countDownText.target.color = Color.yellow;
                            waitFirst = false;
                            waitTime = 0;
                        }
                        lastPlayersCount = PlayNetwork.PlayersCount;
                    }
                }
                break;

            case State.StartCounting:
                break;

            case State.Counting:
                {
                    if (PlayNetwork.PlayTime >= 0)
                        PlayGame();
                }
                break;
        }
    }

    private void StartGame(double startTime)
    {
        StableRandom.Initialize(Mathf.RoundToInt((float)startTime));
        Game.LoadMap(PlayNetwork.MapId);

        PlayerPresenterOnline.CreateOnline(playerData, grade, false);

        var botcount = PlayModel.maxPlayerCount - PlayNetwork.PlayersCount;
        BotPresenter.InitializeBots(botcount);

        RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower;
        state = State.Counting;

        if (PlayModel.IsOnline)
        {
            Profile.Score -= 1;
            Network.SendScore(Profile.Score);
            FuelTimerPresenter.ReduceFuel();

#if DATABEEN
            DataBeen.SendCustomEventData("game", new DataBeenConnection.CustomEventInfo[] {
                new DataBeenConnection.CustomEventInfo() { key = "started", value = "true" },
                new DataBeenConnection.CustomEventInfo() { key = "bots", value = botcount.ToString() },
            });
#endif
        }

        StartCoroutine(DisplayPlayersInfo());
    }

    private IEnumerator DisplayPlayersInfo()
    {
        yield return new WaitForSeconds(1);

        searchbar.SetActive(false);
        playersInfoBar.SetActive(true);

        if (PlayerPresenter.allPlayers.Count < PlayModel.maxPlayerCount)
            yield return new WaitForSeconds(1);
        if (PlayerPresenter.allPlayers.Count < PlayModel.maxPlayerCount)
            yield return new WaitForSeconds(1);

        opponentInfo.gameObject.SetActive(false);
        foreach (var player in PlayerPresenter.allPlayers)
        {
            if (player.IsPlayer) continue;
            opponentInfo.Clone<UiFindOpponentsPlayerInfo>().Setup(player).gameObject.SetActive(true);
        }
        Destroy(opponentInfo.gameObject);
    }

    private void PlayGame()
    {
        state = State.Started;
        PlayModel.minForwardSpeed = GlobalConfig.Race.startSpeed;
        PlayModel.maxForwardSpeed = PlayerPresenterOnline.FindMaxGameSpeed();
        gameManager.OpenState<State_Playing>(true);
    }

    private void ExitToMainMenu()
    {
        Game.LoadMap(0);
        Game.Instance.ClosePopup(true);
        Game.Instance.OpenState<State_Home>(true);
    }

    public override void Back()
    {
        //PlayNetwork.Stop(ExitToMainMenu);
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static bool WaitMore
    {
        get { return PlayerPrefs.GetInt("State_FindOpponents.WaitMore", 0) > 0; }
        set { PlayerPrefs.SetInt("State_FindOpponents.WaitMore", value ? 1 : 0); }
    }

    private static int TipsNumber
    {
        get { return PlayerPrefs.GetInt("State_FindOpponents.TipsNumber", 0); }
        set { PlayerPrefs.SetInt("State_FindOpponents.TipsNumber", value); }
    }

    private static void OnNetworkError(PlayNetwork.Error error)
    {
        switch (error)
        {
            case PlayNetwork.Error.FailedToConnectToPhoton:
            case PlayNetwork.Error.Disconnected:
            case PlayNetwork.Error.ConnectionFail:
            case PlayNetwork.Error.PhotonCreateRoomFailed:
            case PlayNetwork.Error.CustomAuthenticationFailed:
                if (gameManager.CurrentState.enabled)
                {
                    gameManager.CurrentState.enabled = false;
                    Game.Instance.OpenPopup<Popup_Confirm>().Setup(LocalizationService.Get(111102) + error, false, isok => PlayNetwork.Disconnect(() =>
                    {
                        gameManager.CurrentState.SendMessage("ExitToMainMenu", SendMessageOptions.DontRequireReceiver);
                    }));
                }

#if DATABEEN
                DataBeen.SendCustomEventData("game", new DataBeenConnection.CustomEventInfo[] {
                    new DataBeenConnection.CustomEventInfo() { key = "started", value = "false" },
                    new DataBeenConnection.CustomEventInfo() { key = "error", value = error.ToString() },
                });
#endif
                break;

            case PlayNetwork.Error.PhotonJoinRoomFailed:
                break;
        }
    }
}
