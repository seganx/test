using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Popup_Rewards : GameState
{
    [SerializeField] private LocalText raceRewardDesc = null;
    [SerializeField] private UiRewardItemResource gemItem = null;
    [SerializeField] private UiRewardItemResource coinItem = null;
    [SerializeField] private UiRewardRacerCard racerCardPrefab = null;
    [SerializeField] private UiRewardCustomeCard racerCustomePrefab = null;

    private System.Action onNextTaskFunc = null;
    private float delayitem = 0;

    private void Awake()
    {
        raceRewardDesc.gameObject.SetActive(false);
        gemItem.gameObject.SetActive(false);
        coinItem.gameObject.SetActive(false);
        racerCardPrefab.gameObject.SetActive(false);
        racerCustomePrefab.gameObject.SetActive(false);
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
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
        DelayCall(delayitem += 0.5f, () => gemItem.Setup(value).gameObject.SetActive(true));
        return this;
    }

    private Popup_Rewards DisplayCoins(int value)
    {
        DelayCall(delayitem += 1.0f, () => coinItem.Setup(value).gameObject.SetActive(true));
        return this;
    }

    private Popup_Rewards DisplayRacerCard(int racerId, int count)
    {
        DelayCall(delayitem += 0.75f, () => racerCardPrefab.Clone<UiRewardRacerCard>().Setup(racerId, count).gameObject.SetActive(true));
        return this;
    }

    private Popup_Rewards DisplayAddCustomeCard(RacerCustomeType type, int racerId, int customeId)
    {
        DelayCall(delayitem += 0.75f, () => racerCustomePrefab.Clone<UiRewardCustomeCard>().Setup(type, racerId, customeId).gameObject.SetActive(true));
        return this;
    }

    public Popup_Rewards DisplayDesc(params object[] args)
    {
        raceRewardDesc.gameObject.SetActive(true);
        raceRewardDesc.SetFormatedText(args);
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
            if (item.gems > 0) page.DisplayGems(item.gems);
            if (item.coins > 0) page.DisplayCoins(item.coins);
            if (item.customeType != RacerCustomeType.None)
                page.DisplayAddCustomeCard(item.customeType, item.racerid, item.customid);
            else if (item.racerid > 0 && item.count > 0)
                page.DisplayRacerCard(item.racerid, item.count);
        }
        rewards.Clear();
        return page;
    }
}
