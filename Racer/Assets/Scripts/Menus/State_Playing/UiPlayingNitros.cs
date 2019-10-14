using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingNitros : MonoBehaviour
{
    [SerializeField] private Image nitrosBar = null;
    [SerializeField] private Color nitrosBarFullColor = Color.red;
    [SerializeField] private AudioSource nosFullAudio = null;
    [SerializeField] private RectTransform nitrosBonus = null;
    [SerializeField] private GameObject nitrosBonusButtons = null;
    [SerializeField] private Image[] nitrosButtonImage = null;
    [SerializeField] private Animation[] nitrosButtonAnimation = null;

    private Color nitrosBarDefaultColor = Color.yellow;
    private float nitrosHintTimer = 0;
    private bool playNosAudio = false;


    private bool ActiveNosBoost
    {
        get { return nitrosBonus.gameObject.activeSelf; }
        set
        {
            nitrosBonus.gameObject.SetActive(value);
            if (value == false) boostCoods.x = -1;
        }
    }

    private bool ActiveSound
    {
        set
        {
            if (value && playNosAudio != value)
                nosFullAudio.Play();
            playNosAudio = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        nitrosBarDefaultColor = nitrosBar.color;
        nitrosBar.fillAmount = 0;
        ActiveSound = false;
        ActiveNosBoost = false;
        boostCoods.z = nitrosBar.rectTransform.rect.width;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
        HandleNitorsHint();

        nitrosBar.fillAmount = PlayerPresenter.local.Nitros;
        nitrosBar.color = PlayerPresenter.local.IsNitrosFull ? nitrosBarFullColor : nitrosBarDefaultColor;

        RacerCamera.fovScale = PlayerPresenter.local.IsNitrosUsing ? 1.45f : 1;
        SeganX.Effects.CameraFX.MotionBlurValue = Mathf.Lerp(SeganX.Effects.CameraFX.MotionBlurValue, PlayerPresenter.local.IsNitrosUsing ? 0.4f : 0.1f, Time.deltaTime * 2);

        ActiveSound = PlayerPresenter.local.IsNitrosFull;
        if (PlayerPresenter.local.IsNitrosUsing == false)
            ActiveNosBoost = false;

        for (int i = 0; i < nitrosButtonImage.Length; i++)
        {
            nitrosButtonImage[i].fillAmount = PlayerPresenter.local.Nitros;
            nitrosButtonImage[i].gameObject.SetActive(PlayerPresenter.local.IsNitrosReady && ActiveNosBoost == false);
        }
        nitrosBonusButtons.SetActive(IsBoostInRange);
    }

    public static bool checkme = false;

    private void HandleInput()
    {
        if ((ActiveNosBoost && PlayerPresenter.local.IsNitrosUsing) &&
            (InputManager.Boost.isPointerClick || UiPlayingGesture.UseNitors
#if UNITY_EDITOR || UNITY_STANDALONE
                || Input.GetKeyDown(KeyCode.LeftControl)
#endif
        ))
        {
            if (IsBoostInRange)
            {
                PlayerPresenter.local.BoostNitros();
                nosFullAudio.Play();
            }
            ActiveNosBoost = false;
            boostCoods.x = -1;
        }
        else if (checkme)
        {
            Debug.LogError(ActiveNosBoost + "&&" + PlayerPresenter.local.IsNitrosUsing + "&&" + InputManager.Boost.isPointerClick);
        }

        if ((PlayerPresenter.local.IsNitrosReady && PlayerPresenter.local.IsNitrosUsing == false) &&
            (InputManager.Boost.isPointerClick || UiPlayingGesture.UseNitors
#if UNITY_EDITOR || UNITY_STANDALONE
                || Input.GetKeyDown(KeyCode.LeftControl)
#endif
        ))
        {
            PlayerPresenter.local.UseNitrous();

            if (RaceModel.IsTutorial || Random.Range(0, 100) < GlobalConfig.Race.config.nosBonusChance)
            {
                boostCoods.x = Random.Range(70.0f, 310 - GlobalConfig.Race.config.nosBonusWidth);
                boostCoods.y = GlobalConfig.Race.config.nosBonusWidth + Random.Range(0, 20);
                nitrosBonus.SetAnchordPositionX(boostCoods.x);
                nitrosBonus.SetAnchordWidth(boostCoods.y);
                ActiveNosBoost = true;
            }
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
                for (int i = 0; i < nitrosButtonAnimation.Length; i++)
                    nitrosButtonAnimation[i].Play();
            }
        }
        else
        {
            nitrosHintTimer = 0;
        }
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static Vector3 boostCoods = -Vector3.one;

    public static bool IsBoostInRange
    {
        get
        {
            if (boostCoods.x < 0) return false;
            var nos = PlayerPresenter.local.player.CurrNitrous;
            var nosmin = boostCoods.x / boostCoods.z;
            var nosmax = (boostCoods.x + boostCoods.y) / boostCoods.z;
            return nosmin < nos && nos < nosmax;
        }
    }
}
