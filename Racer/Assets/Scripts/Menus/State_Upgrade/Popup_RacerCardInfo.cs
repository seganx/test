using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_RacerCardInfo : GameState
{
    [SerializeField] private UiRacerCard racerCardPrefab = null;
    [SerializeField] private Transform racerCardHolder = null;
    [SerializeField] private LocalText cardsCountLabel = null;
    [SerializeField] private LocalText neededCardsLabel = null;
    [SerializeField] private Button onlineButton = null;
    [SerializeField] private Button freeButton = null;
    [SerializeField] private Button shopButton = null;

    private void Start()
    {
        onlineButton.onClick.AddListener(() =>
        {
            Back();
            gameManager.OpenState<State_LeagueStart>();
        });

        freeButton.onClick.AddListener(() =>
        {
            Back();
            gameManager.OpenState<State_Shop>();
        });

        shopButton.onClick.AddListener(() =>
        {
            Back();
            gameManager.OpenState<State_Shop>();
        });

        var config = RacerFactory.Racer.GetConfig(GarageRacer.racer.Id);
        var racerprofile = Profile.GetRacer(config.Id);
        neededCardsLabel.SetFormatedText(Mathf.Clamp(config.CardCount - (racerprofile != null ? racerprofile.cards : 0), 0, config.CardCount));
        cardsCountLabel.SetFormatedText(racerprofile != null ? racerprofile.cards : 0, config.CardCount);
        UiShowHide.ShowAll(transform);
        racerCardPrefab.Clone<UiRacerCard>(racerCardHolder).Setup(config.Id);
    }
}
