using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class State_UnlockRacer : GameState
{
    [SerializeField] private GameObject introWindow = null;
    [SerializeField] private Text[] racerNameLabel = null;
    [SerializeField] private LocalText racerSpeedLabel = null;
    [SerializeField] private LocalText racerNitrosLabel = null;
    [SerializeField] private LocalText racerSteeringLabel = null;
    [SerializeField] private LocalText racerBodyLabel = null;
    [SerializeField] private LocalText racerPowerLabel = null;

    private static int racerId = 0;

    // Use this for initialization
    public State_UnlockRacer Setup(int unlockedRacerId)
    {
        racerId = unlockedRacerId;
        return this;
    }

    IEnumerator Start()
    {
        UiHeader.Hide();
        GarageCamera.SetCameraId(7);

        var config = RacerFactory.Racer.GetConfig(racerId);

        foreach (var item in racerNameLabel)
            item.text = config.Name;

        racerSpeedLabel.SetFormatedText(config.ComputeSpeed(0));
        racerNitrosLabel.SetFormatedText(config.ComputeNitro(0));
        racerSteeringLabel.SetFormatedText(config.ComputeSteering(0));
        racerBodyLabel.SetFormatedText(config.ComputeBody(0));
        racerPowerLabel.SetFormatedText(config.MinPower);

        UiShowHide.ShowAll(transform);

        yield return new WaitForSeconds(3);
        Destroy(introWindow);
    }
}
