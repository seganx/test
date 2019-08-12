using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class FreeShopItemTimerPresenter : TimerPresenter
{
    [SerializeField] private int index = 0;
    [SerializeField] private Button getButton = null;
    [SerializeField] private GameObject deactiveButtonGameObject = null;
    [SerializeField] private LocalText timerText = null;

    public override void Start()
    {
        if (index >= GlobalConfig.Shop.loadingBoxPackage.Count)
        {
            Destroy(gameObject);
            return;
        }

        base.Start();
        timerText.SetFormatedText(0, 0, 0);

        getButton.onClick.AddListener(() =>
        {
            if (State_Settings.IsFreePackageNotificationActive)
                NotificationManager.SendWithAppIcon(GlobalConfig.Shop.loadingBoxPackage[index].nextTime, NotificationType.FreePackage);
            StartTimer(GlobalConfig.Shop.loadingBoxPackage[index].nextTime);
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