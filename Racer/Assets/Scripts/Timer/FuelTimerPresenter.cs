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
                NotificationManager.Cancel(NotificationType.FullFuel);
                SkipTimer();
            });
        });

        UpdateFuelCountText();

        if (FuelCount > 0)
            SetActiveTimerObjects(false);
    }

    public static void ReduceFuel()
    {
        FuelCount--;
        if (FuelCount <= 0)
        {
            if (State_Settings.IsFullFuelActiveNotificationActive)
                NotificationManager.SendWithAppIcon(GlobalConfig.Recharg.time, NotificationType.FullFuel);
            TimerManager.SetTimer(TimerManager.Type.FullFuelTimer, GlobalConfig.Recharg.time);
            //StartTimer(GlobalConfig.Recharg.time);
        }
    }

    void UpdateFuelCountText()
    {
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
            if (FuelCount <= 0)
            {
                FuelCount = GlobalConfig.Recharg.count;
                UpdateFuelCountText();

                if (popup_TimerSkip)
                    popup_TimerSkip.Back();
                // if Popup_TimerSkip is open, close it
            }
        }
    }

    public override void SetActiveTimerObjects(bool active)
    {
        if (FuelCount > 0)
        {
            startButton.gameObject.SetActive(true);
            timerSkipButton.gameObject.SetActive(false);
            timerGameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(!active);
            timerSkipButton.gameObject.SetActive(active);
            timerGameObject.SetActive(active);
        }
    }
}