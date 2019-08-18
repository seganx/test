using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_GameTutorial : GameState
{
    [SerializeField] private GameObject[] tutorialObjects;
    [SerializeField] private Button nextTutorialButton;
    [SerializeField] private Button prevTutorialButton;
    [SerializeField] private Button tutorialRaceButton;
    private int currentTutorialPageIndex = 0;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
        UpdateCurrenctTutorialPage();

        nextTutorialButton.onClick.AddListener(() =>
        {
            currentTutorialPageIndex++;
            UpdateCurrenctTutorialPage();
        });

        prevTutorialButton.onClick.AddListener(() =>
        {
            currentTutorialPageIndex--;
            UpdateCurrenctTutorialPage();
        });

        tutorialRaceButton.onClick.AddListener(() =>
        {
        });
    }

    private void UpdateCurrenctTutorialPage()
    {
        foreach (var item in tutorialObjects)
            item.SetActive(false);
        tutorialObjects[currentTutorialPageIndex].SetActive(true);

        prevTutorialButton.gameObject.SetActive(currentTutorialPageIndex > 0);
        nextTutorialButton.gameObject.SetActive(currentTutorialPageIndex < tutorialObjects.Length - 1);
    }

    public void OnStartTutorial()
    {
        RaceModel.Reset(RaceModel.Mode.Tutorial);

        RaceModel.specs.mapId = 2;
        RaceModel.specs.maxPlayerCount = 2;
        RaceModel.specs.maxPlayTime = 120;
        RaceModel.specs.minForwardSpeed = GlobalConfig.Race.startSpeed;
        RaceModel.specs.maxForwardSpeed = 120;

        RaceModel.traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance * 1.5f;
        RaceModel.traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;

        PlayNetwork.IsOffline = true;
        PlayNetwork.EloScore = Profile.EloScore;
        PlayNetwork.EloPower = Profile.CurrentRacerPower;
        PlayNetwork.MapId = RaceModel.specs.mapId;
        PlayNetwork.MaxPlayerCount = RaceModel.specs.maxPlayerCount;

        Game.LoadMap(PlayNetwork.MapId);

        var playerData = new PlayerData(Profile.Name, Profile.Score, Profile.Position, Profile.CurrentRacer);
        playerData.IsPlayer = true;

        PlayerPresenter.local = PlayerPresenterOffline.Create(playerData);

        gameManager.OpenState<State_Playing>(true);
    }
}
