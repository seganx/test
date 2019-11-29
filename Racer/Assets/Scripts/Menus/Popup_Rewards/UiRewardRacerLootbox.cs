using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRewardRacerLootbox : Base
{
    [SerializeField] private LocalText chanceLabel = null;
    [SerializeField] private RectTransform chanceCircle = null;
    [SerializeField] private Image circleImage = null;
    [SerializeField] private Image circlePlusImage = null;
    [SerializeField] private UiRacerCard racerInfo = null;
    [SerializeField] private Button spinButton = null;
    [SerializeField] private Animation animator = null;

    private RacerConfig config = null;
    private int rewardLootValue = 0;

    public bool IsOpened { get; private set; }

    private void Awake()
    {
        var rewardsList = RaceModel.IsOnline ? GlobalConfig.Race.rewardsOnline : GlobalConfig.Race.rewardsOffline;
        var rewardata = rewardsList[Mathf.Clamp(RaceModel.stats.playerRank, 0, rewardsList.Count - 1)];
        if (rewardata.cardLootFactor > 0)
        {
            // find a league which player has at least one car
            var lindex = Profile.League;
            var cars = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId == (lindex + 1));
            if (cars.Exists(x => Profile.IsUnlockedRacer(x.Id)) == false) lindex--;
            var league = GlobalConfig.Leagues.GetByIndex(lindex);

            // select a racer
            var selectedId = GetLastRacerId(lindex);
            if (selectedId > 0 && Profile.IsUnlockedRacer(selectedId) == false)
            {
                config = RacerFactory.Racer.GetConfig(selectedId);
            }
            else
            {
                int rindex = Random.Range(0, 100) % 3;
                config = Profile.IsUnlockedRacer(league.lootboxRacerIds[rindex]) ? null : RacerFactory.Racer.GetConfig(league.lootboxRacerIds[rindex]);
                if (config == null) config = Profile.IsUnlockedRacer(league.lootboxRacerIds[++rindex % 3]) ? null : RacerFactory.Racer.GetConfig(league.lootboxRacerIds[rindex]);
                if (config == null) config = Profile.IsUnlockedRacer(league.lootboxRacerIds[++rindex % 3]) ? null : RacerFactory.Racer.GetConfig(league.lootboxRacerIds[rindex]);
            }

            if (config != null)
            {
                rewardLootValue = league.lootboxValue * rewardata.cardLootFactor / 100;
                SetLastRacerId(lindex, config.Id);
            }
        }

        if (config == null)
            gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        if (config == null || racerInfo == null)
        {
            gameObject.SetActive(false);
            yield break;
        }
        else
        {

            circleImage.SetFillAmount(Profile.CardLootboxChance, 100);
            circlePlusImage.SetFillAmount(Profile.CardLootboxChance, 100);
            racerInfo.Setup(config);
            spinButton.onClick.AddListener(() => StartCoroutine(DoSpin()));

            // add loot reward
            {
                float currLootvalue = Profile.CardLootboxChance;
                Profile.CardLootboxChance += rewardLootValue;
                float nextLootValue = Profile.CardLootboxChance;

                var wait = new WaitForEndOfFrame();
                while (Mathf.Abs(currLootvalue - nextLootValue) > 1)
                {
                    yield return wait;
                    currLootvalue = Mathf.Lerp(currLootvalue, nextLootValue, Time.deltaTime);
                    circlePlusImage.SetFillAmount(currLootvalue, 100);
                    chanceLabel.SetFormatedText(currLootvalue + 0.5f);
                }
            }
        }
    }

    private IEnumerator DoSpin()
    {
        spinButton.gameObject.SetActive(false);

        float chanceValue = Random.Range(0, 1000) * 0.001f;
        bool rewarded = (chanceValue * 100) <= Profile.CardLootboxChance;
        if (rewarded)
        {
            Profile.AddRacerCard(config.Id, 1);
            Profile.CardLootboxChance = 0;
        }

        float currDegree = 0;
        float nextDegree = 2 * 360 + chanceValue * 360 + 90;
        var wait = new WaitForEndOfFrame();
        while (Mathf.Abs(currDegree - nextDegree) > 1)
        {
            yield return wait;
            currDegree = Mathf.Lerp(currDegree, nextDegree, Time.deltaTime * 3);
            chanceCircle.localEulerAngles = Vector3.forward * currDegree;
        }

        if (rewarded)
            animator.Play();

        yield return new WaitForSeconds(1);
        IsOpened = true;
    }

    private int GetLastRacerId(int leagueIndex)
    {
        return PlayerPrefs.GetInt("Lootbox.RacerId." + leagueIndex, 0);
    }

    private void SetLastRacerId(int leagueIndex, int racerId)
    {
        PlayerPrefs.SetInt("Lootbox.RacerId." + leagueIndex, racerId);
    }
}
