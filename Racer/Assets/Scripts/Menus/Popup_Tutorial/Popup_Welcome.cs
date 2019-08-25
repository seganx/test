using SeganX;
using SeganX.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Welcome : GameState
{
    [SerializeField] private Button nextButton = null;

    private System.Action onNextTaskFunc = null;

    private IEnumerator Start()
    {
        Profile.IsFirstSession = true;
        RewardLogic.IsFirstRace = true;
        UiShowHide.ShowAll(transform);
        nextButton.gameObject.SetActive(false);
        yield return new WaitWhile(() => CameraFX_Optimizer.IsRunning);
        //nextButton.gameObject.SetActive(true);
        //nextButton.onClick.AddListener(Back);
        Back();
    }


    public Popup_Welcome SetCallback(System.Action onNextTask = null)
    {
        onNextTaskFunc = onNextTask;
        return this;
    }

    public override void Back()
    {
        base.Back();
        Profile.ResetData(2);
        if (onNextTaskFunc != null) onNextTaskFunc();
    }
}
