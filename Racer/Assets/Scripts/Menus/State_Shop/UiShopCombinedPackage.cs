using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopCombinedPackage : Base
{
    [SerializeField] private Image coinBackground = null;
    [SerializeField] private LocalText coinsLabel = null;
    [SerializeField] private LocalText coinDiscountLabel = null;
    [SerializeField] private LocalText infoLabel = null;
    //[SerializeField] private Text racerNameLabel = null;
    //[SerializeField] private Image racerImage = null;
    [SerializeField] private LocalText gemPriceLabel = null;
    [SerializeField] private Button purchaseButton = null;
    [SerializeField] private Sprite[] images = null;

    public UiShopCombinedPackage Setup(int index)
    {
        var pack = GlobalConfig.Shop.combinedPackages[index % GlobalConfig.Shop.combinedPackages.Count];
        coinBackground.sprite = images[index % images.Length];
        coinsLabel.SetText(pack.coin.ToString("#,0"));
        coinDiscountLabel.gameObject.SetActive(pack.coinDiscount > 0);
        coinDiscountLabel.SetFormatedText(pack.coinDiscount);
        infoLabel.SetFormatedText(pack.racers, pack.customes);
        gemPriceLabel.SetFormatedText(pack.price);

        purchaseButton.onClick.AddListener(() =>
        {
            purchaseButton.SetInteractable(false);
            Game.SpendGem(pack.price, () => DisplayRewards(pack));
            DelayCall(1, () => purchaseButton.SetInteractable(true));
        });

        return this;
    }

    private void DisplayRewards(GlobalConfig.Data.Shop.CombinedPackage pack)
    {
        Profile.EarnResouce(0, pack.coin);
        Popup_Rewards.AddResource(0, pack.coin);

        var racersList = new List<int>(pack.racers);
        for (int i = 0; i < pack.racers; i++)
        {
            var index = RewardLogic.SelectProbability(RacerFactory.Racer.AllConfigs.Count, pack.racersCenter, pack.racersRaduis);
            var racerid = RacerFactory.Racer.AllConfigs[index].Id;
            racersList.Add(racerid);
            Profile.AddRacerCard(racerid, 1);
            Popup_Rewards.AddRacerCard(racerid, 1);
        }

        for (int i = 0; i < pack.customes; i++)
        {
            var custom = RewardLogic.GetCustomReward(racersList.RandomOne());
            Profile.AddRacerCustom(custom.type, custom.racerId, custom.customId);
            Popup_Rewards.AddCustomCard(custom.type, custom.racerId, custom.customId);
        }

        Popup_Rewards.Display().DisplayPurchaseReward();
        ProfileLogic.SyncWidthServer(true, done => { });

#if DATABEEN
        DataBeen.SendContentView("CombinedPack_" + pack.price, "CombinedPack_" + pack.price);
#endif
    }
}
