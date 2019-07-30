using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class CombiedShopItemTimerLogic : TimerPresenter
{
    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime < 0)
        {
            StartTimer(GlobalConfig.Shop.combinedPackagesNextTime);
            UiShopSpecialPackage.RefreshAllRacerId();
        }
    }

    public override void SetActiveTimerObjects(bool active)
    {
    }
}