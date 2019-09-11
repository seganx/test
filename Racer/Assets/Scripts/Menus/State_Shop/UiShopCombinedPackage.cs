using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopCombinedPackage : MonoBehaviour
{
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText coinDiscountLabel = null;
    [SerializeField] private Image coinBackground = null;
    [SerializeField] private LocalText racerInfoLabel = null;
    [SerializeField] private Image racerImage = null;
    [SerializeField] private LocalText gemPriceLabel = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private Sprite[] images = null;

    //private int packIndex = 0;
    private RacerConfig racerConfig = null;
    private int coins = 0;
    private int totalGemPrice = 0;

    public void Start()
    {
        Setup(0);
    }

    public UiShopCombinedPackage Setup(int index)
    {
        var pack = GlobalConfig.Shop.combinedPackages[index % GlobalConfig.Shop.combinedPackages.Count];
        coinBackground.sprite = images[index % images.Length];
        coins = pack.coin;
        coinsLabel.SetText(coins.ToString("#,0"));
        coinDiscountLabel.gameObject.SetActive(pack.coinDiscount > 0);
        coinDiscountLabel.SetFormatedText(pack.coinDiscount);

        var racerId = pack.racerIndex; // todo_
        racerConfig = RacerFactory.Racer.GetConfig(racerId);
        racerImage.sprite = racerConfig.halfIcon;
        racerInfoLabel.SetFormatedText(pack.customes);

        var racerGemValue = Mathf.RoundToInt(pack.racerGemValueFactor * racerConfig.Price) / 25 * 25;
        totalGemPrice = pack.coinGemValue + racerGemValue;
        gemPriceLabel.SetFormatedText(totalGemPrice);

        purchaseButton.onClick.AddListener(() =>
        {
            purchaseButton.SetInteractable(false);
            Game.SpendGem(totalGemPrice, () =>
            {
                purchaseButton.SetInteractable(true);
                DisplayRewards(pack);
                PurchaseSystem.Consume();
                Destroy(gameObject);

#if DATABEEN
                //DataBeen.SendPurchase(sku, msg);
#endif
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
}
