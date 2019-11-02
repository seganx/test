using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_RaceResult : GameState
{
    [SerializeField] private UiRaceResultItem prefabItem = null;
    [SerializeField] private LocalText positionLabel = null;
    [SerializeField] private LocalText distanceLabel = null;
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
        nextButton.onClick.AddListener(() =>
        {
            base.Back();
            onNextTask();
        });

        Popup_RateUs.SetPlayerInjoy(RaceModel.stats.playerRank < 2, 2);

        return this;
    }

    private void Start()
    {
        positionLabel.SetFormatedText(RaceModel.stats.playerRank + 1);
        distanceLabel.SetFormatedText(RaceModel.stats.playerBehindDistance.ToString("0.0"));
        distanceLabel.transform.parent.gameObject.SetActive(RaceModel.stats.playerBehindDistance > 0);

        foreach (var racer in PlayerPresenter.all)
        {
            var rac = RacerFactory.Racer.GetConfig(racer.player.RacerId);
            if (rac == null) continue;
            var item = prefabItem.Clone<UiRaceResultItem>().Setup(
                racer, 
                racer.player.CurrRank + 1, 
                racer.player.name, 
                rac.Name, 
                racer.player.RacerPower, 
                racer.player.Score,
                racer.player.IsPlayer ? RaceLogic.raceResult.rewardScore : GlobalConfig.Race.positionScore[racer.player.CurrRank]);

            if (racer.player.IsPlayer) item.GetComponent<Image>().color = Color.blue;
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
        if (RaceLogic.raceResult.lastLeague > 0)
        {
            var prevIndex = RaceLogic.raceResult.lastLeague - 1;
            var prevLeague = GlobalConfig.Leagues.GetByIndex(prevIndex);
            prevLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(prevIndex);
            prevScoreLabel.SetText(prevLeague.startRank > 0 ? prevLeague.startRank.ToString("#,0") : prevLeague.startScore.ToString("#,0"));
        }
        else prevLeagueIcon.transform.parent.gameObject.SetActive(false);

        //  display current league
        {
            var currLeague = GlobalConfig.Leagues.GetByIndex(RaceLogic.raceResult.lastLeague);
            currLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(RaceLogic.raceResult.lastLeague);
            currScoreLabel.SetText(currLeague.startRank > 0 ? Profile.PositionString : RaceLogic.raceResult.lastScore.ToString("#,0"));
            addScoreLabel.gameObject.SetActive(RaceLogic.raceResult.rewardScore > 0 && currLeague.startRank == 0);
            addScoreLabel.SetText(RaceLogic.raceResult.rewardScore.ToString());
        }

        //  display next league
        if (RaceLogic.raceResult.lastLeague < GlobalConfig.Leagues.list.Count - 1)
        {
            var nextIndex = RaceLogic.raceResult.lastLeague + 1;
            var nextLeague = GlobalConfig.Leagues.GetByIndex(nextIndex);
            nextLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(nextIndex);
            nextScoreLabel.SetText(nextLeague.startRank > 0 ? nextLeague.startRank.ToString("#,0") : nextLeague.startScore.ToString("#,0"));
        }
        else nextLeagueIcon.transform.parent.gameObject.SetActive(false);
    }

    public override void Back()
    {
        nextButton.onClick.Invoke();
    }
}
