using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ShopSpecialPackage : GameState
{
    [SerializeField] private ShopItemTimerPresenter timer = null;
    [SerializeField] private Image racerImage = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Transform cardHolder = null;
    [SerializeField] private LocalText racerGroupLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private LocalText realPriceLabel = null;
    [SerializeField] private LocalText discountLabel = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private LocalText customeCardsLabel = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText coinsLabel = null;

    private RacerConfig config = null;
    private static ShopLogic.SpecialOffer.Package purchasingPack = null;

    public Popup_ShopSpecialPackage Setup(ShopLogic.SpecialOffer.Package pack, System.Action<ShopLogic.SpecialOffer.Package> onPurchase)
    {
        timer.timerType = ShopLogic.SpecialOffer.GetTimerType(pack.packgIndex);
        config = RacerFactory.Racer.GetConfig(pack.racerId);
        racerImage.sprite = GarageRacerImager.GetImageTransparent(config.Id, config.DefaultRacerCustom, racerImage.rectTransform.rect.width, racerImage.rectTransform.rect.height);
        descLabel.SetFormatedText(pack.item.discount, config.CardCount);
        GlobalFactory.CreateRacerCard(pack.racerId, cardHolder);
        racerGroupLabel.SetFormatedText(config.GroupId);
        priceLabel.SetFormatedText(pack.item.price.ToString("#,0"));
        realPriceLabel.SetFormatedText(pack.item.realPrice.ToString("#,0"));
        discountLabel.SetText(pack.item.discount.ToString());
        customeCardsLabel.SetFormatedText(pack.item.customes);
        gemsLabel.SetText(pack.item.gem.ToString("#,0"));
        coinsLabel.SetText(pack.item.coin.ToString("#,0"));

        purchaseButton.onClick.AddListener(() =>
        {
            purchasingPack = pack;
            purchaseButton.SetInteractable(false);

            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.item.sku, (success, token) =>
            {
                if (success)
                {
                    DisplayRewards(purchasingPack);
                    PurchaseSystem.Consume(purchasingPack.item.sku);
                    AnalyticsManager.NewBuisinessEvent(Online.Purchase.Provider.Cafebazaar, purchasingPack.item.price, purchasingPack.item.sku, token);
                    if (this != null) Back();
                    if (onPurchase != null) onPurchase(purchasingPack);
                }
                else purchaseButton.SetInteractable(true);
            });
        });

        UiShowHide.ShowAll(transform);
        return this;
    }

    private void DisplayRewards(ShopLogic.SpecialOffer.Package pack)
    {
        Profile.EarnResouce(pack.item.gem, pack.item.coin);
        Popup_Rewards.AddResource(pack.item.gem, pack.item.coin);

        Profile.AddRacerCard(config.Id, config.CardCount);
        Popup_Rewards.AddRacerCard(config.Id, config.CardCount);

        for (int i = 0; i < pack.item.customes; i++)
        {
            var custom = RewardLogic.GetCustomReward(config.Id);
            Profile.AddRacerCustom(custom.type, custom.racerId, custom.customId);
            Popup_Rewards.AddCustomCard(custom.type, custom.racerId, custom.customId);
        }

        Popup_Rewards.Display().DisplayPurchaseReward();
        ProfileLogic.SyncWidthServer(true, done => { });
    }
}
