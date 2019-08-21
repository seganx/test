﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class State_Playing : GameState
{
    [SerializeField] private Text timeLabel = null;
    [SerializeField] private AudioSource[] timerAudios = null;

    private bool allowUserHandle = true;
    private int timerAudioPlayed = -1;
    private bool isGamePaused = false;
    private int cameraMode = 0;

    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);
        RacerCamera.offset.z = -10;
        gameManager.OpenPopup<Popup_PlayingCountDown>();

        var waitTimer = new WaitForSeconds(0.2f);

        yield return waitTimer;
        foreach (var player in PlayerPresenter.all)
            player.BroadcastMessage("EnableRacerAudio", SendMessageOptions.DontRequireReceiver);

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

        float gametime = PlayNetwork.PlayTime / GlobalConfig.Race.maxTime;
        //PlayerPresenter.UpdateAll(Mathf.Clamp01(1 - Mathf.Pow(gametime - 1, 2)), deltaTime);
        PlayerPresenter.UpdateAll(gametime, deltaTime);

        RaceModel.stats.playerSpeed = PlayerPresenter.local.player.CurrSpeed;
        RaceModel.stats.playerPosition = PlayerPresenter.local.player.CurrPosition;

        RacerCamera.offset.z = Mathf.Lerp(RacerCamera.offset.z, -cameraMode * 0.6f, deltaTime * 3);
        RacerCamera.UpdateAll(deltaTime);

        var remainedTime = Mathf.Max(0, RaceModel.specs.maxPlayTime - PlayNetwork.PlayTime);
        timeLabel.text = Utilities.TimeToString(remainedTime, 3);

        if (remainedTime < 11 && timerAudioPlayed != Mathf.FloorToInt(remainedTime))
        {
            timerAudioPlayed = Mathf.FloorToInt(remainedTime);
            if (timerAudioPlayed > 0) timerAudios[0].Play();
            else timerAudios[1].Play();
        }

        if (allowUserHandle)
        {
            HandleUserActions();

            if (PlayNetwork.PlayTime > RaceModel.specs.maxPlayTime)
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
            PlayerPresenter.local.SteeringValue = InputManager.Left.isPointerDown ? -1 : (InputManager.Right.isPointerDown ? 1 : 0);
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
