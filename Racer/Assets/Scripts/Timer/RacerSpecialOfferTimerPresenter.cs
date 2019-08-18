using LocalPush;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacerSpecialOfferTimerPresenter : TimerPresenter
{
    [SerializeField] private LocalText timerText = null;
    [SerializeField] private Button specialOfferButton = null;
    [SerializeField] private Popup_ShopSpecialRacer popup = null;

    private int state = 0;

    private void Awake()
    {
        if (Profile.TotalRaces > 5)
        {
            timerText.SetFormatedText(0, 0, 0);

            ShopLogic.SpecialRacerPopup.Load();

            if (specialOfferButton)
            {
                specialOfferButton.gameObject.SetActive(false);
                specialOfferButton.onClick.AddListener(() => ShopLogic.SpecialRacerPopup.Display(() => specialOfferButton.gameObject.SetActive(false)));
            }
        }
        else Destroy(gameObject);
    }

    public override void UpdateTimerText(int remainTime)
    {
        if (remainTime >= 0)
        {
            timerText.SetFormatedText(remainTime / 3600, remainTime % 3600 / 60, remainTime % 60);

            if (state == 0)
            {
                state = 1;
                DisplayPackage();
            }
        }
        else
        {
            if (state != 2)
            {
                state = 2;
                RemovePackage();
            }

            if (remainTime < -GlobalConfig.Shop.specialRacerCardPopup.durationTime)
            {
                ShopLogic.SpecialRacerPopup.TryToCreateNewPackage();
                if (ShopLogic.SpecialRacerPopup.IsAvailable)
                    state = 0;
            }
        }
    }

    public void DisplayPackage()
    {
        if (specialOfferButton)
        {
            if (ShopLogic.SpecialRacerPopup.IsAvailable)
            {
                specialOfferButton.gameObject.SetActive(true);
                specialOfferButton.GetComponent<LocalText>(true, true).SetFormatedText(ShopLogic.SpecialRacerPopup.Package.discount);
                if (ShopLogic.SpecialRacerPopup.AutoDisplay) specialOfferButton.onClick.Invoke();
            }
        }
    }

    public void RemovePackage()
    {
        ShopLogic.SpecialRacerPopup.Clear();
        if (specialOfferButton)
            specialOfferButton.gameObject.SetActive(false);
        else if (popup)
            popup.Back();
        else
            Destroy(transform.gameObject);
    }

    public override void SetActiveTimerObjects(bool active) { }
}