using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_FindOpponents : GameState
{
    private enum State { Waiting, StartCounting, Counting, Started }

    [SerializeField] private Image racerIcon = null;
    [SerializeField] private Text racerName = null;
    [SerializeField] private LocalText racerPower = null;
    [SerializeField] private LocalText scoreValue = null;
    [SerializeField] private Image leagueImage = null;
    [SerializeField] private GameObject searchbar = null;
    [SerializeField] private LocalText countDownText = null;
    [SerializeField] private LocalText pingLabel = null;
    [SerializeField] private LocalText tipsLabel = null;

    private int grade = 0;
    private float waitTime = 0;
    private State state = State.Waiting;
    private int lastPlayersCount = 0;
    private bool waitFirst = true;

    public static bool WaitMore
    {
        get { return PlayerPrefs.GetInt("State_FindOpponents.WaitMore", 0) > 0; }
        set { PlayerPrefs.SetInt("State_FindOpponents.WaitMore", value ? 1 : 0); }
    }

    private int TipsNumber
    {
        get { return PlayerPrefs.GetInt("State_FindOpponents.TipsNumber", 0); }
        set { PlayerPrefs.SetInt("State_FindOpponents.TipsNumber", value); }
    }

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

        searchbar.SetActive(PlayModel.OfflineMode == false);

        var racerconfig = RacerFactory.Racer.GetConfig(Profile.SelectedRacer);
        racerIcon.sprite = racerconfig.icon;
        racerName.text = racerconfig.Name;
        racerPower.SetText(racerconfig.ComputePower(Profile.CurrentRacer.level.NitroLevel, Profile.CurrentRacer.level.SteeringLevel, Profile.CurrentRacer.level.BodyLevel).ToString("0"));

        scoreValue.SetText(Profile.Score.ToString());
        leagueImage.sprite = GlobalFactory.League.GetBigIcon(Profile.League);

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

    private void StartGame(double startTime)
    {
        StableRandom.Initialize(Mathf.RoundToInt((float)startTime));
        Game.LoadMap(PlayNetwork.RoomMapId);

        var nplayer = new PlayerData(Profile.Name, Profile.Score, Profile.CurrentRacer);
        PlayerPresenterOnline.CreateOnline(nplayer, grade, false);

        var botcount = PlayModel.maxPlayerCount - PlayNetwork.PlayersCount;
        BotPresenter.InitializeBots(botcount);

        RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower;
        state = State.Counting;

        if (PlayModel.OfflineMode == false)
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
    }


    private void LateUpdate()
    {
        switch (state)
        {
            case State.Waiting:
                {
                    waitTime += Time.deltaTime;
                    if (PlayModel.OfflineMode || waitTime > GlobalConfig.MatchMaking.joinTimeout || PlayNetwork.PlayersCount == PlayModel.maxPlayerCount)
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
                            PlayNetwork.Start(PlayModel.OfflineMode ? 10 : 3);
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

    private void PlayGame()
    {
        state = State.Started;
        PlayModel.maxGameTime = GlobalConfig.Race.maxTime;
        PlayModel.minForwardSpeed = GlobalConfig.Race.startSpeed;
        PlayModel.maxForwardSpeed = PlayerPresenterOnline.FindMaxGameSpeed();
        gameManager.OpenState<State_Playing>(true);
    }

    private void ExitToMainMenu()
    {
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
