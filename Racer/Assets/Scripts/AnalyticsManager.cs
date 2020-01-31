using GameAnalyticsSDK;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private void Start()
    {
        GameAnalytics.Initialize();
    }

    public static void NewBuisinessEvent(Online.Purchase.Provider provider, int rialAmount, string sku, string token)
    {
        Online.Purchase.Verify(provider, sku, token, (success, playload) =>
        {
            if (success)
                GameAnalytics.NewBusinessEvent("USD", rialAmount / 100, sku, "1", "cartType");
        });
    }
}