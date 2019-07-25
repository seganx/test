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

    public UiShopResourcePackage SetupAsCoinsPack(int index)
    {
        var pack = GlobalConfig.Shop.coinPackages[index % GlobalConfig.Shop.coinPackages.Count];
        background.sprite = images[index % images.Length];
        amountLabel.SetText(pack.coins.ToString("#,0"));
        discountLabel.gameObject.SetActive(pack.discount > 0);
        discountLabel.SetFormatedText(pack.discount);
        priceLabel.SetFormatedText(pack.price);

        purchaseButton.onClick.AddListener(() =>
        {
            Game.SpendGem(pack.price, () =>
            {
                Profile.EarnResouce(0, pack.coins);
                Popup_Rewards.AddResource(0, pack.coins);
                Popup_Rewards.Display();
            });
        });

        return this;
    }

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
                PurchaseSystem.Consume();
                purchaseButton.SetInteractable(true);
                if (success)
                {
                    Profile.EarnResouce(pack.gems, 0);
                    Popup_Rewards.AddResource(pack.gems, 0);
                    Popup_Rewards.Display();
                    ProfileLogic.SyncWidthServer(true, done => { });
                }
            });
        });

        return this;
    }
}
