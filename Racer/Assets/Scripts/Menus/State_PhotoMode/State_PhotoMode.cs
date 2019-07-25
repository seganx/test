﻿using SeganX;
using SeganX.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_PhotoMode : GameState
{
    [SerializeField] private GameObject upPanel = null;
    [SerializeField] private GameObject effectPanel = null;
    [SerializeField] private Button prevEffectButton = null;
    [SerializeField] private Button nextEffectButton = null;
    [SerializeField] private LocalText numberLabel = null;
    [SerializeField] private Button screenshotButton = null;
    [SerializeField] private GameObject screenshotButtonLabel = null;
    [SerializeField] private GameObject screenshotButtonImage = null;

    private static bool RewardLabelVisible
    {
        get { return PlayerPrefs.GetInt("State_PhotoMode.RewardLabelVisible", 1) > 0; }
        set { PlayerPrefs.SetInt("State_PhotoMode.RewardLabelVisible", value ? 1 : 0); }
    }

    private void Start()
    {
        UiHeader.Hide();
        GarageCamera.SetCameraId(0);
        GarageRacer.SetRacerWheelsSpeed(0.02f);
        GarageRacer.SetRacerWheelsAngle(-30);
        UiShowHide.ShowAll(transform);

        if (CameraFX.Activated)
            SetLutTexture(0);
        else
            effectPanel.gameObject.SetActive(false);
        SetScreenshotButtonLabel(false);
        //SetScreenshotButtonLabel(RewardLabelVisible);

        prevEffectButton.onClick.AddListener(() => SetLutTexture(-1));
        nextEffectButton.onClick.AddListener(() => SetLutTexture(1));
        screenshotButton.onClick.AddListener(() =>
        {
            /*if (RewardLabelVisible)
            {
                gameManager.OpenPopup<Popup_Confirm>().Setup(111004, false, null);
                SetScreenshotButtonLabel(RewardLabelVisible = false);
            }
            else*/
            StartCoroutine(TakeScreenShot());
        });
    }

    private void SetScreenshotButtonLabel(bool visible)
    {
        screenshotButtonLabel.SetActive(visible);
        screenshotButtonImage.SetActive(!visible);
    }

    private void SetLutTexture(int step)
    {
        int index = CameraFX.LutIndex + step;
        CameraFX.LutIndex = index;
        prevEffectButton.SetInteractable(index > 0);
        nextEffectButton.SetInteractable(index < CameraFX_Luts.Count);
        numberLabel.SetFormatedText(CameraFX.LutIndex, CameraFX.LutName);
    }

    public IEnumerator TakeScreenShot()
    {
        upPanel.SetActive(false);
        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        var filename = System.IO.Path.Combine(Application.dataPath + "/../../Documents/Screenshots", "racer_" + System.DateTime.Now.Ticks + ".png");
#else
        var filename = System.IO.Path.Combine(Application.temporaryCachePath, "racer_" + System.DateTime.Now.Ticks + ".png");
#endif
        yield return new WaitForSeconds(1);
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(filename, 3);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2);
        upPanel.SetActive(true);

        try
        {
            SocialAndSharing.ShareTextAndImageFile(GlobalConfig.Socials.storeUrl, Application.productName, filename);
            //new NativeShare().AddFile(filename).SetSubject(Application.productName).SetText(GlobalConfig.Socials.storeUrl).Share();
        }
        catch { }

        // TODO: compelete for android platform
        try
        {
            Application.OpenURL(filename);
        }
        catch { }
    }

    public override void Back()
    {
        base.Back();
        GarageRacer.SetRacerWheelsSpeed(0);
    }
}
