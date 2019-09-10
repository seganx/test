using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSpecialPackage : MonoBehaviour
{
    [SerializeField] private LocalText customeCardsLabel = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText[] discountLabels = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private LocalText realPriceLabel = null;
    [SerializeField] private LocalText racerGroupId = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private Image racerImage = null;

    private int packIndex = 0;
    private RacerConfig config = null;
    private int gems = 0;
    private int coins = 0;

    public UiShopSpecialPackage Setup(int index)
    {
        packIndex = index;
        var racerId = 0;// GetRacerId(index);
        config = RacerFactory.Racer.GetConfig(racerId);

        var pack = GlobalConfig.Shop.combinedPackages[index % GlobalConfig.Shop.combinedPackages.Count];
        var price = pack.price;
        var sku = pack.sku;
        gems = pack.gem;
        coins = pack.coin;

        racerImage.sprite = GarageRacerImager.GetImageTransparent(racerId, config.DefaultRacerCustom, racerImage.rectTransform.rect.width, racerImage.rectTransform.rect.height);
        customeCardsLabel.SetFormatedText(pack.customes);
        gemsLabel.SetText(gems.ToString("#,0"));
        coinsLabel.SetText(coins.ToString("#,0"));
        priceLabel.SetFormatedText(price);
        realPriceLabel.SetFormatedText(pack.realPrice);
        racerGroupId.SetFormatedText(config.GroupId);
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
                    Destroy(gameObject);

#if DATABEEN
                    DataBeen.SendPurchase(sku, msg);
#endif
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
