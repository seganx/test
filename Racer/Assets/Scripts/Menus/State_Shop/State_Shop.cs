using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Shop : GameState
{
    [SerializeField] private UiShopSpecialPackage specialPackagePrefab = null;
    [SerializeField] private UiShopCombinedPackage combinedPackagePrefab = null;
    [SerializeField] private UiShopResourcePackage gemsPackagePrefab = null;

    private void Awake()
    {
        specialPackagePrefab.gameObject.SetActive(false);
        combinedPackagePrefab.gameObject.SetActive(false);
    }

    private void Start()
    {
        GarageCamera.SetCameraId(1);
        UiHeader.Show();
        Popup_Loading.Display();
        ProfileLogic.SyncWidthServer(false, success =>
        {
            Popup_Loading.Hide();
            if (success)
            {
                if (GlobalConfig.ForceUpdate.shop)
                {
                    gameManager.OpenPopup<Popup_Confirm>().Setup(111100, true, true, yes =>
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
                gameManager.OpenPopup<Popup_Confirm>().Setup(111060, false, true, ok => Back());
        });
    }

    private void DisplayItems()
    {
        for (int i = 0; i < GlobalConfig.Shop.gemPackages.Count; i++)
            gemsPackagePrefab.Clone<UiShopResourcePackage>().SetupAsGemsPack(i);
        Destroy(gemsPackagePrefab.gameObject);

        for (int i = 0; i < GlobalConfig.Shop.combinedPackages.Count; i++)
            combinedPackagePrefab.Clone<UiShopCombinedPackage>().Setup(i).gameObject.SetActive(true);
        Destroy(combinedPackagePrefab.gameObject);

        if (Profile.TotalRaces > 10)
        {
            ShopLogic.SpecialOffer.Refresh();
            foreach (var item in ShopLogic.SpecialOffer.Packages)
                specialPackagePrefab.Clone<UiShopSpecialPackage>().Setup(item).gameObject.SetActive(true);
        }
        Destroy(specialPackagePrefab.gameObject);

        UiShowHide.ShowAll(transform);
        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(71));
    }
}
