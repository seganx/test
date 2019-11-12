using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_SteeringMode : GameState
{
    [SerializeField] private RectTransform selected = null;
    [SerializeField] private Button defaultModeButton = null;
    [SerializeField] private Button runnerModeButton = null;
    [SerializeField] private Button tiltModeButton = null;

    private System.Action onCloseFunc = null;

    public Popup_SteeringMode Setup(System.Action onClose)
    {
        onCloseFunc = onClose;
        return this;
    }

    private void Start()
    {
        if (SystemInfo.supportsAccelerometer == false)
        {
            tiltModeButton.GetComponent<Outline>().enabled = false;
            tiltModeButton.SetInteractable(SystemInfo.supportsAccelerometer);
        }

        switch (Settings.SteeringMode)
        {
            case RaceModel.SteeringMode.Normal: selected.SetAnchordPositionX(defaultModeButton.transform.GetAnchordPosition().x); break;
            case RaceModel.SteeringMode.Easy: selected.SetAnchordPositionX(runnerModeButton.transform.GetAnchordPosition().x); break;
            case RaceModel.SteeringMode.Tilt: selected.SetAnchordPositionX(tiltModeButton.transform.GetAnchordPosition().x); break;
        }

        defaultModeButton.onClick.AddListener(() =>
        {
            RaceModel.specs.steering = Settings.SteeringMode = RaceModel.SteeringMode.Normal;
            Back();
        });

        runnerModeButton.onClick.AddListener(() =>
        {
            RaceModel.specs.steering = Settings.SteeringMode = RaceModel.SteeringMode.Easy;
            Back();
        });

        tiltModeButton.onClick.AddListener(() =>
        {
            RaceModel.specs.steering = Settings.SteeringMode = RaceModel.SteeringMode.Tilt;
            Back();
        });

        UiShowHide.ShowAll(transform);
    }

    public override void Back()
    {
        base.Back();
        onCloseFunc();
    }
}
