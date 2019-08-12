using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingNitros : MonoBehaviour
{
    [SerializeField] private Image nitrosBar = null;
    [SerializeField] private Text nitrosChangeLabel = null;
    [SerializeField] private Color nitrosBarFullColor = Color.red;
    [SerializeField] private InputScreenButton nitrosButtons = null;
    [SerializeField] private AudioSource nosFullAudio = null;

    private Color nitrosBarDefaultColor = Color.yellow;
    private Color nitrosChangeLabelColor = Color.yellow;
    private float grading = 0;
    private float gradingLength = 1;
    private float lastNitros = 0;

    private bool NitrosButtonsActive
    {
        set
        {
            if (value && nitrosButtons.intractable != value)
                nosFullAudio.Play();

            nitrosButtons.intractable = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        nitrosBarDefaultColor = nitrosBar.color;
        nitrosChangeLabelColor = nitrosChangeLabel.color;
        nitrosBar.fillAmount = 0;
        NitrosButtonsActive = false;

        for (int i = 0; i < PlayerPresenter.allPlayers.Count; i++)
            PlayerPresenter.allPlayers[i].CurrNitrous = i / 4.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();

        if (PlayerPresenter.local.Grading > grading)
            gradingLength = PlayerPresenter.local.Grading - grading;
        grading = PlayerPresenter.local.Grading;

        nitrosBar.fillAmount = Mathf.MoveTowards(nitrosBar.fillAmount, grading > 0 ? (grading / gradingLength) : PlayerPresenter.local.Nitros, Time.deltaTime);
        nitrosBar.color = PlayerPresenter.local.NitrosReady ? nitrosBarFullColor : nitrosBarDefaultColor;
        nitrosChangeLabelColor.a = Mathf.MoveTowards(nitrosChangeLabelColor.a, 0, Time.deltaTime);
        nitrosChangeLabel.color = nitrosChangeLabelColor;
        NitrosButtonsActive = PlayerPresenter.local.NitrosReady;

        RacerCamera.fovScale = grading > 0 ? 1.45f : 1;
        SeganX.Effects.CameraFX.MotionBlurValue = Mathf.Lerp(SeganX.Effects.CameraFX.MotionBlurValue, grading > 0 ? 0.6f : 0.15f, Time.deltaTime * 2);

        if (lastNitros != PlayerPresenter.local.Nitros)
        {
            var nosdiff = (PlayerPresenter.local.Nitros - lastNitros) * 100;
            if (nosdiff > 0)
            {
                nitrosChangeLabel.SetText("+" + nosdiff.ToString("0.00"), false, LocalizationService.IsPersian);
                nitrosChangeLabelColor = Color.blue;
            }
            else if (nosdiff < 0)
            {
                nitrosChangeLabel.SetText(nosdiff.ToString("0.00"), false, LocalizationService.IsPersian);
                nitrosChangeLabelColor = Color.red;
            }
            lastNitros = PlayerPresenter.local.Nitros;
        }
    }

    private void HandleInput()
    {
        if (PlayerPresenter.local.NitrosReady == false) return;


        if (InputManager.Boost.isPointerDown || UiPlayingGesture.UseNitors
#if UNITY_EDITOR || UNITY_STANDALONE
                || Input.GetKeyDown(KeyCode.LeftShift)
#endif
        )
        {
            PlayerPresenter.local.UseNitrous();
            lastNitros = 0;
        }
    }
}
