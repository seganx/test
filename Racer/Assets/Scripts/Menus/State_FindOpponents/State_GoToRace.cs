using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_GoToRace : GameState
{
    private enum State { Waiting, StartCounting, Counting, Started }

    [SerializeField] private LocalText countDownText = null;
    [SerializeField] private LocalText pingLabel = null;
    [SerializeField] private LocalText tipsLabel = null;
    [SerializeField] private GameObject searchbar = null;
    [SerializeField] private GameObject playersInfoBar = null;
    [SerializeField] private UiGoToRacePlayerInfo playerInfo = null;
    [SerializeField] private UiGoToRacePlayerInfo opponentInfo = null;


    private PlayerData playerData = null;
    private float waitTime = 0;
    private State state = State.Waiting;
    private int lastPlayersCount = 0;
    private bool waitFirst = true;

    private string lastPingValueString = "lastPingValue";
    private int lastPingValue
    {
        set { PlayerPrefs.SetInt(lastPingValueString, value); }
        get { return PlayerPrefs.GetInt(lastPingValueString, 50); }
    }

    public State_GoToRace Setup(PlayerData playerdata)
    {
        playerData = playerdata;
        return this;
    }

    private IEnumerator Start()
    {
        GarageCamera.SetCameraId(1);
        UiHeader.Hide();

        PlayNetwork.Connect(() => { },
        StartGame,
        OnNetworkError);

        searchbar.SetActive(RaceModel.IsOnline);
        playersInfoBar.SetActive(false);
        opponentInfo.gameObject.SetActive(false);


        if (playerData == null)
            playerData = new PlayerData(Profile.Name, Profile.Score, Profile.Position, Profile.CurrentRacer);
        playerInfo.Setup(playerData);

        // entring loop
        int tipsCounter = 1;
        while (true)
        {
            if (PlayNetwork.IsOffline) // disconnect on last game
            {
                if (Random.value > .4f) lastPingValue += Random.Range(-2, 2);
                lastPingValue = Mathf.Clamp(lastPingValue, 30, 90);
            }
            else
                lastPingValue = PhotonNetwork.GetPing();


            countDownText.SetText(Mathf.Max(0, Mathf.RoundToInt(GlobalConfig.MatchMaking.joinTimeout - waitTime)).ToString());
            pingLabel.SetFormatedText(lastPingValue);
            pingLabel.target.color = lastPingValue < 100 ? Color.green : (lastPingValue < 300 ? Color.yellow : Color.red);
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
                    if (RaceModel.IsOnline == false || waitTime > GlobalConfig.MatchMaking.joinTimeout || PlayNetwork.PlayersCount == RaceModel.specs.maxPlayerCount)
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

        PlayerPresenterOnline.Create(playerData, false);

        RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower;
        state = State.Counting;

        if (RaceModel.IsOnline)
        {
            Profile.Score -= 1;
            Network.SendScore(Profile.Score);
            FuelTimerPresenter.ReduceFuel();
        }

        StartCoroutine(DisplayPlayersInfo());
    }

    private IEnumerator DisplayPlayersInfo()
    {
        yield return new WaitForSeconds(1);

        searchbar.SetActive(false);
        playersInfoBar.SetActive(true);

        if (PlayerPresenter.all.Count < PlayNetwork.PlayersCount)
            yield return new WaitForSeconds(1);
        if (PlayerPresenter.all.Count < PlayNetwork.PlayersCount)
            yield return new WaitForSeconds(1);

        if (PhotonNetwork.isMasterClient)
        {
            var presenter = PlayerPresenter.all.FindMin(x => x.player.RacerPower);
            BotPresenter.InitializeBots(RaceModel.specs.maxPlayerCount - PlayNetwork.PlayersCount, presenter.player.Score, presenter.player.RacerId, presenter.player.RacerPower);
        }

        foreach (var player in PlayerPresenter.all)
        {
            if (player.player.IsPlayer) continue;
            opponentInfo.Clone<UiGoToRacePlayerInfo>().Setup(player.player).gameObject.SetActive(true);
        }
        Destroy(opponentInfo.gameObject);

        if (PlayNetwork.IsMaster)
        {
            yield return new WaitForSeconds(1);
            PlayerPresenter.SetReadyToRace();
        }
    }

    private void PlayGame()
    {
        state = State.Started;
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
                break;

            case PlayNetwork.Error.PhotonJoinRoomFailed:
                break;
        }
    }
}
