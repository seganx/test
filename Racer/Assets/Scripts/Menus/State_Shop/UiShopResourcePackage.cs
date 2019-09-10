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
            purchaseButton.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.sku, (success, msg) =>
            {
                purchaseButton.SetInteractable(true);
                if (success)
                {
                    Profile.EarnResouce(pack.gems, 0);
                    Popup_Rewards.AddResource(pack.gems, 0);
                    Popup_Rewards.Display().DisplayPurchaseReward();
                    PurchaseSystem.Consume();
                    ProfileLogic.SyncWidthServer(true, done => { });

#if DATABEEN
                    DataBeen.SendPurchase(pack.sku, msg);
#endif
                }
            });
        });

        return this;
    }
}
