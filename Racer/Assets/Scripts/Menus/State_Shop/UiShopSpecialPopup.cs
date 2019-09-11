using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiShopSpecialPopup : MonoBehaviour
{
    [SerializeField] private LocalText infoLabel = null;
    [SerializeField] private ShopItemTimerPresenter timer = null;

    private IEnumerator Start()
    {
        ShopLogic.SpecialOffer.Refresh();
        if (ShopLogic.SpecialOffer.Packages.Count > 0)
        {
            var package = ShopLogic.SpecialOffer.Packages.LastOne();
            if (ShopLogic.SpecialOffer.CanDisplay(package.packgIndex))
            {
                infoLabel.SetFormatedText(package.item.discount);
                timer.timerType = ShopLogic.SpecialOffer.GetTimerType(package.packgIndex);
                timer.gameObject.SetActive(true);

                if (CanDisplayPopup)
                {
                    yield return new WaitForSeconds(1);
                    Game.Instance.OpenPopup<Popup_ShopSpecialPackage>().Setup(package, () =>
                    {
                        package = null;
                    });
                }
            }
        }
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static bool CanDisplayPopup = true;
}
