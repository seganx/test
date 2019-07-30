using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class State_Playing : GameState
{
    [SerializeField] private Text timeLabel = null;
    [SerializeField] private AudioSource[] timerAudios = null;

    private float forwardSpeedDelta = 0;
    private float maxForwardSpeed = 0;
    private float currForwardSpeed = 0;
    private float currForwardPosition = 100;
    private bool allowUserHandle = true;
    private int timerAudioPlayed = -1;
    private bool isGamePaused = false;
    private int cameraMode = 0;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
        gameManager.OpenPopup<Popup_PlayingCountDown>();
        maxForwardSpeed = PlayModel.maxForwardSpeed;
        forwardSpeedDelta = maxForwardSpeed - PlayModel.minForwardSpeed;
        DelayCall(0.1f, () =>
        {
            foreach (var player in PlayerPresenter.all)
                player.BroadcastMessage("EnableRacerAudio", SendMessageOptions.DontRequireReceiver);
        });
    }

    private void Update()
    {
        if (PlayerPresenter.local == null || PlayNetwork.IsJoined == false) return;

        //  compute racers speed
        {
            float x = PlayNetwork.PlayTime / GlobalConfig.Race.maxTime;
            float y = Mathf.Clamp01(1 - Mathf.Pow(x - 1, 4));
            currForwardSpeed = Mathf.Min(y * forwardSpeedDelta + PlayModel.minForwardSpeed, maxForwardSpeed);
            currForwardPosition += currForwardSpeed * Time.deltaTime;
            RacerCollisionContact.currSpeed = currForwardSpeed * 0.5f;
            RoadSpeedPresenter.CurrentSpeed = currForwardSpeed;
        }

        foreach (var player in PlayerPresenter.all)
            player.ForwardValue = currForwardPosition;

        RacerCamera.offset.z = Mathf.Lerp(RacerCamera.offset.z, -cameraMode * 0.5f, Time.deltaTime);

        var remainedTime = Mathf.Max(0, PlayModel.maxGameTime - PlayNetwork.PlayTime);
        timeLabel.text = Utilities.TimeToString(remainedTime, 3);

        if (remainedTime < 11 && timerAudioPlayed != Mathf.FloorToInt(remainedTime))
        {
            timerAudioPlayed = Mathf.FloorToInt(remainedTime);
            if (timerAudioPlayed > 0) timerAudios[0].Play();
            else timerAudios[1].Play();
        }

        if (allowUserHandle)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            int leftRight = Input.GetKey(KeyCode.LeftArrow) ? -1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
            if (leftRight != 0)
                PlayerPresenter.local.SteeringValue = leftRight;
            else
#endif
                PlayerPresenter.local.SteeringValue = InputManager.Left.isPointerDown ? -1 : (InputManager.Right.isPointerDown ? 1 : 0);

#if UNITY_EDITOR || UNITY_STANDALONE
            if (PlayerPresenter.local.NitrosReady && Input.GetKeyDown(KeyCode.LeftShift))
            {
                PlayerPresenter.local.UseNitrous();
                UiPlayingNitros.lastNitros = 0;
            }
            else
#endif
            if (PlayerPresenter.local.NitrosReady && InputManager.Boost.isPointerDown)
            {
                PlayerPresenter.local.UseNitrous();
                UiPlayingNitros.lastNitros = 0;
            }

            PlayerPresenter.local.Horn(InputManager.Horn.isPointerDown);

            if (PlayNetwork.PlayTime > PlayModel.maxGameTime)
            {
                UiShowHide.HideAll(transform);
                allowUserHandle = false;
                DisplayFinalCamera();
                PlayerPresenter.local.gameObject.AddComponent<BotPresenter>();
                gameManager.OpenPopup<Popup_RaceResult>().Setup(ExitToMainMenu);
            }
        }
    }

    public void OnCameraButton()
    {
        RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower;
        cameraMode = (cameraMode + 1) % 3;
#if OFF
        switch (RacerCameraConfig.Instance.currentMode)
        {
            case RacerCamera.Mode.StickingFollower: RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.QuadCopter; break;
            case RacerCamera.Mode.QuadCopter: RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.Cinematic; break;
            case RacerCamera.Mode.Cinematic: RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower; break;
                //case RacerCamera.Mode.Driver: RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.Front; break;
                //case RacerCamera.Mode.Front: RacerCameraConfig.Instance.currentMode = RacerCamera.Mode.StickingFollower; break;
        }
#endif
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
            isGamePaused = PlayNetwork.PlayTime < PlayModel.maxGameTime;
        else if (isGamePaused && PlayNetwork.PlayTime > PlayModel.maxGameTime)
            ExitToMainMenu();
    }
}
