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
        rewardButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_LeaguePrize>());
        leaderboardButton.onClick.AddListener(() => gameManager.OpenState<State_Leaderboards>());

        var leagueInfo = GlobalConfig.Leagues.GetByIndex(Profile.League);
        startButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(leagueInfo.startGroup, () =>
        {
            if (Profile.IsUnlockedRacer(GarageRacer.racer.Id))
                StartOnlineGame();
            else
                gameManager.OpenPopup<Popup_RacerCardInfo>();
        }));

        UiShowHide.ShowAll(transform);
    }

    private void StartOnlineGame()
    {
        PlayModel.mode = PlayModel.Mode.Online;
        PlayNetwork.IsOffline = false;
        PlayNetwork.EloScore = Profile.EloScore;
        PlayNetwork.EloPower = Profile.CurrentRacerPower;
        PlayNetwork.MapId = PlayModel.mapId = PlayModel.SelectRandomMap();
        PlayNetwork.MaxPlayerCount = PlayModel.maxPlayerCount = 4;
        PlayModel.maxPlayTime = GlobalConfig.Race.maxTime;
        PlayModel.Traffic.baseDistance = GlobalConfig.Race.traffics.baseDistance;
        PlayModel.Traffic.distanceRatio = GlobalConfig.Race.traffics.speedFactor;
        gameManager.OpenState<State_FindOpponents>();
    }

    private void ClaimRewards()
    {
        Network.SendPrizeResult(msg =>
        {
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
        });
    }
}
