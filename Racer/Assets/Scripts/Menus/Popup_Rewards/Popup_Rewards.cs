using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Popup_Rewards : GameState
{
    [SerializeField] private GameObject titleReward = null;
    [SerializeField] private LocalText titleRaceReward = null;
    [SerializeField] private LocalText titlePurchaseReward = null;
    [SerializeField] private Transform rewardContent = null;
    [SerializeField] private UiRewardItemResource gemPrefab = null;
    [SerializeField] private UiRewardItemResource coinPrefab = null;
    [SerializeField] private UiRewardRacerCard racerCardPrefab = null;
    [SerializeField] private UiRewardCustomeCard racerCustomePrefab = null;

    private System.Action onNextTaskFunc = null;
    private float delayitem = 0;

    private void Awake()
    {
        titleReward.gameObject.SetActive(true);
        titleRaceReward.gameObject.SetActive(false);
        titlePurchaseReward.gameObject.SetActive(false);

        for (int i = 0; i < rewardContent.childCount; i++)
            rewardContent.GetChild(i).gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < rewardContent.childCount; i++)
            rewardContent.GetChild(i).gameObject.SetActive(false);

        UiShowHide.ShowAll(transform);

        var waitTime = new WaitForSeconds(0.5f);
        for (int i = 0; i < rewardContent.childCount; i++)
        {
            var item = rewardContent.GetChild(i);
            if (gemPrefab && gemPrefab.transform == item) continue;
            if (coinPrefab && coinPrefab.transform == item) continue;
            if (racerCardPrefab && racerCardPrefab.transform == item) continue;
            if (racerCustomePrefab && racerCustomePrefab.transform == item) continue;
            yield return waitTime;
            item.gameObject.SetActive(true);
        }
    }

    public override void Back()
    {
        base.Back();
        if (onNextTaskFunc != null) onNextTaskFunc();
    }

    public Popup_Rewards SetCallback(System.Action onNextTask = null)
    {
        onNextTaskFunc = onNextTask;
        return this;
    }

    private Popup_Rewards DisplayGems(int value)
    {
        gemPrefab.Clone<UiRewardItemResource>().Setup(value).transform.SetAsLastSibling();
        return this;
    }

    private Popup_Rewards DisplayCoins(int value)
    {
        coinPrefab.Clone<UiRewardItemResource>().Setup(value).transform.SetAsLastSibling();
        return this;
    }

    private Popup_Rewards DisplayRacerCard(int racerId, int count)
    {
        racerCardPrefab.Clone<UiRewardRacerCard>().Setup(racerId, count).transform.SetAsLastSibling();
        return this;
    }

    private Popup_Rewards DisplayAddCustomeCard(RacerCustomeType type, int racerId, int customeId)
    {
        racerCustomePrefab.Clone<UiRewardCustomeCard>().Setup(type, racerId, customeId).transform.SetAsLastSibling();
        return this;
    }

    public Popup_Rewards DisplayRacerReward(params object[] args)
    {
        titleRaceReward.gameObject.SetActive(true);
        titleRaceReward.SetFormatedText(args);
        return this;
    }

    public Popup_Rewards DisplayPurchaseReward()
    {
        titleReward.SetActive(false);
        titlePurchaseReward.gameObject.SetActive(true);
        return this;
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    private class RewardItem
    {
        public int gems = 0;
        public int coins = 0;
        public int racerid = 0;
        public int customid = 0;
        public int count = 0;
        public RacerCustomeType customeType = RacerCustomeType.None;
    }

    private static List<RewardItem> rewards = new List<RewardItem>(10);

    public static void AddResource(int _gems, int _coins)
    {
        var exist = rewards.Find(x => x.gems > 0 || x.coins > 0);
        if (exist != null)
        {
            exist.gems += _gems;
            exist.coins += _coins;
        }
        else rewards.Add(new RewardItem() { gems = _gems, coins = _coins });
    }

    public static void AddRacerCard(int _racerId, int _count)
    {
        var exist = rewards.Find(x => x.racerid == _racerId && x.count > 0 && x.customeType == RacerCustomeType.None);
        if (exist != null)
            exist.count += _count;
        else
            rewards.Add(new RewardItem() { racerid = _racerId, count = _count });
    }

    public static void AddCustomeCard(RacerCustomeType type, int _racerId, int _customeId)
    {
        if (rewards.Exists(x => x.customeType == type && x.racerid == _racerId && x.customid == _customeId)) return;
        rewards.Add(new RewardItem() { customeType = type, racerid = _racerId, customid = _customeId });
    }

    public static Popup_Rewards Display(System.Action onNextTask = null)
    {
        var page = gameManager.OpenPopup<Popup_Rewards>().SetCallback(onNextTask);
        foreach (var item in rewards)
        {
            if (item.racerid > 0 && item.count > 0)
                page.DisplayRacerCard(item.racerid, item.count);
            if (item.customeType != RacerCustomeType.None)
                page.DisplayAddCustomeCard(item.customeType, item.racerid, item.customid);
            if (item.gems > 0) page.DisplayGems(item.gems);
            if (item.coins > 0) page.DisplayCoins(item.coins);
        }
        rewards.Clear();
        return page;
    }
}
