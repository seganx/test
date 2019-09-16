using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBlackMarketPackage : MonoBehaviour
{
    [System.Serializable]
    private class Package
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
    private Package pack = null;

    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    public UiBlackMarketPackage Setup(int pindex)
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
        if (pack == null)
        {
            Destroy(gameObject);
            return;
        }

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
    [System.Serializable]
    private class SerializableData
    {
        public List<Package> packages = new List<Package>();
    }

    private static List<UiBlackMarketPackage> all = new List<UiBlackMarketPackage>();
    private static SerializableData data = new SerializableData();

    private static Package GetPackage(int index)
    {
        if (data.packages.Count < 1) LoadPackages();
        return data.packages.Count > 0 ? data.packages[index % data.packages.Count] : null;
    }

    private static bool PackageExist(int id)
    {
        return data.packages.Exists(x => x.racerId == id);
    }

    private static void LoadPackages()
    {
        data = PlayerPrefsEx.Deserialize("UiBlackMarketPackage", new SerializableData());
        if (data.packages.Count < 1) CreatePackages();
    }

    private static void SavePackages()
    {
        PlayerPrefsEx.Serialize("UiBlackMarketPackage", data);
    }

    public static void CreatePackages()
    {
        data.packages = new List<Package>(GlobalConfig.Shop.blackMarket.packages.Count);

        int model = 0;
        for (int i = 0; i < GlobalConfig.Shop.blackMarket.packages.Count; i++)
        {
            var item = GlobalConfig.Shop.blackMarket.packages[i];
            var config = SelectRacer(i, model);

            // validate search result and change model
            if (i == 1) model = 1;
            if (config == null)
            {
                if (model == 1) model = 2;
                config = SelectRacer(i, model);
            }


            // create package
            if (config == null) continue;
            var newpack = new Package();
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

            data.packages.Add(newpack);
        }

        // save packages
        SavePackages();

        // display packages
        foreach (var item in all)
            item.Display();
    }

    private static RacerConfig SelectRacer(int index, int model)
    {
        var id = SelectRandomRacerId(index, model);
        for (int i = 0; i < 20 && id == 0; i++)
            id = SelectRandomRacerId(index, model);
        return RacerFactory.Racer.GetConfig(id);
    }

    private static int SelectRandomRacerId(int packindex, int model)
    {
        switch (model)
        {
            case 0: return SelectRandomRacerId_Bell(packindex);
            case 1: return SelectRandomRacerId_Interested();
            case 2: return SelectRandomRacerId_Suggestion();
        }
        return 0;
    }

    private static int SelectRandomRacerId_Bell(int packindex)
    {
        int radius = GlobalConfig.Probabilities.blackmarketRacerRadius;
        int center = RewardLogic.FindSelectRacerCenter() + packindex * radius;
        int count = center + radius;
        var index = RewardLogic.SelectProbability(count, center, radius);
        if (index < 0 || index >= RacerFactory.Racer.AllConfigs.Count) return 0;
        var config = RacerFactory.Racer.AllConfigs[index];
        if (PackageExist(config.Id) || Profile.IsUnlockedRacer(config.Id)) return 0;
        return config.Id;
    }

    private static int SelectRandomRacerId_Interested()
    {
        var currConfig = RacerFactory.Racer.GetConfig(Profile.SelectedRacer);
        if (currConfig == null) return 0;
        var configList = RacerFactory.Racer.AllConfigs.FindAll(x =>
        x.GroupId != currConfig.GroupId &&
        Profile.IsUnlockedRacer(x.Id) == false &&
        Profile.GetCurrentRacerCards(x.Id) > GlobalConfig.Shop.blackMarket.interestedSearch.minReqCards[x.GroupId]);
        if (configList.Count < GlobalConfig.Shop.blackMarket.interestedSearch.minCandidateCount) return 0;
        var res = configList.RandomOne().Id;
        return PackageExist(res) ? 0 : res;
    }

    private static int SelectRandomRacerId_Suggestion()
    {
        var bestList = GlobalConfig.Shop.blackMarket.suggestionSearch.bestRacerIds.FindAll(x => PackageExist(x) == false && Profile.IsUnlockedRacer(x) == false);
        if (bestList.Count > 0) return bestList.RandomOne();

        var currConfig = RacerFactory.Racer.GetConfig(Profile.SelectedRacer);
        if (currConfig == null) return 0;
        var configList = RacerFactory.Racer.AllConfigs.FindAll(x =>
        x.GroupId != currConfig.GroupId &&
        PackageExist(x.Id) == false &&
        Profile.IsUnlockedRacer(x.Id) == false &&
        Profile.GetCurrentRacerCards(x.Id) < GlobalConfig.Shop.blackMarket.suggestionSearch.maxReqCards[x.GroupId]);
        if (configList.Count > 0) return configList.RandomOne().Id;

        configList = RacerFactory.Racer.AllConfigs.FindAll(x => PackageExist(x.Id) == false && Profile.IsUnlockedRacer(x.Id) == false);
        return (configList.Count > 0) ? configList.RandomOne().Id : 0;
    }
}
