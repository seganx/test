using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class CombiedShopItemTimerPresenter : TimerPresenter
{
    [SerializeField] private LocalText timerText;

    public override void Start()
    {
        base.Start();
        timerText.SetFormatedText(0, 0, 0);
    }

    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime > 0)
            timerText.SetFormatedText(remainTime / 3600, remainTime % 3600 / 60, remainTime % 60);
        else
            Destroy(transform.parent.gameObject);
    }

    public override void SetActiveTimerObjects(bool active)
    {
    }
}