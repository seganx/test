using SeganX;
using SeganX.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Welcome : GameState
{
    [SerializeField] private Text infoLabel = null;
    [SerializeField] private Button nextButton = null;

    private System.Action onNextTaskFunc = null;

    private IEnumerator Start()
    {
        Profile.IsFirstSession = true;
        RewardLogic.IsFirstRace = true;
        UiShowHide.ShowAll(transform);
        nextButton.gameObject.SetActive(false);

        while (CameraFX_Optimizer.IsRunning)
        {
            infoLabel.text = "Step: " + CameraFX_Optimizer.Step + " FPS:" + CameraFX_Optimizer.Fps + " Resolution:" + CameraFX.Resolution + " Quality :" + CameraFX.Quality + " Bloom :" + CameraFX.Bloom;
            yield return new WaitForSeconds(0.5f);
        }

        //yield return new WaitWhile(() => CameraFX_Optimizer.IsRunning);
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
