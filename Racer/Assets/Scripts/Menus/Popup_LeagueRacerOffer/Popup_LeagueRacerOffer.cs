using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class Popup_LeagueRacerOffer : GameState
{
    //[SerializeField] LocalText titleLabel = null;
    [SerializeField] LocalText offerDescText = null;
    [SerializeField] Button offerButton = null;
    [SerializeField] LocalText offerButtonText = null;
    [SerializeField] Button resumeButton = null;
    [SerializeField] Button backButton = null;

    public Popup_LeagueRacerOffer Setup(int leagueGroupeId, System.Action<bool> onFinished)
    {
        //titleLabel.SetText(config.title);
        offerDescText.SetFormatedText(leagueGroupeId);
        offerButtonText.SetFormatedText(leagueGroupeId);

        offerButton.onClick.AddListener(() =>
        {
            base.Back();
            if (onFinished != null)
                onFinished(false);
        });

        resumeButton.onClick.AddListener(() =>
        {
            base.Back();
            if (onFinished != null)
                onFinished(true);
        });

        backButton.onClick.AddListener(() =>
        {
            Back();
        });

        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }
}