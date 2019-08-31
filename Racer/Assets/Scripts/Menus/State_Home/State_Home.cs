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
    [SerializeField] private RectTransform mapTransform = null;


    public Vector2 MapPosition
    {
        get
        {
            var res = Vector2.zero;
            res.x = PlayerPrefs.GetFloat(name + ".MapPosition.x", 0);
            res.y = PlayerPrefs.GetFloat(name + ".MapPosition.y", 0);
            return res;
        }

        set
        {
            PlayerPrefs.SetFloat(name + ".MapPosition.x", value.x);
            PlayerPrefs.SetFloat(name + ".MapPosition.y", value.y);
        }
    }

    private void Start()
    {
        UiHeader.Show();
        GarageCamera.SetCameraId(1);

        mapTransform.anchoredPosition = MapPosition;

        if (Profile.TotalRaces >= 0 && Profile.TotalRaces < 11)
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(Profile.TotalRaces, true, () => SetFocused(Profile.TotalRaces)));

        blackMarketButton.onClick.AddListener(() => gameManager.OpenState<State_BlackMarket>());

        shopButton.onClick.AddListener(() => gameManager.OpenState<State_Shop>());

        loadingBoxButton.onClick.AddListener(() => gameManager.OpenState<State_LoadingBox>());

        garageButton.onClick.AddListener(() =>
        {
            gameManager.OpenState<State_Garage>().Setup(0, rc => gameManager.OpenState<State_PhotoMode>());
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(61));
        });

        upgradeButton.onClick.AddListener(() =>
        {
            gameManager.OpenState<State_Garage>().Setup(0, rc => gameManager.OpenState<State_Upgrade>());
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(41, true, () => Profile.TotalRaces++));
        });

        customButton.onClick.AddListener(() =>
        {
            gameManager.OpenState<State_Garage>().Setup(0, rc =>
            {
                if (Profile.IsUnlockedRacer(rc.Id))
                    gameManager.OpenState<State_Custome>();
                else
                    gameManager.OpenState<State_PhotoMode>();
            });
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(91));
        });

        offlineButton.onClick.AddListener(() =>
        {
            gameManager.OpenState<State_Garage>().Setup(0, rc =>
            {
                if (Profile.IsUnlockedRacer(rc.Id))
                    StartOffline();
                else
                    gameManager.OpenPopup<Popup_RacerCardInfo>();
            });
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(11));
        });

        onlineButton.onClick.AddListener(() =>
        {
            gameManager.OpenState<State_LeagueStart>();
        });

        storyButton.onClick.AddListener(() =>
        {
            gameManager.OpenPopup<Popup_Confirm>().Setup(111103, true, null);

        });

        gameTutorialButton.onClick.AddListener(() => { gameManager.OpenState<State_GameTutorial>(); });

        CheckPreset(() =>
        {
            UiHeader.Show();
            UiShowHide.ShowAll(transform);
            GarageRacer.LoadRacer(0);

            Popup_RateUs.CheckAndDisplay();
        });
    }

    public override float PreClose()
    {
        MapPosition = mapTransform.anchoredPosition;
        return base.PreClose();
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
        if (Profile.IsFirstSession || Profile.SelectedRacer > 0)
            nextTask();
        else
            gameManager.OpenPopup<Popup_Welcome>().SetCallback(nextTask);
    }

    private void SetFocused(int tutorialId)
    {
        Vector2 destPosition = mapTransform.anchoredPosition;

        Button focusButton = onlineButton;
        switch (tutorialId)
        {
            case 0: focusButton = gameTutorialButton; break;
            case 1: case 2: focusButton = offlineButton; break;
            case 3: case 5: focusButton = onlineButton; break;
            case 4: focusButton = upgradeButton; break;
            case 6: focusButton = loadingBoxButton; break;
            case 7: focusButton = garageButton; break;
            case 8: focusButton = shopButton; break;
            case 9: focusButton = blackMarketButton; break;
            case 10: focusButton = customButton; break;
        }

        mapTransform.anchoredPosition = -focusButton.transform.AsRectTransform().anchoredPosition;
    }
}
