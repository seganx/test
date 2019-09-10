using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCustomePackage : MonoBehaviour
{
    [SerializeField] private LocalText customeCount = null;
    [SerializeField] private Transform images = null;
    [SerializeField] private LocalText price = null;
    [SerializeField] private Button purchase = null;

    private System.Action callbackFunc = null;

    public UiCustomePackage Setup(RacerCustomeType type, int racerId, System.Action onPurchase)
    {
        switch (type)
        {
            case RacerCustomeType.None:
            case RacerCustomeType.Height:
            case RacerCustomeType.BodyColor:
            case RacerCustomeType.WindowColor:
            case RacerCustomeType.LightsColor:
            case RacerCustomeType.Horn:
                {
                    gameObject.SetActive(false);
                    return this;
                }

            case RacerCustomeType.Hood: UpdateVisual(type, racerId, 0, GlobalConfig.Shop.racerCosts.hood, RacerFactory.Hood.GetPrefabs(racerId)); break;
            case RacerCustomeType.Roof: UpdateVisual(type, racerId, 1, GlobalConfig.Shop.racerCosts.roof, RacerFactory.Roof.GetPrefabs(racerId)); break;
            case RacerCustomeType.Spoiler: UpdateVisual(type, racerId, 2, GlobalConfig.Shop.racerCosts.spoiler, RacerFactory.Spoiler.GetPrefabs(racerId)); break;
            case RacerCustomeType.Vinyl: UpdateVisual(type, racerId, 3, GlobalConfig.Shop.racerCosts.vinyl, RacerFactory.Vinyl.GetPrefabs(racerId)); break;
            case RacerCustomeType.Wheel: UpdateVisual(type, racerId, 4, GlobalConfig.Shop.racerCosts.wheel, RacerFactory.Wheel.GetPrefabs(racerId)); break;
        }

        callbackFunc = onPurchase;
        return this;
    }

    private void UpdateVisual(RacerCustomeType type, int racerId, int imageIndex, GlobalConfig.Data.Shop.RacerCosts.CustomCost cc, List<RacerCustomPresenter> list)
    {
        var rc = RacerFactory.Racer.GetConfig(racerId);
        var lockedList = list.FindAll(x => Profile.IsUnlockedCustom(type, racerId, x.Id) == false);
        var totalCustom = list.Count;
        var totalLocked = lockedList.Count;
        var totalUnlocked = totalCustom - totalLocked;
        var packprice = ShopLogic.GetCustomPackagePrice(totalUnlocked, cc, rc);

        var n = Mathf.Min(cc.count, totalLocked);
        customeCount.SetFormatedText(n);
        images.SetActiveChild(imageIndex);
        price.SetFormatedText(packprice);

        purchase.onClick.RemoveAllListeners();
        purchase.onClick.AddListener(() =>
        {
            Game.SpendGem(packprice, () =>
            {
                lockedList.Sort((x, y) => Random.Range(-1000, 1000));

                for (int i = 0; i < n; i++)
                {
                    var custumId = lockedList[i].Id;
                    Profile.AddRacerCustom(type, racerId, custumId);
                    Popup_Rewards.AddCustomCard(type, racerId, custumId);
                }

                Popup_Rewards.Display(callbackFunc);
            });
        });

        gameObject.SetActive(totalLocked > 0);
    }
}
