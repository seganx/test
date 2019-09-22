﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_LeagueStart : GameState
{
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private Button bigIcon = null;
    [SerializeField] private Button rewardButton = null;
    [SerializeField] private Button startButton = null;
    [SerializeField] private Button leaderboardButton = null;
    [SerializeField] private Button claimRewardsButton = null;

    private void Start()
    {
        GarageCamera.SetCameraId(1);
        UiHeader.Show();
        Popup_Loading.Display();
        ProfileLogic.SyncWidthServer(false, success =>
        {
            Popup_Loading.Hide();
            if (success)
            {
                if (GlobalConfig.ForceUpdate.league)
                {
                    gameManager.OpenPopup<Popup_Confirm>().Setup(111100, true, yes =>
                    {
                        if (yes)
                        {
                            Application.OpenURL(GlobalConfig.Socials.storeUrl);
                            Application.Quit();
                        }
                        else Back();
                    });
                }
                else DisplayItems();
            }
            else gameManager.OpenPopup<Popup_Confirm>().Setup(111061, false, ok => Back());
        });
    }

    public void DisplayItems()
    {
        scoreLabel.SetFormatedText(Profile.Score);
        rankLabel.SetText(Profile.PositionString);
        bigIcon.targetGraphic.As<Image>().sprite = GlobalFactory.League.GetBigIcon(Profile.League);
        claimRewardsButton.gameObject.SetActive(Profile.LeagueResultExist);

        claimRewardsButton.onClick.AddListener(ClaimRewards);
        bigIcon.onClick.AddListener(() => gameManager.OpenPopup<Popup_LeagueInfo>());
        rewardButton.onClick.AddListener(() =>
        {
            gameManager.OpenPopup<Popup_LeaguePrize>();
            //PopupQueue.Add(.5f, () => Popup_Tutorial.Display(34, false));
        });
        leaderboardButton.onClick.AddListener(() => gameManager.OpenState<State_Leaderboards>());

        var leagueInfo = GlobalConfig.Leagues.GetByIndex(Profile.League);
        startButton.onClick.AddListener(() =>
        {
            SetupGarage(leagueInfo);
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(32));
        });

        UiShowHide.ShowAll(transform);

        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(31));

        if (Profile.TotalRaces < 30 && FuelTimerPresenter.FuelCount <= 0)
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(39, true, () => FuelTimerPresenter.FullFuel()));
    }

    private void SetupGarage(GlobalConfig.Data.League leagueInfo)
    {
        gameManager.OpenState<State_Garage>().Setup(leagueInfo.startGroup, rconfig =>
        {
            if (Profile.IsUnlockedRacer(rconfig.Id))
            {
                if (rconfig.GroupId == leagueInfo.startGroup)
                {
                    ShopLogic.SpecialOffer.Refresh();

                    ShopLogic.SpecialOffer.Package leagueOfferPackage = null;
                    foreach (var item in ShopLogic.SpecialOffer.Packages)
                    {
                        RacerConfig packageRacerConfig = null;
                        packageRacerConfig = RacerFactory.Racer.GetConfig(item.racerId);
                        if (packageRacerConfig.GroupId == leagueInfo.startGroup)
                            leagueOfferPackage = item;
                    }
                    leagueOfferPackage = new ShopLogic.SpecialOffer.Package();
                    leagueOfferPackage.racerIndex = 130;
                    leagueOfferPackage.packgIndex = 2;
                    leagueOfferPackage.item = GlobalConfig.Shop.leagueSpecialPackages[leagueOfferPackage.packgIndex];


                    if (leagueOfferPackage != null)
                    {
                        gameManager.OpenPopup<Popup_LeagueRacerOffer>().Setup(leagueInfo.startGroup, resume =>
                        {
                            if (resume)
                                StartOnlineGame(leagueInfo.startGroup);
                            else
                                Game.Instance.OpenPopup<Popup_ShopSpecialPackage>().Setup(leagueOfferPackage, pack => { SetupGarage(leagueInfo); });
                        });
                    }
                    else
                    {
                        var str = string.Format(LocalizationService.Get(111141), leagueInfo.startGroup);
                        gameManager.OpenPopup<Popup_Confirm>().Setup(str, true, isok =>
                        {
                            if (isok) StartOnlineGame(leagueInfo.startGroup);
                        });
                    }
                }
                else StartOnlineGame(leagueInfo.startGroup);
            }
            else gameManager.OpenPopup<Popup_RacerCardInfo>();

        });
    }

    private void StartOnlineGame(int racegroup)
    {
        RaceModel.Reset(RaceModel.Mode.Online);

        RaceModel.specs.mapId = RaceModel.SelectRandomMap();
        RaceModel.specs.racersGroup = racegroup;
        RaceModel.specs.maxPlayerCount = 4;
        RaceModel.specs.maxPlayTime = GlobalConfig.Race.maxTime;
        RaceModel.traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance;
        RaceModel.traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;

        PlayNetwork.IsOffline = false;
        PlayNetwork.EloScore = Profile.EloScore;
        PlayNetwork.EloPower = Profile.CurrentRacerPower;
        PlayNetwork.MapId = RaceModel.specs.mapId;
        PlayNetwork.MaxPlayerCount = RaceModel.specs.maxPlayerCount;

        gameManager.OpenState<State_GoToRace>();
    }

    private void ClaimRewards()
    {
        Popup_Loading.Display();
        claimRewardsButton.SetInteractable(false);
        Network.SendPrizeResult(msg =>
        {
            Popup_Loading.Hide();
            claimRewardsButton.SetInteractable(true);
            if (msg != Network.Message.ok) return;

            claimRewardsButton.gameObject.SetActive(false);
            var lindex = GlobalConfig.Leagues.GetIndex(Profile.LeagueResultScore, Profile.LeagueResultPosition);
            var league = GlobalConfig.Leagues.GetByIndex(lindex);
            Profile.EarnResouce(league.rewardGem, league.rewardCoin);
            Popup_Rewards.AddResource(league.rewardGem, league.rewardCoin);

            var list = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId.Between(league.cardsGroups.x, league.cardsGroups.y));
            for (int i = 0; i < league.rewardCards; i++)
            {
                var racerid = list.RandomOne().Id;
                Profile.AddRacerCard(racerid, 1);
                Popup_Rewards.AddRacerCard(racerid, 1);
            }
            Popup_Rewards.Display();

            Profile.LeagueResultExist = false;
            ProfileLogic.SyncWidthServer(true, done => { });
        });
    }
}
