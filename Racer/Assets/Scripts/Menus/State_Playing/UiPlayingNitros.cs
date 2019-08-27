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
    private bool playNosAudio = false;


    private bool ActiveNitros
    {
        set
        {
            if (value && playNosAudio != value)
                nosFullAudio.Play();
            playNosAudio = value;

            nitrosButtons.intractable = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        nitrosBarDefaultColor = nitrosBar.color;
        nitrosBar.fillAmount = 0;
        nitrosBonus.gameObject.SetActive(false);
        ActiveNitros = false;
        boostCoods.z = nitrosBar.rectTransform.rect.width;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
        HandleNitorsHint();

        nitrosBar.fillAmount = PlayerPresenter.local.Nitros;
        nitrosBar.color = PlayerPresenter.local.IsNitrosFull ? nitrosBarFullColor : nitrosBarDefaultColor;
        ActiveNitros = PlayerPresenter.local.IsNitrosFull;

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
                Stat.SetLastNitroUseSlot(IsBoostInRange);
                if (IsBoostInRange)
                    PlayerPresenter.local.BoostNitros();

                nitrosBonus.gameObject.SetActive(false);
                boostCoods.x = -1;
            }
            else if (usingbonuse == false && (RaceModel.IsTutorial || Random.value < Stat.NitroUsePercentage))
            {
                usingbonuse = true;
                boostCoods.x = Random.Range(70.0f, 310 - GlobalConfig.Race.nosBonusWidth);
                boostCoods.y = GlobalConfig.Race.nosBonusWidth + Random.Range(0, 20);
                nitrosBonus.SetAnchordPositionX(boostCoods.x);
                nitrosBonus.SetAnchordWidth(boostCoods.y);
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


    private static class Stat
    {
        private static int nitroUseSaveSlotCount = 6;

        private static int LastNitroUseSaveSlotIndex
        {
            get { return PlayerPrefs.GetInt("UiPlayingNitros.LastNitroUseSaveSlotIndex"); }
            set { PlayerPrefs.SetInt("UiPlayingNitros.LastNitroUseSaveSlotIndex", value); }
        }

        public static float NitroUsePercentage
        {
            get
            {
                int totalNitroUse = 0;
                for (int i = 0; i < nitroUseSaveSlotCount; i++)
                    if (GetNitroUseSlot(i))
                        totalNitroUse++;

                return Mathf.Max(GlobalConfig.Race.nosBonusMinPercentage, totalNitroUse / (float)nitroUseSaveSlotCount);
            }
        }

        private static string GetNitroUseSlotString(int slotIndex)
        {
            return "UiPlayingNitros.NitroUseSaveSlot" + slotIndex.ToString();
        }

        public static void SetLastNitroUseSlot(bool use)
        {
            LastNitroUseSaveSlotIndex++;
            LastNitroUseSaveSlotIndex = LastNitroUseSaveSlotIndex % nitroUseSaveSlotCount;
            PlayerPrefs.SetInt(GetNitroUseSlotString(LastNitroUseSaveSlotIndex), use ? 1 : 0);
        }

        private static bool GetNitroUseSlot(int index)
        {
            return PlayerPrefs.GetInt(GetNitroUseSlotString(index)) == 1;
        }
    }
}
