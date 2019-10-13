using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Popup_Rewards : GameState
{
    [SerializeField] private GameObject titleReward = null;
    [SerializeField] private LocalText titleRaceReward = null;
    [SerializeField] private LocalText titlePurchaseReward = null;
    [SerializeField] private RectTransform rewardContent = null;
    [SerializeField] private UiRewardItemResource gemPrefab = null;
    [SerializeField] private UiRewardItemResource coinPrefab = null;
    [SerializeField] private UiRewardRacerCard racerCardPrefab = null;
    [SerializeField] private UiRewardCustomeCard racerCustomePrefab = null;
    [SerializeField] private UiShowHide bottomBar = null;
    [SerializeField] private LocalText inventoryDesc = null;
    [SerializeField] private Button inventoryButton = null;


    private System.Action onNextTaskFunc = null;
    private ScrollRect scroller = null;
    private float contentPos = 0;

    private void Awake()
    {
        titleReward.gameObject.SetActive(true);
        titleRaceReward.gameObject.SetActive(false);
        titlePurchaseReward.gameObject.SetActive(false);

        scroller = rewardContent.GetComponentInParent<ScrollRect>(true);
        for (int i = 0; i < rewardContent.childCount; i++)
            rewardContent.GetChild(i).gameObject.SetActive(false);

        inventoryButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Inventory>().Setup(UpdateInventoryDesc));
    }

    private IEnumerator Start()
    {
        if (gemPrefab) Destroy(gemPrefab.gameObject);
        if (coinPrefab) Destroy(coinPrefab.gameObject);
        if (racerCardPrefab) Destroy(racerCardPrefab.gameObject);
        if (racerCustomePrefab) Destroy(racerCustomePrefab.gameObject);

        for (int i = 0; i < rewardContent.childCount; i++)
            rewardContent.GetChild(i).gameObject.SetActive(false);

        UpdateInventoryDesc();
        UiShowHide.ShowAll(transform);
        bottomBar.Hide();
        yield return new WaitForSeconds(0.5f);
        contentPos = rewardContent.anchoredPosition.x;

        // first open racer cards
        for (int i = 0; i < rewardContent.childCount; i++)
        {
            var item = rewardContent.GetChild<UiRewardRacerCard>(i);
            if (item == null) continue;

            // display and wait
            item.gameObject.SetActive(true);
            yield return new WaitUntil(() => item.IsOpened);
            yield return new WaitForSeconds(0.1f);
            if (i < rewardContent.childCount - 1)
            {
                item = rewardContent.GetChild<UiRewardRacerCard>(i + 1);
                if (item != null) MoveContentToLeft(item.rectTransform.rect.width);
            }
        }
        bottomBar.Show();

        // open remained items
        for (int i = 0; i < rewardContent.childCount; i++)
        {
            var item = rewardContent.GetChild<RectTransform>(i);
            if (item.gameObject.activeSelf) continue;

            // wait and display
            yield return new WaitForSeconds(0.75f);
            item.gameObject.SetActive(true);
            if (i < rewardContent.childCount - 1)
            {
                item = rewardContent.GetChild<RectTransform>(i);
                if (item != null) MoveContentToLeft(item.rect.width);
            }
        }
    }

    private void UpdateInventoryDesc()
    {
        int rcards = Popup_Inventory.ComputeNumberOfCards();
        inventoryDesc.SetFormatedText(rcards);
        inventoryDesc.gameObject.SetActive(rcards > 0);
    }

    private void MoveContentToLeft(float itemWidth)
    {
        if (rewardContent.rect.width + rewardContent.anchoredPosition.x > 800) return;
        contentPos = contentPos - (itemWidth + 50);
    }

    private void Update()
    {
        if (Mathf.Abs(scroller.velocity.x) > 0)
        {
            contentPos = rewardContent.anchoredPosition.x;
        }
        else
        {
            var pos = rewardContent.anchoredPosition;
            pos.x += (contentPos - pos.x) * Time.deltaTime * 20;
            rewardContent.anchoredPosition = pos;
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
        racerCardPrefab.Clone<UiRewardRacerCard>().Setup(racerId, count).transform.SetAsFirstSibling();
        Popup_RateUs.SetPlayerInjoy(true, 3);
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

    public static void AddCustomCard(RacerCustomeType type, int _racerId, int _customeId)
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

    public static Popup_Rewards Display(RewardLogic.RaceReward reward, System.Action onNextTask = null)
    {
        if (reward.racerId > 0)
            AddRacerCard(reward.racerId, reward.racerCount);
        if (reward.custome != null)
            AddCustomCard(reward.custome.type, reward.custome.racerId, reward.custome.customId);
        if (reward.gems > 0)
            AddResource(reward.gems, 0);
        if (reward.coins > 0)
            AddResource(0, reward.coins);
        return Display(onNextTask);
    }
}
