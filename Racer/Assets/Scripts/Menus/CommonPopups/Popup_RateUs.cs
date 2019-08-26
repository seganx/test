using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_RateUs : GameState
{
    [SerializeField] private Button[] stars = null;
    [SerializeField] private Button sendButton = null;

    private void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            int index = i;
            stars[i].onClick.AddListener(() => OnStarButton(index));
        }
        stars[0].onClick.Invoke();

        sendButton.SetInteractable(false);
        sendButton.onClick.AddListener(OnSendButton);

        UiShowHide.ShowAll(transform);
    }

    public override void Back()
    {
        base.Back();
        PlayerInjoyed = 0;
        RateUsValue = 0;
    }

    private void OnStarButton(int index)
    {
        RateUsValue = index + 1;

        for (int i = 0; i < stars.Length; i++)
            stars[i].transform.GetChild(0).gameObject.SetActive(i <= index);

        sendButton.SetInteractable(true);
    }

    private void OnSendButton()
    {
        base.Back();
        if (RateUsValue > 4)
        {
            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111118, true, yes =>
            {
                if (yes) SocialAndSharing.RateUs(null, GlobalConfig.Socials.rateUrl);
            });
        }
        else Game.Instance.OpenPopup<Popup_Confirm>().Setup(111119, false, null);
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    private static int PlayerInjoyed
    {
        get { return PlayerPrefsEx.GetInt("Popup_RateUs.PlayerInjoyed", 0); }
        set { PlayerPrefsEx.SetInt("Popup_RateUs.PlayerInjoyed", value); }
    }

    private static int RateUsValue
    {
        get { return PlayerPrefsEx.GetInt("Popup_RateUs.RateUsValue", 0); }
        set { PlayerPrefsEx.SetInt("Popup_RateUs.RateUsValue", value); }
    }

    public static void SetPlayerInjoy(bool playerInjoyed)
    {
        if (playerInjoyed)
            PlayerInjoyed++;
        else
            PlayerInjoyed = 0;
    }

    public static void CheckAndDisplay()
    {
        if (Profile.TotalRaces < 6) return;
        if (RateUsValue > 0 || PlayerInjoyed < 2) return;
        PopupQueue.Add(0, () => gameManager.OpenPopup<Popup_RateUs>());
    }

    [Console("test", "rateus")]
    public static void Test()
    {
        RateUsValue = 0;
        PlayerInjoyed = 2;
        CheckAndDisplay();
    }
}
