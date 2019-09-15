using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSpecialPopup : MonoBehaviour
{
    [SerializeField] private UiShowHide holder = null;
    [SerializeField] private LocalText infoLabel = null;
    [SerializeField] private ShopItemTimerPresenter timer = null;
    [SerializeField] private Button button = null;


    private void Awake()
    {
        holder.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        if (Profile.TotalRaces > 10)
        {
            ShopLogic.SpecialOffer.Refresh();

            if (ShopLogic.SpecialOffer.Packages.Count > 0)
            {
                // update the button view
                var package = ShopLogic.SpecialOffer.Packages.LastOne();
                if (ShopLogic.SpecialOffer.CanDisplay(package.packgIndex))
                {
                    infoLabel.SetFormatedText(package.item.discount);
                    timer.timerType = ShopLogic.SpecialOffer.GetTimerType(package.packgIndex);
                    button.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_ShopSpecialPackage>().Setup(package, pack => Destroy(holder.gameObject)));
                    timer.gameObject.SetActive(true);
                    holder.gameObject.SetActive(true);
                    holder.Show();
                }

                if (firstRun)
                {
                    yield return new WaitForSeconds(1);
                    yield return new WaitUntil(() => Game.Instance.CurrentPopup == null);
                    firstRun = false;
                    foreach (var item in ShopLogic.SpecialOffer.Packages)
                    {
                        Game.Instance.OpenPopup<Popup_ShopSpecialPackage>().Setup(item, pack =>
                        {
                            if (pack == package)
                                Destroy(holder.gameObject);
                        });
                    }
                }
                else if (popupId != package.racerId)
                {
                    button.onClick.Invoke();
                }

                popupId = package.racerId;
            }
        }
        else Destroy(holder.gameObject);
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static bool firstRun = true;
    private static int popupId = 0;
}
