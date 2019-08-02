using SeganX;
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

        startButton.onClick.AddListener(() => gameManager.OpenState<State_Garage>().Setup(() =>
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
        PlayModel.OfflineMode = false;
        PlayModel.eloScore = Profile.EloScore;
        PlayModel.eloPower = Profile.CurrentRacerPower;
        PlayModel.selectedMapId = PlayModel.SelectRandomMap();
        PlayModel.maxPlayerCount = 4;
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
            for (int i = 0; i < league.rewardCards; i++)
            {
                var racerid = RewardLogic.SelectRacerReward();
                Profile.AddRacerCard(racerid, 1);
                Popup_Rewards.AddRacerCard(racerid, 1);
            }
            Popup_Rewards.Display();
        });
    }
}
