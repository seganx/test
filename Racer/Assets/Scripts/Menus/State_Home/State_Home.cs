using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Home : GameState
{
    [SerializeField] private Button garageButton = null;
    [SerializeField] private Button upgradeButton = null;
    [SerializeField] private Button customButton = null;
    [SerializeField] private Button blackMarketButton = null;
    [SerializeField] private Button shopButton = null;
    [SerializeField] private Button loadingBoxButton = null;
    [SerializeField] private Button offlineButton = null;
    [SerializeField] private Button onlineButton = null;
    [SerializeField] private Button storyButton = null;
    [SerializeField] private Button gameTutorialButton = null;

    private void Start()
    {
        GarageCamera.SetCameraId(1);

#if UNITY_EDITOR
        //Profile.TotalRaces = 0;
#endif
        if (Profile.TotalRaces >= 0 && Profile.TotalRaces < 10)
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(Profile.TotalRaces));

        blackMarketButton.onClick.AddListener(() =>
        {
            OnOpened(blackMarketButton);
            gameManager.OpenState<State_BlackMarket>();
        });

        shopButton.onClick.AddListener(() =>
        {
            OnOpened(shopButton);
            gameManager.OpenState<State_Shop>();
        });

        loadingBoxButton.onClick.AddListener(() =>
        {
            OnOpened(loadingBoxButton);
            gameManager.OpenState<State_LoadingBox>();
        });

        garageButton.onClick.AddListener(() =>
        {
            OnOpened(garageButton);
            gameManager.OpenState<State_Garage>().Setup(0, rc => gameManager.OpenState<State_PhotoMode>());
        });

        upgradeButton.onClick.AddListener(() =>
        {
            OnOpened(upgradeButton);
            gameManager.OpenState<State_Garage>().Setup(0, rc => gameManager.OpenState<State_Upgrade>());
        });

        customButton.onClick.AddListener(() =>
        {
            OnOpened(customButton);
            gameManager.OpenState<State_Garage>().Setup(0, rc =>
            {
                if (Profile.IsUnlockedRacer(rc.Id))
                    gameManager.OpenState<State_Custome>();
                else
                    gameManager.OpenState<State_PhotoMode>();
            });
        });

        offlineButton.onClick.AddListener(() =>
        {
            OnOpened(offlineButton);
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "offline" } });
#endif
            gameManager.OpenState<State_Garage>().Setup(0, rc =>
            {
                if (Profile.IsUnlockedRacer(rc.Id))
                    StartOffline();
                else
                    gameManager.OpenPopup<Popup_RacerCardInfo>();
            });
        });

        onlineButton.onClick.AddListener(() =>
        {
            OnOpened(onlineButton);
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "online" } });
#endif
            gameManager.OpenState<State_LeagueStart>();
        });

        storyButton.onClick.AddListener(() =>
        {
            OnOpened(storyButton);
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "story" } });
#endif
            gameManager.OpenPopup<Popup_Confirm>().Setup(111103, true, null);

        });

        gameTutorialButton.onClick.AddListener(() =>
        {
            OnOpened(gameTutorialButton);
            gameManager.OpenState<State_GameTutorial>();
        });

        CheckPreset(() =>
        {
            UiHeader.Show();
            UiShowHide.ShowAll(transform);
            GarageRacer.LoadRacer(0);

            Popup_RateUs.CheckAndDisplay();
        });
    }

    public override void Back()
    {
        gameManager.OpenPopup<Popup_Confirm>().Setup(111063, true, yes =>
        {
            if (yes) Application.Quit();
        });
    }

    private void StartOffline()
    {
        RaceModel.Reset(RaceModel.Mode.FreeDrive);

        RaceModel.specs.mapId = RaceModel.SelectRandomMap();
        RaceModel.specs.maxPlayerCount = 4;
        RaceModel.specs.maxPlayTime = GlobalConfig.Race.maxTime;

        RaceModel.traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance;
        RaceModel.traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;

        PlayNetwork.IsOffline = true;
        PlayNetwork.EloScore = Profile.EloScore;
        PlayNetwork.EloPower = Profile.CurrentRacerPower;
        PlayNetwork.MapId = RaceModel.specs.mapId;
        PlayNetwork.MaxPlayerCount = RaceModel.specs.maxPlayerCount;

        gameManager.OpenState<State_GoToRace>();
    }

    private void CheckPreset(System.Action nextTask)
    {
        if (Profile.SelectedRacer > 0)
            nextTask();
        else
            gameManager.OpenPopup<Popup_Welcome>().SetCallback(nextTask);
    }

    void OnOpened(Button buttonGameObject)
    {
        UiHomeLockItem uiHomeLockItem = buttonGameObject.GetComponent<UiHomeLockItem>();
        if (uiHomeLockItem)
            uiHomeLockItem.SetOpenedOnce();
    }
}
