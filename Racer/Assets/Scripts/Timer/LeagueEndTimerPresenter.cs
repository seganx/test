using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

public class LeagueEndTimerPresenter : TimerPresenter
{
    [SerializeField] private LocalText timerText = null;

    public override void Start()
    {
        base.Start();

        timerText.SetFormatedText(0, 0, 0);

        int remainTime = TimerManager.GetLeagueRemainTime();
        if (remainTime >= 0)
            timerText.SetFormatedText(remainTime / 86400, remainTime % 86400 / 3600, remainTime % 3600 / 60);
    }

    public override void SetActiveTimerObjects(bool active) { }

    public override void UpdateTimerText(int remainTime) { }
}