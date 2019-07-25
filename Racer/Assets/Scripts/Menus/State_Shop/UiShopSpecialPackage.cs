using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSpecialPackage : MonoBehaviour
{
    [SerializeField] private LocalText racerCardsLabel = null;
    [SerializeField] private LocalText customeCardsLabel = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText discountLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button purchaseButton = null;

    public UiShopSpecialPackage Setup(int index)
    {
        var pack = GlobalConfig.Shop.specialPackages[index % GlobalConfig.Shop.specialPackages.Count];
        racerCardsLabel.SetFormatedText(pack.racerCards);
        customeCardsLabel.SetFormatedText(pack.customes);
        gemsLabel.SetText(pack.gems.ToString("#,0"));
        coinsLabel.SetText(pack.coins.ToString("#,0"));
        discountLabel.SetFormatedText(pack.discount);
        priceLabel.SetFormatedText(pack.price);

        purchaseButton.onClick.AddListener(() =>
        {
            purchaseButton.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.sku, (success, msg) =>
            {
                PurchaseSystem.Consume();
                purchaseButton.SetInteractable(true);
                if (success) DisplayRewards(pack);
            });
        });

        return this;
    }

    private void DisplayRewards(GlobalConfig.Data.Shop.SpecialPackage pack)
    {
        Profile.EarnResouce(pack.gems, pack.coins);
        Popup_Rewards.AddResource(pack.gems,pack.coins);

        for (int i = 0; i < pack.racerCards; i++)
        {
            var racerid = RewardLogic.SelectRacerReward(true);
            Profile.AddRacerCard(racerid, 1);
            Popup_Rewards.AddRacerCard(racerid, 1);
        }

        for (int i = 0; i < pack.customes; i++)
        {
            var custom = RewardLogic.GetCustomReward();
            Profile.AddRacerCustome(custom.type, custom.racerId, custom.customId);
            Popup_Rewards.AddCustomeCard(custom.type, custom.racerId, custom.customId);
        }

        Popup_Rewards.Display();
        ProfileLogic.SyncWidthServer(true, done => { });
    }
}
