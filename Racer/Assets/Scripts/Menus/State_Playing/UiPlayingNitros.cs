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
    [SerializeField] private RectTransform nitrosBonus = null;

    private Color nitrosBarDefaultColor = Color.yellow;
    private float nitrosHintTimer = 0;
    private bool usingbonuse = false;

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
        nitrosBonus.gameObject.SetActive(false);
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

        RacerCamera.fovScale = PlayerPresenter.local.IsNitrosUsing ? 1.45f : 1;
        SeganX.Effects.CameraFX.MotionBlurValue = Mathf.Lerp(SeganX.Effects.CameraFX.MotionBlurValue, PlayerPresenter.local.IsNitrosUsing ? 0.6f : 0.3f, Time.deltaTime * 2);

        if (PlayerPresenter.local.IsNitrosUsing == false)
            nitrosBonus.gameObject.SetActive(usingbonuse = false);
    }

    private void HandleInput()
    {
        if (InputManager.Boost.isPointerDown || UiPlayingGesture.UseNitors
#if UNITY_EDITOR || UNITY_STANDALONE
                || Input.GetKeyDown(KeyCode.LeftControl)
#endif
        )
        {
            if (PlayerPresenter.local.IsNitrosReady)
                PlayerPresenter.local.UseNitrous();

            if (nitrosBonus.gameObject.activeSelf)
            {
                var nos = PlayerPresenter.local.player.CurrNitrous;
                var nosmin = nitrosBonus.anchoredPosition.x / nitrosBar.rectTransform.rect.width;
                var nosmax = (nitrosBonus.anchoredPosition.x + nitrosBonus.rect.width) / nitrosBar.rectTransform.rect.width;
                if (nosmin < nos && nos < nosmax)
                    PlayerPresenter.local.BoostNitros();
                nitrosBonus.gameObject.SetActive(false);
            }
            else if (usingbonuse == false)
            {
                usingbonuse = true;
                nitrosBonus.SetAnchordPositionX(Random.Range(70.0f, 270.0f));
                nitrosBonus.SetAnchordWidth(20 + Random.Range(0, 20) * 1000 / PlayerPresenter.local.player.RacerPower);
                nitrosBonus.gameObject.SetActive(true);
            }
        }
    }

    private void HandleNitorsHint()
    {
        if (RaceModel.IsTutorial == false && PlayerPresenter.local.IsNitrosFull)
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
