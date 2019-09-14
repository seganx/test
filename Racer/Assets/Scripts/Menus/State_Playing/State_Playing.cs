using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class State_Playing : GameState
{
    [SerializeField] private Text timeLabel = null;
    [SerializeField] private Animation timeBeat = null;
    [SerializeField] private AudioSource[] timerAudios = null;

    private float forwardSpeedDelta = 0;
    private bool allowUserHandle = true;
    private int timerAudioPlayed = -1;
    private bool isGamePaused = false;
    private int cameraMode = 0;

    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);
        RacerCamera.offset.z = -10;
        gameManager.OpenPopup<Popup_PlayingCountDown>();


        RaceModel.specs.minForwardSpeed = GlobalConfig.Race.startSpeed;
        RaceModel.specs.maxForwardSpeed = PlayerPresenter.FindMaxGameSpeed();
        forwardSpeedDelta = RaceModel.specs.maxForwardSpeed - RaceModel.specs.minForwardSpeed;

        var waitTimer = new WaitForSeconds(0.2f);
        yield return waitTimer;
        foreach (var player in PlayerPresenter.all)
        {
            player.BroadcastMessage("EnableRacerAudio", SendMessageOptions.DontRequireReceiver);
            player.racer.SetTransparent(player.player.IsPlayer == false);
        }

        PlayerPresenter.UpdateRanks();
        for (int i = 0; i < PlayerPresenter.all.Count; i++)
            PlayerPresenter.all[i].player.CurrNitrous = i * GlobalConfig.Race.nosStartFactor;

        while (true)
        {
            if (allowUserHandle)
            {
                PlayerPresenter.UpdateRanks();
                UiPlayingBoard.UpdatePositions();
                RaceModel.stats.playerRank = PlayerPresenter.local.player.CurrRank;
            }
            yield return waitTimer;
        }
    }

    private void Update()
    {
        if (PlayerPresenter.local == null || PlayNetwork.IsJoined == false) return;
        float deltaTime = Time.deltaTime;

        //  compute racers speed
        RaceModel.stats.playTime = PlayNetwork.PlayTime;
        float gametime = RaceModel.stats.playTime / RaceModel.specs.maxPlayTime;
        float speedtime = Mathf.Clamp01(1 - Mathf.Pow(Mathf.Abs(gametime - 1), 1.5f));
        RaceModel.stats.globalSpeed = Mathf.Min(speedtime * forwardSpeedDelta + RaceModel.specs.minForwardSpeed, RaceModel.specs.maxForwardSpeed);

        PlayerPresenter.UpdateAll(speedtime, deltaTime);

        RaceModel.stats.playerSpeed = PlayerPresenter.local.player.CurrSpeed;
        RaceModel.stats.playerPosition = PlayerPresenter.local.player.CurrPosition;

        RacerCamera.offset.z = Mathf.Lerp(RacerCamera.offset.z, -cameraMode * 0.6f, deltaTime * 3);
        RacerCamera.UpdateAll(deltaTime);

        var remainedTime = Mathf.Max(0, RaceModel.specs.maxPlayTime - PlayNetwork.PlayTime);
        timeLabel.text = Utilities.TimeToString(remainedTime, 1);

        if (remainedTime < 11 && timerAudioPlayed != Mathf.FloorToInt(remainedTime))
        {
            timerAudioPlayed = Mathf.FloorToInt(remainedTime);
            if (timerAudioPlayed > 0) timerAudios[0].Play();
            else timerAudios[1].Play();
            timeBeat.Play();
        }

        if (allowUserHandle)
        {
            HandleUserActions();

            if (RaceModel.stats.playTime > RaceModel.specs.maxPlayTime)
            {
                allowUserHandle = false;
                FinishRace();
            }
        }
    }

    private void HandleUserActions()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        int leftRight = Input.GetKey(KeyCode.LeftArrow) ? -1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
        if (leftRight != 0)
            PlayerPresenter.local.SteeringValue = leftRight;
        else
#endif
            PlayerPresenter.local.SteeringValue = UiPlayingGesture.GoToLeft ? -1 : (UiPlayingGesture.GoToRight ? 1 : 0);
            //PlayerPresenter.local.SteeringValue = InputManager.Left.isPointerDown ? -1 : (InputManager.Right.isPointerDown ? 1 : 0);
            RacerCamera.steeringValue = PlayerPresenter.local.SteeringValue;
        PlayerPresenter.local.Horn(InputManager.Horn.isPointerDown);
    }

    private void FinishRace()
    {
        UiShowHide.HideAll(transform);
        DisplayFinalCamera();
        PlayerPresenter.local.gameObject.AddComponent<BotPresenter>();
        gameManager.OpenPopup<Popup_RaceResult>().Setup(ExitToMainMenu);
    }

    public void OnCameraButton()
    {
        RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower;
        cameraMode = (cameraMode + 1) % 3;
    }

    private void DisplayFinalCamera()
    {
        RacerCameraConfig.Instance.currentMode = (RacerCamera.Mode)Random.Range(1, 8);
        DelayCall(3, () => DisplayFinalCamera());
    }

    public override void Back()
    {
#if UNITY_EDITOR
        ExitToMainMenu();
#endif
    }

    public void ExitToMainMenu()
    {
        enabled = false;
        PlayNetwork.Disconnect(() =>
        {
            Game.LoadMap(0);
            Game.Instance.ClosePopup(true);
            gameManager.OpenState<State_Home>(true);
        });
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            isGamePaused = PlayNetwork.PlayTime < RaceModel.specs.maxPlayTime;
        else if (isGamePaused && PlayNetwork.PlayTime > RaceModel.specs.maxPlayTime)
            ExitToMainMenu();
    }
}
