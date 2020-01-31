﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSpecialPackage : MonoBehaviour
{
    [SerializeField] private LocalText customeCardsLabel = null;
    [SerializeField] private ShopItemTimerPresenter timer = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText[] discountLabels = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private LocalText realPriceLabel = null;
    [SerializeField] private LocalText racerGroupId = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private Image racerImage = null;

    private RacerConfig config = null;
    private static ShopLogic.SpecialOffer.Package purchasingPack = null;

    public UiShopSpecialPackage Setup(ShopLogic.SpecialOffer.Package pack)
    {
        config = RacerFactory.Racer.GetConfig(pack.racerId);

        timer.timerType = ShopLogic.SpecialOffer.GetTimerType(pack.packgIndex);
        racerImage.sprite = GarageRacerImager.GetImageTransparent(pack.racerId, config.DefaultRacerCustom, racerImage.rectTransform.rect.width, racerImage.rectTransform.rect.height);
        customeCardsLabel.SetFormatedText(pack.item.customes);
        gemsLabel.SetText(pack.item.gem.ToString("#,0"));
        coinsLabel.SetText(pack.item.coin.ToString("#,0"));
        priceLabel.SetFormatedText(pack.item.price);
        realPriceLabel.SetFormatedText(pack.item.realPrice);
        racerGroupId.SetFormatedText(config.GroupId);
        foreach (var item in discountLabels)
            item.SetFormatedText(pack.item.discount);

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
                    if (this != null) Destroy(gameObject, 0.1f);
                }
                else purchaseButton.SetInteractable(true);
            });
        });

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
