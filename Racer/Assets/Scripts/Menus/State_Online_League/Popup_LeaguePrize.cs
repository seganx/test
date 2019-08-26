using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_LeaguePrize : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText topXLabel = null;
    [SerializeField] private LocalText topOneLabel = null;
    [SerializeField] private Image bigIcon = null;
    [SerializeField] private LocalText gemLabel = null;
    [SerializeField] private LocalText coinLabel = null;
    [SerializeField] private LocalText cardsLabel = null;
    [SerializeField] private LocalText cardsGroupLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Transform[] smallIcons = null;
    [SerializeField] private RectTransform selectedIcon = null;
    [SerializeField] private RectTransform playerPosition = null;

    // Use this for initialization
    private void Start()
    {
        Popup_Tutorial.Display(34);

        DisplayLeague(Profile.League);

        for (int i = 0; i < smallIcons.Length; i++)
        {
            int index = i;
            smallIcons[i].GetComponent<Button>().onClick.AddListener(() => DisplayLeague(index));
        }

        var pos = smallIcons[Profile.League].GetAnchordPositionInRect(selectedIcon.parent as RectTransform);
        playerPosition.SetAnchordPositionX(pos.x);

        UiShowHide.ShowAll(transform);
    }

    private void DisplayLeague(int index)
    {
        title.SetFormatedText(GlobalFactory.League.GetName(index));
        bigIcon.sprite = GlobalFactory.League.GetBigIcon(index);

        var league = GlobalConfig.Leagues.GetByIndex(index);
        scoreLabel.SetFormatedText(league.startScore);
        topXLabel.SetFormatedText(league.startRank);
        gemLabel.SetFormatedText(league.rewardGem);
        coinLabel.SetFormatedText(league.rewardCoin);
        cardsLabel.SetFormatedText(league.rewardCards);
        cardsGroupLabel.SetText(league.cardsGroups.x.ToString());
        //descLabel.SetText(LocalizationService.Get(111130 + index));
        descLabel.SetFormatedText(league.cardsGroups.x, league.rewardCards);

        scoreLabel.gameObject.SetActive(index > 0 && index < 5);
        topXLabel.gameObject.SetActive(index >= 5 && index < 10);
        topOneLabel.gameObject.SetActive(index == 10);

        for (int i = 0; i < smallIcons.Length; i++)
            smallIcons[i].GetChild(0).localScale = Vector3.one * (index == i ? 0.5f : 0.3f);

        var pos = smallIcons[index].GetAnchordPositionInRect(selectedIcon.parent as RectTransform);
        selectedIcon.GoToAnchordPosition(pos.x, selectedIcon.anchoredPosition.y, 0, 20);
    }
}
