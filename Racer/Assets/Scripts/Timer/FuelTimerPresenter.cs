using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;
using LocalPush;

public class FuelTimerPresenter : TimerPresenter
{
    [SerializeField] private Button startButton = null;
    [SerializeField] private Button timerSkipButton = null;
    [SerializeField] private LocalText fuelCountText;
    [SerializeField] private GameObject timerGameObject;
    [SerializeField] private LocalText timerText;

    private Popup_TimerSkip popup_TimerSkip;

    public override void Start()
    {
        base.Start();
        timerText.SetFormatedText(0, 0);

        startButton.onClick.AddListener(() =>
        {
            //UpdateFuelCountText();
        });

        timerSkipButton.onClick.AddListener(() =>
        {
            popup_TimerSkip = gameManager.OpenPopup<Popup_TimerSkip>().Setup(() =>
            {
                FuelCount = GlobalConfig.Recharg.count;
                UpdateFuelCountText();
                UpdateFuelNotification();
            });
        });

        if (TimerManager.GetRemainTime(TimerManager.Type.FullFuelTimer) > GlobalConfig.Recharg.time)
            TimerManager.SetTimer(TimerManager.Type.FullFuelTimer, GlobalConfig.Recharg.time);

        UpdateFuelCountText();

        if (FuelCount > 0)
            SetActiveTimerObjects(false);
    }

    public static void ReduceFuel()
    {
        FuelCount--;
        UpdateFuelNotification();
    }

    void UpdateFuelCountText()
    {
        timerGameObject.SetActive(FuelCount < GlobalConfig.Recharg.count);
        fuelCountText.SetFormatedText(FuelCount, GlobalConfig.Recharg.count);
    }

    public static int FuelCount
    {
        get { return PlayerPrefsEx.GetInt("fuelCount", GlobalConfig.Recharg.count); }
        set { PlayerPrefsEx.SetInt("fuelCount", value); }
    }

    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime >= 0)
        {
            timerText.SetFormatedText(remainTime / 60, remainTime % 60);

            if (popup_TimerSkip)
                popup_TimerSkip.UpdateTimerText(remainTime);
        }
        else
        {
            /*
            if (FuelCount <= 0)
            {
                if (popup_TimerSkip)
                    popup_TimerSkip.Back();
                // if Popup_TimerSkip is open, close it
            }*/

            FuelCount++;
            FuelCount += -(remainTime / GlobalConfig.Recharg.time);
            FuelCount = Mathf.Min(FuelCount, GlobalConfig.Recharg.count);
            int newTime = GlobalConfig.Recharg.time + remainTime % GlobalConfig.Recharg.time;
            StartTimer(newTime);
            UpdateFuelCountText();
            UpdateFuelNotification();
        }
    }

    public override void SetActiveTimerObjects(bool active)
    {
        if (FuelCount > 0)
        {
            startButton.gameObject.SetActive(true);
            timerSkipButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(!active);
            timerSkipButton.gameObject.SetActive(active);
        }
    }

    static void UpdateFuelNotification()
    {
        if (State_Settings.IsFullFuelActiveNotificationActive && FuelCount < GlobalConfig.Recharg.count / 2)
            NotificationManager.SendWithAppIcon(GlobalConfig.Recharg.time * (GlobalConfig.Recharg.count - FuelCount), NotificationType.FullFuel);
        else
            NotificationManager.Cancel(NotificationType.FullFuel);
    }
}