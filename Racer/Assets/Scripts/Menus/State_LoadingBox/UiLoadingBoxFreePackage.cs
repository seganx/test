using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class UiLoadingBoxFreePackage : TimerPresenter
{
    [SerializeField] private int index = 0;
    [SerializeField] private LocalText dealyDesc = null;
    [SerializeField] private LocalText countDesc = null;
    [SerializeField] private LocalText remainedLabel = null;

    [SerializeField] private Button getButton = null;
    [SerializeField] private GameObject deactiveButtonGameObject = null;
    [SerializeField] private LocalText timerText = null;

    [SerializeField] private Button buyButton = null;
    [SerializeField] private LocalText gemPriceText = null;

    private GlobalConfig.Data.Shop.LoadingBox data = null;

    private bool IsSameDay
    {
        get { return PlayerPrefsEx.GetInt(name + ".day", 0) == TimerManager.ServerTime.DayOfYear; }
        set { if (value) PlayerPrefsEx.SetInt(name + ".day", TimerManager.ServerTime.DayOfYear); }
    }

    public override void Start()
    {
        data = index < GlobalConfig.Shop.loadingBoxPackage.Count ? GlobalConfig.Shop.loadingBoxPackage[index] : null;
        if (data != null)
        {
            if (IsSameDay == false)
            {
                IsSameDay = true;
                Profile.UsedFreeItem = 0;
            }

            UpdateVisual();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        base.Start();
        timerText.SetFormatedText(0, 0, 0);

        getButton.onClick.AddListener(() =>
        {
            Profile.UsedFreeItem++;

            int newTime = 0;
            if (data.dailyCount > Profile.UsedFreeItem)
                newTime = GlobalConfig.Shop.loadingBoxPackage[index].nextTime;
            else
            {
                DateTime now = TimerManager.ServerTime;
                int hours = 0, minutes = 0, seconds = 0;
                hours = (24 - now.Hour) - 1;
                minutes = (60 - now.Minute) - 1;
                seconds = (60 - now.Second - 1);

                newTime = seconds + (minutes * 60) + (hours * 3600);
            }

            if (State_Settings.IsFreePackageNotificationActive && newTime > 3600)
                NotificationManager.SendWithAppIcon(newTime, NotificationType.FreePackage);
            StartTimer(newTime);

            UpdateVisual();

            ShowReward();
        });

        gemPriceText.SetText(data.gemPrice.ToString());
        buyButton.onClick.AddListener(() =>
        {
            Game.SpendGem(data.gemPrice, ShowReward);
        });
    }

    public void ShowReward()
    {
        if (UnityEngine.Random.Range(0, 100) < data.cardChance)
        {
            var gems = data.gemValues.RandomOne();
            Profile.EarnResouce(gems, 0);
            Popup_Rewards.AddResource(gems, 0);
        }
        else
        {
            if (UnityEngine.Random.value < .5f)
            {
                var coins = data.coinValues.RandomOne();
                Profile.EarnResouce(0, coins);
                Popup_Rewards.AddResource(0, coins);
            }
            else
            {
                var list = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId.Between(data.cardsGroups.x, data.cardsGroups.y));
                var racerid = list.Count > 0 ? list.RandomOne().Id : RewardLogic.SelectRacerReward();
                Profile.AddRacerCard(racerid, 1);
                Popup_Rewards.AddRacerCard(racerid, 1);
            }
        }

        Popup_Rewards.Display();
    }

    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime >= 0)
            timerText.SetFormatedText(remainTime / 3600, remainTime % 3600 / 60, remainTime % 60);
    }

    public override void SetActiveTimerObjects(bool active)
    {
        buyButton.gameObject.SetActive(active);
        getButton.gameObject.SetActive(!active);
        if (deactiveButtonGameObject)
            deactiveButtonGameObject.SetActive(active);
    }

    private void UpdateVisual()
    {
        if (dealyDesc)
            dealyDesc.SetFormatedText(data.nextTime > 3600 ? data.nextTime / 3600 : data.nextTime / 60);

        if (countDesc)
            countDesc.SetFormatedText(data.dailyCount);

        if (remainedLabel)
        {
            var ramained = Mathf.Max(0, data.dailyCount - Profile.UsedFreeItem);
            remainedLabel.SetFormatedText(ramained, data.dailyCount);
        }
    }
}
