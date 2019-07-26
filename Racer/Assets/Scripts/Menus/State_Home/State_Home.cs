﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Home : GameState
{
    [SerializeField] private Button garageButton = null;
    [SerializeField] private Button customButton = null;
    [SerializeField] private Button upgradeButton = null;
    [SerializeField] private Button blackMarketButton = null;
    [SerializeField] private Button shopButton = null;
    [SerializeField] private Button loadingBoxButton = null;
    [SerializeField] private Button offlineButton = null;
    [SerializeField] private Button onlineButton = null;

    private void Start()
    {
        GarageCamera.SetCameraId(1);

        blackMarketButton.onClick.AddListener(() => gameManager.OpenState<State_BlackMarket>());
        shopButton.onClick.AddListener(() => gameManager.OpenState<State_Shop>());
        loadingBoxButton.onClick.AddListener(() => gameManager.OpenState<State_LoadingBox>());

        garageButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(() => gameManager.OpenState<State_PhotoMode>()));
        customButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(() => gameManager.OpenState<State_Custome>()));
        upgradeButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(() => gameManager.OpenState<State_Upgrade>()));

        offlineButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "offline" } });
#endif
            gameManager.OpenState<State_Garage>().Setup(() => StartOffline());
        });

        onlineButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "online" } });
#endif
            gameManager.OpenState<State_LeagueStart>();
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
        PlayModel.OfflineMode = true;
        PlayModel.eloScore = 0;
        PlayModel.eloPower = 0;
        PlayModel.selectedMapId = PlayModel.SelectRandomMap();
        PlayModel.maxPlayerCount = 4;

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
