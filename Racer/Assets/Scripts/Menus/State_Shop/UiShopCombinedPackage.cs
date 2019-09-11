using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopCombinedPackage : MonoBehaviour
{
    [SerializeField] private Image coinBackground = null;
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText coinDiscountLabel = null;
    [SerializeField] private LocalText infoLabel = null;
    [SerializeField] private Text racerNameLabel = null;
    [SerializeField] private Image racerImage = null;
    [SerializeField] private LocalText gemPriceLabel = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private Sprite[] images = null;

    private RacerConfig racerConfig = null;
    private int coins = 0;
    private int totalGemPrice = 0;

    public UiShopCombinedPackage Setup(int index)
    {
        var racerId = GetRacerId(index);
        if (racerId < 1)
        {
            Destroy(gameObject);
            return this;
        }

        var pack = GlobalConfig.Shop.combinedPackages[index % GlobalConfig.Shop.combinedPackages.Count];
        coinBackground.sprite = images[index % images.Length];
        coins = pack.coin;
        coinsLabel.SetText(coins.ToString("#,0"));
        coinDiscountLabel.gameObject.SetActive(pack.coinDiscount > 0);
        coinDiscountLabel.SetFormatedText(pack.coinDiscount);

        racerConfig = RacerFactory.Racer.GetConfig(racerId);
        racerImage.sprite = racerConfig.halfIcon;
        infoLabel.SetFormatedText(pack.customes);
        racerNameLabel.text = racerConfig.Name;

        var racerGemValue = Mathf.RoundToInt(pack.racerGemValueFactor * racerConfig.Price) / 25 * 25;
        totalGemPrice = pack.coinGemValue + racerGemValue;
        gemPriceLabel.SetFormatedText(totalGemPrice);

        purchaseButton.onClick.AddListener(() =>
        {
            Game.SpendGem(totalGemPrice, () =>
            {
                DisplayRewards(pack);
                SetRacerId(index, 0);
            });
        });

        return this;
    }

    private void DisplayRewards(GlobalConfig.Data.Shop.CombinedPackage pack)
    {
        Profile.EarnResouce(0, coins);
        Popup_Rewards.AddResource(0, coins);

        Profile.AddRacerCard(racerConfig.Id, 1);
        Popup_Rewards.AddRacerCard(racerConfig.Id, 1);

        for (int i = 0; i < pack.customes; i++)
        {
            var custom = RewardLogic.GetCustomReward(racerConfig.Id);
            Profile.AddRacerCustom(custom.type, custom.racerId, custom.customId);
            Popup_Rewards.AddCustomCard(custom.type, custom.racerId, custom.customId);
        }

        Popup_Rewards.Display().DisplayPurchaseReward();
        ProfileLogic.SyncWidthServer(true, done => { });
        Destroy(gameObject);
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static int GetRacerId(int index)
    {
        var res = PlayerPrefsEx.GetInt("UiShopCombinedPackage.RacerId." + index, 0);
        if (res < 1)
        {
            res = RewardLogic.SelectRacerReward();
            if (Profile.IsUnlockedRacer(res)) res = RewardLogic.SelectRacerReward();
            if (Profile.IsUnlockedRacer(res)) res = RewardLogic.SelectRacerReward();
            if (Profile.IsUnlockedRacer(res)) res = RewardLogic.SelectRacerReward();
            if (Profile.IsUnlockedRacer(res)) res = RewardLogic.SelectRacerReward();
        }
        SetRacerId(index, res);
        return res;
    }

    public static void SetRacerId(int index, int racerId)
    {
        PlayerPrefsEx.SetInt("UiShopCombinedPackage.RacerId." + index, racerId);
    }
}
