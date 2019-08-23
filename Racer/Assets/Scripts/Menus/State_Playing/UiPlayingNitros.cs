using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingNitros : MonoBehaviour
{
    [SerializeField] private Image nitrosBar = null;
    [SerializeField] private Color nitrosBarFullColor = Color.red;
    [SerializeField] private InputScreenButton nitrosButtons = null;
    [SerializeField] private AudioSource nosFullAudio = null;
    [SerializeField] private RectTransform nitrosHint = null;

    private Color nitrosBarDefaultColor = Color.yellow;
    private float nitrosHintTimer = 0;

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
        nitrosBar.fillAmount = 0;
        NitrosButtonsActive = false;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
        HandleNitorsHint();

        nitrosBar.fillAmount = PlayerPresenter.local.Nitros;
        nitrosBar.color = PlayerPresenter.local.IsNitrosFull ? nitrosBarFullColor : nitrosBarDefaultColor;
        NitrosButtonsActive = PlayerPresenter.local.IsNitrosReady;

        RacerCamera.fovScale = PlayerPresenter.local.IsNitrosUsing  ? 1.45f : 1;
        SeganX.Effects.CameraFX.MotionBlurValue = Mathf.Lerp(SeganX.Effects.CameraFX.MotionBlurValue, PlayerPresenter.local.IsNitrosUsing ? 0.6f : 0.15f, Time.deltaTime * 2);
    }

    private void HandleInput()
    {
        if (PlayerPresenter.local.IsNitrosReady == false) return;


        if (InputManager.Boost.isPointerDown || UiPlayingGesture.UseNitors
#if UNITY_EDITOR || UNITY_STANDALONE
                || Input.GetKeyDown(KeyCode.LeftControl)
#endif
        )
        {
            PlayerPresenter.local.UseNitrous();
        }
    }

    private void HandleNitorsHint()
    {
        if (PlayerPresenter.local.IsNitrosFull)
        {
            nitrosHintTimer += Time.deltaTime;
            if (nitrosHintTimer > 5)
            {
                nitrosHintTimer = 0;
                nitrosHint.SetAnchordPositionX(Random.Range(-200.0f, 200.0f));
                nitrosHint.gameObject.SetActive(true);
            }
        }
        else
        {
            nitrosHintTimer = 0;
            nitrosHint.gameObject.SetActive(false);
        }
    }
}
