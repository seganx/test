using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;


public class UiGoToRacePlayerInfo : MonoBehaviour
{
    [SerializeField] private Text playerName = null;
    [SerializeField] private Image racerIcon = null;
    [SerializeField] private Text racerName = null;
    [SerializeField] private LocalText racerPower = null;
    [SerializeField] private LocalText scoreValue = null;
    [SerializeField] private Image leagueTinyImage = null;
    [SerializeField] private Image leagueBigImage = null;

    public UiGoToRacePlayerInfo Setup(PlayerData data)
    {
        var rconfig = RacerFactory.Racer.GetConfig(data.RacerId);

        playerName.SetText(data.name);
        racerIcon.sprite = rconfig.icon;
        racerPower.SetFormatedText(data.RacerPower);
        if (racerName) racerName.text = rconfig.Name;

        var leagueIndex = GlobalConfig.Leagues.GetIndex(data.Score, data.Rank);
        if (leagueBigImage) leagueBigImage.sprite = GlobalFactory.League.GetBigIcon(leagueIndex);
        leagueTinyImage.sprite = GlobalFactory.League.GetSmallIcon(leagueIndex);
        scoreValue.SetText(data.Score.ToString());

        return this;
    }
}
