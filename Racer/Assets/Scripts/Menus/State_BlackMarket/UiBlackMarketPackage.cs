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
    private static List<UiBlackMarketPackage> all = new List<UiBlackMarketPackage>();
    private static List<Package> packages = null;

    private static Package GetPackage(int index)
    {
        if (packages == null) LoadPackages();
        return packages.Count > 0 ? packages[index % packages.Count] : null;
    }

    private static bool PackageExist(int id)
    {
        return packages.Exists(x => x.racerId == id);
    }

    private static void LoadPackages()
    {
        packages = PlayerPrefsEx.DeserializeBinary<List<Package>>("UiBlackMarketPackage", null);
        if (packages == null) CreatePackages();
    }

    private static void SavePackages()
    {
        if (packages == null) return;
        PlayerPrefsEx.SerializeBinary("UiBlackMarketPackage", packages);
    }

    private static int SelectRandomRacerId(int index)
    {
        int radius = GlobalConfig.Probabilities.blackmarketRacerRadius;
        int center = RewardLogic.FindSelectRacerCenter() + index * (radius + 2);
        int count = center + radius;
        var configindex = RewardLogic.SelectProbability(count, center, radius);
        if (configindex < 0 || configindex >= RacerFactory.Racer.AllConfigs.Count) return 0;
        var config = RacerFactory.Racer.AllConfigs[configindex];
        if (PackageExist(config.Id) || Profile.IsUnlockedRacer(config.Id)) return 0;
        return config.Id;
    }

    private static RacerConfig SelectRacer(int index)
    {
        var id = SelectRandomRacerId(index);
        if (id == 0) id = SelectRandomRacerId(index);
        if (id == 0) id = SelectRandomRacerId(index);
        if (id == 0) id = SelectRandomRacerId(index);
        if (id == 0) id = SelectRandomRacerId(index);
        if (id == 0) id = SelectRandomRacerId(index);
        if (id == 0) id = SelectRandomRacerId(index);
        return RacerFactory.Racer.GetConfig(id);
    }

    public static void CreatePackages()
    {
        packages = new List<Package>(GlobalConfig.Shop.blackMarketPackages.Count);
        for (int i = 0; i < GlobalConfig.Shop.blackMarketPackages.Count; i++)
        {
            var item = GlobalConfig.Shop.blackMarketPackages[i];
            var config = SelectRacer(i);
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

            packages.Add(newpack);
        }
        SavePackages();

        foreach (var item in all)
            item.Display();
    }
}
