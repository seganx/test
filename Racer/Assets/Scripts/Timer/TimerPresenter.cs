using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public abstract class TimerPresenter : Base
{
    public TimerManager.Type timerType = TimerManager.Type.AdTimer;

    private int lastRemainTime;

    public virtual void Start()
    {
        lastRemainTime = 1;
        CheckRemainTime();
    }

    void CheckRemainTime()
    {
        int remainTime = TimerManager.GetRemainTime(timerType); // here, it isn't important that time is valid or invalid

        if (remainTime >= 0 || lastRemainTime < 0)
            SendRemainTimeThenCheckAgain(remainTime);
        else
            StartCoroutine(UpdateThenSendRemainTime());
    }

    IEnumerator UpdateThenSendRemainTime()
    {
        TimerManager.ValidateTime(); // call once
        while (!TimerManager.IsTimeValid)
            yield return new WaitForSeconds(1);

        int remainTime = TimerManager.GetRemainTime(timerType);

        SendRemainTimeThenCheckAgain(remainTime);
    }

    void SendRemainTimeThenCheckAgain(int remainTime)
    {
        UpdateGuiObjects(remainTime);
        lastRemainTime = remainTime;
        DelayCall(1, () =>
        {
            CheckRemainTime();
        });
    }

    public void StartTimer(int time)
    {
        TimerManager.SetTimer(timerType, time);
        lastRemainTime = time;
        UpdateGuiObjects(time);
    }

    public void SkipTimer()
    {
        TimerManager.SetTimer(timerType, -1);
        lastRemainTime = -1;
        UpdateGuiObjects(-1);
    }

    void UpdateGuiObjects(int remainTime)
    {
        //if (timerType == TimerManager.Type.LegendShopActivatorTimer)
        //   Debug.Log(remainTime);
        UpdateTimerText(remainTime);

        if (remainTime >= 0)
            SetActiveTimerObjects(true);
        else
            SetActiveTimerObjects(false);
    }


    public abstract void UpdateTimerText(int remainTime);
    public abstract void SetActiveTimerObjects(bool active);
}
