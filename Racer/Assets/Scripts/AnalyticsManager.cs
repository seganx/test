using GameAnalyticsSDK;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Base
{
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameAnalytics.Initialize();
    }

    private static AnalyticsManager instance = null;

    public static void NewBuisinessEvent(Online.Purchase.Provider provider, int rialAmount, string sku, string token)
    {
        instance.DelayCall(10, () =>
        {
            Online.Purchase.Verify(provider, sku, token, (success, playload) =>
            {
                if (success)
                    GameAnalytics.NewBusinessEvent("USD", rialAmount / 100, sku, "1", "cartType");
            });
        });
    }
}