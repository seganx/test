using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class FreeShopItemTimerPresenter : TimerPresenter
{
    [SerializeField] private Button getButton;
    [SerializeField] private GameObject deactiveButtonGameObject;
    [SerializeField] private LocalText timerText;

    public override void Start()
    {
        base.Start();
        timerText.SetFormatedText(0, 0, 0);

        getButton.onClick.AddListener(() =>
        {
            if (State_Settings.IsFreePackageNotificationActive)
                NotificationManager.SendWithAppIcon(GlobalConfig.Shop.loadingBoxPackage.nextTime, NotificationType.FreePackage);
            StartTimer(GlobalConfig.Shop.loadingBoxPackage.nextTime);
        });
    }

    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime >= 0)
            timerText.SetFormatedText(remainTime / 3600, remainTime % 3600 / 60, remainTime % 60);
    }

    public override void SetActiveTimerObjects(bool active)
    {
        getButton.gameObject.SetActive(!active);
        if (deactiveButtonGameObject)
            deactiveButtonGameObject.SetActive(active);
    }
}