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
    [SerializeField] private Button gameToturialButton = null;

    private void Start()
    {
        GarageCamera.SetCameraId(1);

        blackMarketButton.onClick.AddListener(() => gameManager.OpenState<State_BlackMarket>());
        shopButton.onClick.AddListener(() => gameManager.OpenState<State_Shop>());
        loadingBoxButton.onClick.AddListener(() => gameManager.OpenState<State_LoadingBox>());

        garageButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(0, () => gameManager.OpenState<State_PhotoMode>()));
        upgradeButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(0, () => gameManager.OpenState<State_Upgrade>()));

        customButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(0, () =>
        {
            if (Profile.IsUnlockedRacer(GarageRacer.racer.Id))
                gameManager.OpenState<State_Custome>();
            else
                gameManager.OpenState<State_PhotoMode>();
        }));

        offlineButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "offline" } });
#endif
            gameManager.OpenState<State_Garage>().Setup(0, () =>
            {
                if (Profile.IsUnlockedRacer(GarageRacer.racer.Id))
                    StartOffline();
                else
                    gameManager.OpenPopup<Popup_RacerCardInfo>();
            });
        });

        onlineButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "online" } });
#endif
            gameManager.OpenState<State_LeagueStart>();
        });

        storyButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "story" } });
#endif
            gameManager.OpenPopup<Popup_Confirm>().Setup(111103, true, null);

        });

        gameToturialButton.onClick.AddListener(() => gameManager.OpenState<State_GameToturial>());

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
        PlayModel.mode = PlayModel.Mode.Campain;

        PlayNetwork.IsOffline = true;
        PlayNetwork.EloScore = Profile.EloScore;
        PlayNetwork.EloPower = Profile.CurrentRacerPower;
        PlayNetwork.MapId = PlayModel.mapId = PlayModel.SelectRandomMap();
        PlayNetwork.MaxPlayerCount = PlayModel.maxPlayerCount = 4;
        PlayModel.maxPlayTime = GlobalConfig.Race.maxTime;
        PlayModel.Traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance;
        PlayModel.Traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;

        gameManager.OpenState<State_FindOpponents>();
    }

    private void CheckPreset(System.Action nextTask)
    {
        if (Profile.SelectedRacer > 0)
            nextTask();
        else
            gameManager.OpenPopup<Popup_Welcome>().SetCallback(nextTask);
    }
}
