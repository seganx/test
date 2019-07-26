using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Shop : GameState
{
    [SerializeField] private UiShopResourcePackage coinsPackagePrefab = null;
    [SerializeField] private UiShopResourcePackage gemsPackagePrefab = null;
    [SerializeField] private UiShopSpecialPackage specialPackagePrefab = null;
    [SerializeField] private UiShopSpecialRacerCards specialRacerCardsPackagePrefab = null;


    private void Start()
    {
        Popup_Loading.Display();
        ProfileLogic.SyncWidthServer(false, success =>
        {
            Popup_Loading.Hide();
            if (success)
            {
                if (GlobalConfig.ForceUpdate.shop)
                {
                    gameManager.OpenPopup<Popup_Confirm>().Setup(111100, true, yes =>
                    {
                        if (yes)
                        {
                            Application.OpenURL(GlobalConfig.Socials.storeUrl);
                            Application.Quit();
                        }
                        else Back();
                    });
                }
                else DisplayItems();
            }
            else
                gameManager.OpenPopup<Popup_Confirm>().Setup(111060, false, ok => Back());
        });
    }

    private void DisplayItems()
    {
        for (int i = 0; i < GlobalConfig.Shop.coinPackages.Count; i++)
            coinsPackagePrefab.Clone<UiShopResourcePackage>().SetupAsCoinsPack(i);
        Destroy(coinsPackagePrefab.gameObject);

        for (int i = 0; i < GlobalConfig.Shop.gemPackages.Count; i++)
            gemsPackagePrefab.Clone<UiShopResourcePackage>().SetupAsGemsPack(i);
        Destroy(gemsPackagePrefab.gameObject);

        if (ShopLogic.SpecialRacerPopup.IsAvailable)
            specialRacerCardsPackagePrefab.Clone<UiShopSpecialRacerCards>().Setup(ShopLogic.SpecialRacerPopup.Package, true);

        foreach (var item in GlobalConfig.Shop.specialRacerCardPackages)
            if (UiShopSpecialRacerCards.CanDisplay(item, true))
                specialRacerCardsPackagePrefab.Clone<UiShopSpecialRacerCards>().Setup(item, false);

        Destroy(specialRacerCardsPackagePrefab.gameObject);

        UiShopSpecialPackage.ValidateAllRacerId();
        for (int i = 0; i < GlobalConfig.Shop.specialPackages.Count; i++)
            specialPackagePrefab.Clone<UiShopSpecialPackage>().Setup(i);

        if (specialPackagePrefab.transform.parent.childCount < 4)
            Destroy(specialPackagePrefab.transform.parent.gameObject);
        else
            Destroy(specialPackagePrefab.gameObject);

        UiShowHide.ShowAll(transform);
    }
}
