using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopRacerCardPackage : MonoBehaviour
{
    [System.Serializable]
    private class RacerCardPackage
    {
        public int racerId = 0;
        public int racerCardsCount = 0;
        public int count = 0;
        public int maxCount = 0;
        public bool priceIsCoins = false;
        public int basePrice = 0;
        public float priceRatio = 0;
        public int price { get { return Mathf.RoundToInt(basePrice + priceRatio * (maxCount - count)); } }
    }

    [SerializeField] private Transform cardHolder = null;
    [SerializeField] private LocalText neededCards = null;
    [SerializeField] private LocalText remainedCards = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private GameObject gemImage = null;
    [SerializeField] private GameObject coinImage = null;
    [SerializeField] private GameObject purchaseGameObject = null;
    [SerializeField] private Button purchaseButton = null;

    private int index = 0;
    private RacerCardPackage pack = null;

    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    public UiShopRacerCardPackage Setup(int pindex)
    {
        index = pindex;
        Display();
        purchaseButton.onClick.AddListener(() =>
        {
            if (pack.priceIsCoins)
                Game.SpendCoin(pack.price, OnPurchased);
            else
                Game.SpendGem(pack.price, OnPurchased);
        });
        return this;
    }

    private void OnPurchased()
    {
        Profile.AddRacerCard(pack.racerId, 1);
        pack.count--;
        SavePackages();
        Display();
    }

    private void Display()
    {
        pack = GetPackage(index);
        var rp = Profile.GetRacer(pack.racerId);
        int playerRemained = pack.racerCardsCount - (rp != null ? rp.cards : 0);

        cardHolder.RemoveChildren();
        GlobalFactory.CreateRacerCard(pack.racerId, cardHolder);
        neededCards.SetFormatedText(playerRemained);
        neededCards.gameObject.SetActive(playerRemained > 0);
        remainedCards.SetFormatedText(pack.count);
        priceLabel.SetText(pack.price.ToString());

        gemImage.SetActive(pack.priceIsCoins == false);
        coinImage.SetActive(pack.priceIsCoins == true);

        purchaseButton.SetInteractable(pack.count > 0);
        purchaseGameObject.SetActive(pack.count > 0);
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static List<UiShopRacerCardPackage> all = new List<UiShopRacerCardPackage>();
    private static List<RacerCardPackage> packages = null;

    private static RacerCardPackage GetPackage(int index)
    {
        if (packages == null) LoadPackages();
        return packages[index % packages.Count];
    }

    private static void LoadPackages()
    {
        packages = PlayerPrefsEx.Deserialize<List<RacerCardPackage>>("RacerCardPackages", null);
        if (packages == null) CreatePackages();
    }

    private static void SavePackages()
    {
        if (packages == null) return;
        PlayerPrefsEx.Serialize("RacerCardPackages", packages);
    }

    public static void CreatePackages()
    {
        packages = new List<RacerCardPackage>(GlobalConfig.Shop.racerCardPackages.Count);
        foreach (var item in GlobalConfig.Shop.racerCardPackages)
        {
            var config = RacerFactory.Racer.AllConfigs[RewardLogic.SelectProbabilityForward(RacerFactory.Racer.AllConfigs.Count, RewardLogic.FindSelectRacerCenter(), 10, 0.5f)];
            while (packages.Exists(x => x.racerId == config.Id))
                config = RacerFactory.Racer.AllConfigs[RewardLogic.SelectProbabilityForward(RacerFactory.Racer.AllConfigs.Count, RewardLogic.FindSelectRacerCenter(), 10, 0.5f)];

            var newpack = new RacerCardPackage();
            newpack.racerId = config.Id;
            newpack.racerCardsCount = config.CardCount;
            newpack.count = newpack.maxCount = item.maxCount;
            newpack.basePrice = Mathf.RoundToInt(config.Price * item.basePriceFactor);
            newpack.priceRatio = config.Price * item.priceRatio;

            newpack.priceIsCoins = Random.Range(0, 100) < 50;
            if (newpack.priceIsCoins)
            {
                newpack.basePrice *= GlobalConfig.Shop.gemToCoin;
                newpack.priceRatio *= GlobalConfig.Shop.gemToCoin;
            }

            packages.Add(newpack);
        }
        SavePackages();

        foreach (var item in all)
            item.Display();
    }
}
