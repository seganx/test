using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_LeagueInfo : GameState
{
    [SerializeField] private GameObject panelScore = null;
    [SerializeField] private GameObject panelTop = null;
    [SerializeField] private LocalText[] leagueScore = null;

    private void Start()
    {
        panelScore.SetActive(Profile.League <= 4);
        panelTop.SetActive(Profile.League >= 5);
        for (int i = 0; i < 5; i++)
            leagueScore[i].SetText(GlobalConfig.Leagues.list[i].startScore.ToString("#,0"));

        UiShowHide.ShowAll(transform);
    }
}
