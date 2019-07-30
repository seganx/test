using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSpecialPackage : MonoBehaviour
{
    [SerializeField] private Transform racerCardHolder = null;
    [SerializeField] private LocalText customeCardsLabel = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText[] discountLabels = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button purchaseButton = null;

    private int packIndex = 0;
    private RacerConfig config = null;
    private int gems = 0;
    private int coins = 0;

    public UiShopSpecialPackage Setup(int index)
    {
        // update config or exit
        {
            var racerId = GetRacerId(index);
            if (racerId == 0)
            {
                Destroy(gameObject);
                return this;
            }
            config = RacerFactory.Racer.GetConfig(racerId);
        }
        packIndex = index;

        var pack = GlobalConfig.Shop.combinedPackages[index % GlobalConfig.Shop.combinedPackages.Count];
        var price = pack.prices[config.GroupId - 1];
        var sku = pack.skus[config.GroupId - 1];
        gems = pack.gems[config.GroupId - 1];
        coins = pack.coins[config.GroupId - 1];

        GlobalFactory.CreateRacerCard(config.Id, racerCardHolder);
        customeCardsLabel.SetFormatedText(pack.customes);
        gemsLabel.SetText(gems.ToString("#,0"));
        coinsLabel.SetText(coins.ToString("#,0"));
        priceLabel.SetFormatedText(price);
        foreach (var item in discountLabels)
            item.SetFormatedText(pack.discount);

        purchaseButton.onClick.AddListener(() =>
        {
            purchaseButton.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, sku, (success, msg) =>
            {
                purchaseButton.SetInteractable(true);
                if (success)
                {
                    DisplayRewards(pack);
                    PurchaseSystem.Consume();
                    if (transform.parent.childCount > 2)
                        Destroy(gameObject);
                    else
                        Destroy(transform.parent.gameObject);
                }
            });
        });

        return this;
    }

    private void DisplayRewards(GlobalConfig.Data.Shop.SpecialPackage pack)
    {
        Profile.EarnResouce(gems, coins);
        Popup_Rewards.AddResource(gems, coins);

        Profile.AddRacerCard(config.Id, config.CardCount);
        Popup_Rewards.AddRacerCard(config.Id, config.CardCount);

        for (int i = 0; i < pack.customes; i++)
        {
            var custom = RewardLogic.GetCustomReward(config.Id);
            Profile.AddRacerCustome(custom.type, custom.racerId, custom.customId);
            Popup_Rewards.AddCustomeCard(custom.type, custom.racerId, custom.customId);
        }

        Popup_Rewards.Display().DisplayPurchaseReward();
        ProfileLogic.SyncWidthServer(true, done => { });
        SetRacerId(packIndex, 0);
        Destroy(gameObject);
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static void ValidateAllRacerId()
    {
        for (int i = 0; i < GlobalConfig.Shop.combinedPackages.Count; i++)
        {
            var racerId = SelectRandomRacerId(i);
            if (racerId == 0) continue;
            if (IsIdExist(racerId)) racerId = SelectRandomRacerId(i);
            if (racerId == 0) continue;
            if (IsIdExist(racerId)) racerId = SelectRandomRacerId(i);
            if (racerId == 0) continue;
            if (IsIdExist(racerId)) racerId = SelectRandomRacerId(i);
            if (racerId == 0) continue;
            if (IsIdExist(racerId)) continue;
            SetRacerId(i, racerId);
        }
    }

    private static int GetRacerId(int index)
    {
        return PlayerPrefsEx.GetInt("UiShopSpecialPackage.RacerId." + index, 0);
    }

    private static void SetRacerId(int index, int id)
    {
        PlayerPrefsEx.SetInt("UiShopSpecialPackage.RacerId." + index, id);
    }

    private static bool IsIdExist(int Id)
    {
        for (int i = 0; i < GlobalConfig.Shop.combinedPackages.Count; i++)
            if (GetRacerId(i) == Id)
                return true;
        return false;
    }

    private static int SelectRandomRacerId(int index)
    {
        int radius = GlobalConfig.Probabilities.shopSpecialRacerRadius;
        int center = RewardLogic.FindSelectRacerCenter() + index * (radius + 3);
        int count = center + radius;
        var configindex = RewardLogic.SelectProbability(count, center, radius);
        if (configindex < 0 || configindex >= RacerFactory.Racer.AllConfigs.Count) return 0;
        var config = RacerFactory.Racer.AllConfigs[configindex];
        if (Profile.IsUnlockedRacer(config.Id)) return 0;
        return config.Id;
    }

    [Console("shop", "special")]
    public static void ShopSpecialTest()
    {
        for (int i = 0; i < GlobalConfig.Shop.combinedPackages.Count; i++)
            SetRacerId(i, 0);
    }
}
