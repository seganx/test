using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_GameTutorial : GameState
{
    [SerializeField] private Button tutorialRaceButton;
    private int currentTutorialPageIndex = 0;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public void OnStartTutorial()
    {
        RaceModel.Reset(RaceModel.Mode.Tutorial);

        RaceModel.specs.mapId = 2;
        RaceModel.specs.maxPlayerCount = 2;
        RaceModel.specs.maxPlayTime = 120;
        RaceModel.specs.minForwardSpeed = GlobalConfig.Race.startSpeed;
        RaceModel.specs.maxForwardSpeed = 50;

        RaceModel.traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance * 1.5f;
        RaceModel.traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;

        PlayNetwork.IsOffline = true;
        PlayNetwork.EloScore = Profile.EloScore;
        PlayNetwork.EloPower = Profile.CurrentRacerPower;
        PlayNetwork.MapId = RaceModel.specs.mapId;
        PlayNetwork.MaxPlayerCount = RaceModel.specs.maxPlayerCount;

        gameManager.OpenState<State_GoToRace>();
    }
}
