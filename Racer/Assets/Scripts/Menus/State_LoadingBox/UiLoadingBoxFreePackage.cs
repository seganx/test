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

    private GlobalConfig.Data.Shop.LoadingBox data = null;

    public override void Start()
    {
        data = index < GlobalConfig.Shop.loadingBoxPackage.Count ? GlobalConfig.Shop.loadingBoxPackage[index] : null;
        if (data != null)
        {
            if (IsSameDay == false)
            {
                IsSameDay = true;
                UseCount = 0;
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
            UseCount++;

            if (data.dailyCount > UseCount)
            {
                if (State_Settings.IsFreePackageNotificationActive)
                    NotificationManager.SendWithAppIcon(GlobalConfig.Shop.loadingBoxPackage[index].nextTime, NotificationType.FreePackage);
                StartTimer(GlobalConfig.Shop.loadingBoxPackage[index].nextTime);

            }
            else
            {
                DateTime now = TimerManager.ServerTime;
                int hours = 0, minutes = 0, seconds = 0, newTime = 0;
                hours = (24 - now.Hour) - 1;
                minutes = (60 - now.Minute) - 1;
                seconds = (60 - now.Second - 1);

                newTime = seconds + (minutes * 60) + (hours * 3600);

                if (State_Settings.IsFreePackageNotificationActive)
                    NotificationManager.SendWithAppIcon(newTime, NotificationType.FreePackage);
                StartTimer(newTime);
            }




            UpdateVisual();

            switch (UnityEngine.Random.Range(0, 100) % 4)
            {
                case 0:
                    {
                        var gems = data.gemValues.RandomOne();
                        Profile.EarnResouce(gems, 0);
                        Popup_Rewards.AddResource(gems, 0);
                    }
                    break;

                case 1:
                    {
                        var coins = data.coinValues.RandomOne();
                        Profile.EarnResouce(0, coins);
                        Popup_Rewards.AddResource(0, coins);
                    }
                    break;

                case 2:
                    {
                        var list = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId.Between(data.cardsGroups.x, data.cardsGroups.y));
                        var racerid = list.Count > 0 ? list.RandomOne().Id : RewardLogic.SelectRacerReward();
                        Profile.AddRacerCard(racerid, 1);
                        Popup_Rewards.AddRacerCard(racerid, 1);
                    }
                    break;

                case 3:
                    {
                        var reward = RewardLogic.GetCustomReward();
                        Profile.AddRacerCustome(reward.type, reward.racerId, reward.customId);
                        Popup_Rewards.AddCustomeCard(reward.type, reward.racerId, reward.customId);
                    }
                    break;
            }

            Popup_Rewards.Display();
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


    private bool IsSameDay
    {
        get { return PlayerPrefsEx.GetInt(name + ".day", 0) == TimerManager.ServerTime.DayOfYear; }
        set { if (value) PlayerPrefsEx.SetInt(name + ".day", TimerManager.ServerTime.DayOfYear); }
    }

    private int UseCount
    {
        get { return PlayerPrefsEx.GetInt(name + ".used", 0); }
        set { PlayerPrefsEx.SetInt(name + ".used", value); }
    }

    private void UpdateVisual()
    {
        if (dealyDesc)
            dealyDesc.SetFormatedText(data.nextTime > 3600 ? data.nextTime / 3600 : data.nextTime / 60);

        if (countDesc)
            countDesc.SetFormatedText(data.dailyCount);

        if (remainedLabel)
        {
            var ramained = Mathf.Max(0, data.dailyCount - UseCount);
            remainedLabel.SetFormatedText(ramained, data.dailyCount);
        }
    }
}
