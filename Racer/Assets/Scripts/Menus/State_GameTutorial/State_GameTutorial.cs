using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_GameTutorial : GameState
{
    [SerializeField] private Button tutorialRaceButton;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public void OnStartTutorial()
    {
        RaceModel.Reset(RaceModel.Mode.Tutorial);

        RaceModel.specs.mapId = 2;
        RaceModel.specs.maxPlayerCount = 2;
        RaceModel.specs.maxPlayTime = 90;
#if UNITY_EDITOR
        RaceModel.specs.maxPlayTime = 10;
#endif

        RaceModel.traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance * 1.5f;
        RaceModel.traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;

        PlayNetwork.IsOffline = true;
        PlayNetwork.MapId = RaceModel.specs.mapId;
        PlayNetwork.MaxPlayerCount = RaceModel.specs.maxPlayerCount;

        var racerdata = new RacerProfile();

        racerdata.id = 370;
        racerdata.cards = 1000;
        racerdata.unlock = 1;
        racerdata.level.Level = 1;
        racerdata.level.BodyLevel = 5;
        racerdata.level.NitroLevel = 5;
        racerdata.level.SpeedLevel = 5;
        racerdata.level.SteeringLevel = 5;

        racerdata.custom.BodyColor = 10;
        racerdata.custom.Vinyl = 460;
        racerdata.custom.VinylColor = 580;
        racerdata.custom.Wheel = 70;
        racerdata.custom.Spoiler = 20;
        racerdata.custom.WindowColor = 60;
        racerdata.custom.LightsColor = 60;

        var playerdata = new PlayerData(Profile.Name, Profile.Score, Profile.Position, racerdata);
        gameManager.OpenState<State_GoToRace>().Setup(playerdata);
    }
}
