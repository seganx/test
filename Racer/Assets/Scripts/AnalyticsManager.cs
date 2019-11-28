using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class AnalyticsManager : MonoBehaviour
{
    void Start()
    {
        GameAnalytics.Initialize();
    }

    public static void NewBuisinessEvent(int rialAmount, string itemType)
    {
        GameAnalytics.NewBusinessEvent("USD", rialAmount / 100000, itemType, "1", "cartType");
    }
}