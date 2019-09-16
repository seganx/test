using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class UiBlackMarketTimer : TimerPresenter
{
    [SerializeField] private LocalText timerText = null;
    [SerializeField] private LocalText refreshPriceText = null;
    [SerializeField] private LocalText refreshRemainCounterText = null;
    [SerializeField] private Button refreshBuyButton = null;

    public int RefreshRemainCount
    {
        get { return PlayerPrefsEx.GetInt("UiBlackMarketTimer.RefreshRemainCount", 3); }
        set { PlayerPrefsEx.SetInt("UiBlackMarketTimer.RefreshRemainCount", value); }
    }

    public override void Start()
    {
        base.Start();

        UpdateRefreshGui();
        refreshBuyButton.onClick.AddListener(() =>
        {
            Game.SpendGem(GlobalConfig.Shop.blackMarket.refreshPrices[RefreshRemainCount - 1], () =>
            {
                RefreshRemainCount--;
                UpdateRefreshGui();
                UiBlackMarketPackage.CreatePackages();
            });
        });
        timerText.SetFormatedText(0, 0, 0);
    }

    void UpdateRefreshGui()
    {
        if (RefreshRemainCount > 0)
        {
            refreshBuyButton.SetInteractable(true);
            refreshPriceText.SetText(GlobalConfig.Shop.blackMarket.refreshPrices[RefreshRemainCount - 1].ToString());
        }
        else
            refreshBuyButton.SetInteractable(false);

        refreshRemainCounterText.SetFormatedText(RefreshRemainCount, GlobalConfig.Shop.blackMarket.refreshPrices.Length);
    }

    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime >= 0)
            timerText.SetFormatedText(remainTime / 3600, remainTime % 3600 / 60, remainTime % 60);
        else
            RemainedTimeFinished(remainTime);
    }

    private void RemainedTimeFinished(int remainTime)
    {
        UiBlackMarketPackage.CreatePackages();
        int newTime = GlobalConfig.Shop.blackMarket.refreshTime + remainTime % GlobalConfig.Shop.blackMarket.refreshTime;
        if (State_Settings.IsLegendStoreActive)
            NotificationManager.SendWithAppIcon(newTime, NotificationType.LegendStore);

        RefreshRemainCount = 3;
        UpdateRefreshGui();
        StartTimer(newTime);
    }

    public override void SetActiveTimerObjects(bool active) { }

}