using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Shop : GameState
{
    [SerializeField] private GameObject gemsPanel = null;
    [SerializeField] private GameObject coinsPanel = null;
    [SerializeField] private UiShopResourcePackage gemsPackagePrefab = null;
    [SerializeField] private UiShopResourcePackage coinsPackagePrefab = null;

    private System.Action onNextTask = null;

    public Popup_Shop SetupAsGems(System.Action nextTask)
    {
        onNextTask = nextTask;
        Destroy(coinsPanel);
        for (int i = 0; i < GlobalConfig.Shop.gemPackages.Count; i++)
            gemsPackagePrefab.Clone<UiShopResourcePackage>().SetupAsGemsPack(i);
        Destroy(gemsPackagePrefab.gameObject);
        return this;
    }

    public Popup_Shop SetupAsCoins(System.Action nextTask)
    {
        onNextTask = nextTask;
        Destroy(gemsPanel);
        for (int i = 0; i < GlobalConfig.Shop.coinPackages.Count; i++)
            coinsPackagePrefab.Clone<UiShopResourcePackage>().SetupAsCoinsPack(i);
        Destroy(coinsPackagePrefab.gameObject);
        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public override void Back()
    {
        base.Back();
        if (onNextTask != null) onNextTask();
    }
}
