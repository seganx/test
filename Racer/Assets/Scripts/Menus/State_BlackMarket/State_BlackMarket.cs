using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_BlackMarket : GameState
{
    [SerializeField] private UiBlackMarketPackage racerCardPackagePrefab = null;

    private void Start()
    {
        UiHeader.Show();
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
        for (int i = 0; i < GlobalConfig.Shop.blackMarketPackages.Count; i++)
            racerCardPackagePrefab.Clone<UiBlackMarketPackage>().Setup(i);
        Destroy(racerCardPackagePrefab.gameObject);

        UiShowHide.ShowAll(transform);
    }
}
