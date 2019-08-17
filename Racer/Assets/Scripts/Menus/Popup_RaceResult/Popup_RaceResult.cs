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

    public Popup_RaceResult Setup(System.Action onNextTask)
    {
        RaceLogic.OnRaceFinished();

#if DATABEEN
        if (RewardLogic.IsFirstRace)
        {
            RewardLogic.IsFirstRace = false;
            DataBeen.SendCustomEventData("first_race", new DataBeenConnection.CustomEventInfo[] {
                new DataBeenConnection.CustomEventInfo() { key = "result " + BotPresenter.currentCount + " bot", value = playerCurPosition.ToString() },
            });
        }

#endif

        nextButton.onClick.AddListener(() =>
        {
            base.Back();

            var rewardsList = RaceModel.IsOnline ? GlobalConfig.Race.rewardsOnline : GlobalConfig.Race.rewardsOffline;
            var preward = rewardsList[Mathf.Clamp(RaceModel.stats.playerPosition, 0, rewardsList.Count - 1)];

            var raceReward = RewardLogic.GetRaceReward(preward.racerCardChance, preward.customeChance, preward.gemChance, preward.gems);
            if (raceReward.custome != null)
            {
                Profile.AddRacerCustome(raceReward.custome.type, raceReward.custome.racerId, raceReward.custome.customId);
                Popup_Rewards.AddCustomeCard(raceReward.custome.type, raceReward.custome.racerId, raceReward.custome.customId);
            }
            if (raceReward.racerId > 0)
            {
                Profile.AddRacerCard(raceReward.racerId, 1);
                Popup_Rewards.AddRacerCard(raceReward.racerId, 1);
            }
            if (raceReward.gem > 0)
            {
                Profile.EarnResouce(raceReward.gem, 0);
                Popup_Rewards.AddResource(raceReward.gem, 0);
            }

            Profile.EarnResouce(0, preward.coins);
            Popup_Rewards.AddResource(0, preward.coins);

            Popup_Rewards.Display(onNextTask).DisplayRacerReward(rewardsList[0].coins, rewardsList[1].coins, rewardsList[2].coins, rewardsList[3].coins);
        });

        Popup_RateUs.SetPlayerInjoy(RaceModel.stats.playerPosition < 1);
        return this;
    }

    private void Start()
    {
        positionLabel.SetFormatedText(RaceModel.stats.playerPosition + 1);
        foreach (var player in PlayerPresenter.allPlayers)
        {
            var rac = RacerFactory.Racer.GetConfig(player.RacerId);
            if (rac == null) continue;
            var item = prefabItem.Clone<UiRaceResultItem>().Setup(player.CurrPosition + 1, player.name, rac.Name, player.RacerPower, player.Score,
                player.IsPlayer ? RaceLogic.onlineResult.rewardScore : GlobalConfig.Race.positionScore[player.CurrPosition]);

            if (player.IsPlayer) item.GetComponent<Image>().color = Color.blue;
        }
        Destroy(prefabItem.gameObject);

        if (RaceModel.IsOnline == false)
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
        if (RaceLogic.onlineResult.lastLeague > 0)
        {
            var prevIndex = RaceLogic.onlineResult.lastLeague - 1;
            var prevLeague = GlobalConfig.Leagues.GetByIndex(prevIndex);
            prevLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(prevIndex);
            prevScoreLabel.SetText(prevLeague.startRank > 0 ? prevLeague.startRank.ToString("#,0") : prevLeague.startScore.ToString("#,0"));
        }
        else prevLeagueIcon.transform.parent.gameObject.SetActive(false);

        //  display current league
        {
            var currLeague = GlobalConfig.Leagues.GetByIndex(RaceLogic.onlineResult.lastLeague);
            currLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(RaceLogic.onlineResult.lastLeague);
            currScoreLabel.SetText(currLeague.startRank > 0 ? Profile.PositionString : RaceLogic.onlineResult.lastScore.ToString("#,0"));
            addScoreLabel.gameObject.SetActive(RaceLogic.onlineResult.rewardScore > 0 && currLeague.startRank == 0);
            addScoreLabel.SetText(RaceLogic.onlineResult.rewardScore.ToString());
        }

        //  display next league
        if (RaceLogic.onlineResult.lastLeague < GlobalConfig.Leagues.list.Count - 1)
        {
            var nextIndex = RaceLogic.onlineResult.lastLeague + 1;
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
