using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopResourcePackage : MonoBehaviour
{
    [SerializeField] private Image background = null;
    [SerializeField] private LocalText amountLabel = null;
    [SerializeField] private LocalText discountLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private Sprite[] images = null;

    private static GlobalConfig.Data.Shop.GemPackage purchasingPack = null;

    public UiShopResourcePackage SetupAsGemsPack(int index)
    {
        var pack = GlobalConfig.Shop.gemPackages[index % GlobalConfig.Shop.gemPackages.Count];
        background.sprite = images[index % images.Length];
        amountLabel.SetText(pack.gems.ToString("#,0"));
        discountLabel.gameObject.SetActive(pack.discount > 0);
        discountLabel.SetFormatedText(pack.discount);
        priceLabel.SetFormatedText(pack.price);

        purchaseButton.onClick.AddListener(() =>
        {
            purchasingPack = pack;
            purchaseButton.SetInteractable(false);

            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, purchasingPack.sku, (success, token) =>
            {
                if (success)
                {
                    Profile.EarnResouce(purchasingPack.gems, 0);
                    Popup_Rewards.AddResource(purchasingPack.gems, 0);
                    Popup_Rewards.Display().DisplayPurchaseReward();
                    PurchaseSystem.Consume(purchasingPack.sku);
                    AnalyticsManager.NewBuisinessEvent(Online.Purchase.Provider.Cafebazaar, purchasingPack.price, purchasingPack.sku, token);
                    ProfileLogic.SyncWidthServer(true, done => { });
                }

                if (this != null) purchaseButton.SetInteractable(true);
            });
        });

        return this;
    }
}
