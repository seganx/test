using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_RaceResult : GameState
{
    [SerializeField] private UiRaceResultItem prefabItem = null;
    [SerializeField] private LocalText positionLabel = null;
    [SerializeField] private Image prevLeagueIcon = null;
    [SerializeField] private Image currLeagueIcon = null;
    [SerializeField] private Image nextLeagueIcon = null;
    [SerializeField] private LocalText prevScoreLabel = null;
    [SerializeField] private LocalText currScoreLabel = null;
    [SerializeField] private LocalText nextScoreLabel = null;
    [SerializeField] private LocalText addScoreLabel = null;
    [SerializeField] private Button nextButton = null;
    private int playerCurLeague = 0;
    private int playerCurPosition = 0;
    private int playerCurScore = 0;
    private int playerAddScore = 0;

    public Popup_RaceResult Setup(System.Action onNextTask)
    {
        playerCurPosition = PlayerPresenter.local.player.CurrPosition;

#if DATABEEN
        if (RewardLogic.IsFirstRace)
        {
            RewardLogic.IsFirstRace = false;
            DataBeen.SendCustomEventData("first_race", new DataBeenConnection.CustomEventInfo[] {
                new DataBeenConnection.CustomEventInfo() { key = "result " + BotPresenter.currentCount + " bot", value = playerCurPosition.ToString() },
            });
        }

#endif

        //  update player score
        if (PlayModel.OfflineMode == false)
        {
            playerCurScore = Profile.Score + 1;
            playerCurLeague = Profile.League;
            playerAddScore = GlobalConfig.Race.positionScore[playerCurPosition];
            Profile.Score = playerCurScore + playerAddScore;
            Network.SendScore(Profile.Score);

            Profile.Skill += 2 * PlayModel.maxPlayerCount - 4 * playerCurPosition;
        }
        else Profile.Skill += 2 * PlayModel.maxPlayerCount - 4 * playerCurPosition;


        nextButton.onClick.AddListener(() =>
        {
            base.Back();

            var rewardsList = PlayModel.OfflineMode ? GlobalConfig.Race.rewardsOffline : GlobalConfig.Race.rewardsOnline;
            var preward = rewardsList[Mathf.Clamp(playerCurPosition, 0, rewardsList.Count - 1)];
            Profile.EarnResouce(0, preward.coins);
            Popup_Rewards.AddResource(0, preward.coins);

            var raceReward = RewardLogic.GetRaceReward(preward.racerCardChance, preward.customeChance, preward.gemChance, preward.gems);
            if (raceReward.gem > 0)
            {
                Profile.EarnResouce(raceReward.gem, 0);
                Popup_Rewards.AddResource(raceReward.gem, 0);
            }
            else if (raceReward.racerId > 0)
            {
                Profile.AddRacerCard(raceReward.racerId, 1);
                Popup_Rewards.AddRacerCard(raceReward.racerId, 1);
            }
            else if (raceReward.custome != null)
            {
                Profile.AddRacerCustome(raceReward.custome.type, raceReward.custome.racerId, raceReward.custome.customId);
                Popup_Rewards.AddCustomeCard(raceReward.custome.type, raceReward.custome.racerId, raceReward.custome.customId);
            }
            Popup_Rewards.Display(onNextTask).DisplayRacerReward(rewardsList[0].coins, rewardsList[1].coins, rewardsList[2].coins, rewardsList[3].coins);
        });

        Popup_RateUs.SetPlayerInjoy(playerCurPosition < 1);
        return this;
    }

    private void Start()
    {
        positionLabel.SetFormatedText(playerCurPosition + 1);
        foreach (var player in PlayerPresenter.allPlayers)
        {
            var rac = RacerFactory.Racer.GetConfig(player.RacerId);
            if (rac == null) continue;
            var item = prefabItem.Clone<UiRaceResultItem>().Setup(player.CurrPosition + 1, player.name, rac.Name, player.RacerPower, player.Score, GlobalConfig.Race.positionScore[player.CurrPosition]);
            if (player.IsPlayer) item.GetComponent<Image>().color = Color.blue;
        }
        Destroy(prefabItem.gameObject);

        if (PlayModel.OfflineMode)
        {
            prevLeagueIcon.transform.parent.gameObject.SetActive(false);
            currLeagueIcon.transform.parent.gameObject.SetActive(false);
            nextLeagueIcon.transform.parent.gameObject.SetActive(false);
        }
        else DisplayLeagues();

        UiShowHide.ShowAll(transform);
    }

    private void DisplayLeagues()
    {
        //  display prev league
        if (playerCurLeague > 0)
        {
            var prevIndex = playerCurLeague - 1;
            var prevLeague = GlobalConfig.Leagues.GetByIndex(prevIndex);
            prevLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(prevIndex);
            prevScoreLabel.SetText(prevLeague.startRank > 0 ? prevLeague.startRank.ToString("#,0") : prevLeague.startScore.ToString("#,0"));
        }
        else prevLeagueIcon.transform.parent.gameObject.SetActive(false);

        //  display current league
        {
            var currLeague = GlobalConfig.Leagues.GetByIndex(playerCurLeague);
            currLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(playerCurLeague);
            currScoreLabel.SetText(currLeague.startRank > 0 ? Profile.PositionString : playerCurScore.ToString("#,0"));
            addScoreLabel.gameObject.SetActive(playerAddScore > 0 && currLeague.startRank == 0);
            addScoreLabel.SetText(playerAddScore.ToString());
        }

        //  display next league
        if (playerCurLeague < GlobalConfig.Leagues.list.Count - 1)
        {
            var nextIndex = playerCurLeague + 1;
            var nextLeague = GlobalConfig.Leagues.GetByIndex(nextIndex);
            nextLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(nextIndex);
            nextScoreLabel.SetText(nextLeague.startRank > 0 ? nextLeague.startRank.ToString("#,0") : nextLeague.startScore.ToString("#,0"));
        }
        else nextLeagueIcon.transform.parent.gameObject.SetActive(false);
    }

    public override void Back()
    {
        base.Back();
        nextButton.onClick.Invoke();
    }
}
