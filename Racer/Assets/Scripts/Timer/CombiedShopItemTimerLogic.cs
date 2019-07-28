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
        // call your function here

        //
        if (remainTime < 0)
            StartTimer(GlobalConfig.Shop.combinedPackagesNextTime);
    }

    public override void SetActiveTimerObjects(bool active)
    {
    }
}