using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSpecialRacerCards : MonoBehaviour
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private Transform cardHolder = null;
    [SerializeField] private LocalText neededCards = null;
    [SerializeField] private LocalText[] discountLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private RacerSpecialOfferTimerPresenter timer = null;
    [SerializeField] private GameObject timerGameObject = null;

    public UiShopSpecialRacerCards Setup(GlobalConfig.Data.Shop.SpecialRacerCardPackage pack, bool isPopup)
    {
        if (isPopup == false)
        {
            Destroy(timer);
            Destroy(timerGameObject);
        }

        title.SetFormatedText(pack.cardCount);
        GlobalFactory.CreateRacerCard(pack.racerId, cardHolder);

        var rp = Profile.GetRacer(pack.racerId);
        int playerRemained = RacerFactory.Racer.GetConfig(pack.racerId).CardCount - (rp != null ? rp.cards : 0);
        neededCards.SetFormatedText(playerRemained);

        foreach (var item in discountLabel)
            item.SetFormatedText(pack.discount);
        priceLabel.SetFormatedText(pack.price);

        purchaseButton.onClick.AddListener(() =>
        {
            purchaseButton.SetInteractable(false);
            PurchaseSystem.Purchase(PurchaseProvider.Bazaar, pack.sku, (success, msg) =>
            {
                purchaseButton.SetInteractable(true);
                if (success)
                {
                    DisplayRewards(pack);
                    PurchaseSystem.Consume();
                    if (isPopup) ShopLogic.SpecialRacerPopup.Clear();
                }
            });
        });

        return this;
    }

    private void DisplayRewards(GlobalConfig.Data.Shop.SpecialRacerCardPackage pack)
    {
        for (int i = 0; i < pack.cardCount; i++)
        {
            Profile.AddRacerCard(pack.racerId, 1);
            Popup_Rewards.AddRacerCard(pack.racerId, 1);
        }

        Popup_Rewards.Display();
        ProfileLogic.SyncWidthServer(true, done => { });
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static bool CanDisplay(GlobalConfig.Data.Shop.SpecialRacerCardPackage pack, bool mustHasLessCards)
    {
        if (pack.racerId < 1 || pack.cardCount < 1) return false;

        if (Profile.IsUnlockedRacer(pack.racerId)) return false;

        var sku = pack.sku + "_" + pack.racerId + "_" + pack.cardCount;
        if (Profile.IsPurchased(sku)) return false;

        if (mustHasLessCards)
        {
            var rp = Profile.GetRacer(pack.racerId);
            int racerCardsCount = RacerFactory.Racer.GetConfig(pack.racerId).CardCount;
            int playerCardsCount = rp != null ? rp.cards : 0;
            int condition = racerCardsCount * pack.maxCardFactor / 100;
            if (playerCardsCount >= condition) return false;
        }

        return true;
    }
}
