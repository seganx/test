using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Shop : GameState
{
    [SerializeField] private UiShopResourcePackage gemsPackagePrefab = null;

    private System.Action onNextTask = null;

    public Popup_Shop SetupAsGems(System.Action nextTask)
    {
        onNextTask = nextTask;
        for (int i = 0; i < GlobalConfig.Shop.gemPackages.Count; i++)
            gemsPackagePrefab.Clone<UiShopResourcePackage>().SetupAsGemsPack(i);
        Destroy(gemsPackagePrefab.gameObject);
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
