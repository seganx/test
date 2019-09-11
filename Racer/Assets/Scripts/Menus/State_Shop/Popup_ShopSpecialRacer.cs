﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ShopSpecialPackage : GameState
{
    [SerializeField] private Image racerImage = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Transform cardHolder = null;
    [SerializeField] private LocalText cardsLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private LocalText realPriceLabel = null;
    [SerializeField] private LocalText discountLabel = null;
    [SerializeField] private Button purchaseButton = null;

    public Popup_ShopSpecialPackage Setup(ShopLogic.SpecialOffer.Package pack, System.Action onPurchase)
    {
#if off
        var rconfig = RacerFactory.Racer.GetConfig(pack.PopupRacerId);
        racerImage.sprite = GarageRacerImager.GetImageTransparent(rconfig.Id, rconfig.DefaultRacerCustom, racerImage.rectTransform.rect.width, racerImage.rectTransform.rect.height);
        descLabel.SetFormatedText(pack.discount, rconfig.CardCount);
        GlobalFactory.CreateRacerCard(pack.PopupRacerId, cardHolder);
        cardsLabel.SetFormatedText(rconfig.CardCount);
        priceLabel.SetFormatedText(pack.price.ToString("#,0"));
        discountLabel.SetText(pack.discount.ToString());

        purchaseButton.onClick.AddListener(() =>
        {
            purchaseButton.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.sku, (success, msg) =>
            {
                purchaseButton.SetInteractable(true);
                if (success)
                {
                    Back();
                    ShopLogic.SpecialRacerPopup.Clear();
                    Profile.AddRacerCard(pack.PopupRacerId, rconfig.CardCount);
                    Popup_Rewards.AddRacerCard(pack.PopupRacerId, rconfig.CardCount);
                    Popup_Rewards.Display().DisplayPurchaseReward();
                    PurchaseSystem.Consume();
                    ProfileLogic.SyncWidthServer(true, done => { });
                    if (onPurchase != null) onPurchase();
#if DATABEEN
                    DataBeen.SendPurchase(pack.sku, msg);
#endif
                }
            });
        });

#endif
        UiShowHide.ShowAll(transform);
        return this;
    }
}
