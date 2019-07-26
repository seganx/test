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
        UiShowHide.ShowAll(transform);
        nextButton.gameObject.SetActive(false);
        yield return new WaitWhile(() => CameraFX_Optimizer.IsRunning);
        nextButton.gameObject.SetActive(true);
        nextButton.onClick.AddListener(Back);
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

        var preset = GlobalConfig.ProfilePresets.RandomOne();
        Profile.EarnResouce(preset.gems, preset.coins);
        Popup_Rewards.AddResource(preset.gems, preset.coins);

        var racerconfig = RacerFactory.Racer.GetConfig(preset.racerId);
        Profile.AddRacerCard(preset.racerId, racerconfig.CardCount);
        Popup_Rewards.AddRacerCard(preset.racerId, racerconfig.CardCount);
        Profile.SelectedRacer = preset.racerId;

        for (int i = 0; i < preset.rndCards; i++)
        {
            var racerid = RewardLogic.SelectRacerReward();
            Profile.AddRacerCard(racerid, 1);
            Popup_Rewards.AddRacerCard(racerid, 1);
        }

        Popup_Rewards.Display(onNextTaskFunc);
    }
}
