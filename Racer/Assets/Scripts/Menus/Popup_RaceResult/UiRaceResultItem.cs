using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRaceResultItem : MonoBehaviour
{
    [SerializeField] private LocalText positionLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Text racerLabel = null;
    [SerializeField] private LocalText powerLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText addScoreLabel = null;
    [SerializeField] private Image leagueIcon = null;

    public UiRaceResultItem Setup(int position, string nickName, string racerName, int racerPower, int score, int addscore)
    {
        positionLabel.SetText(position.ToString());
        nameLabel.SetText(nickName);
        racerLabel.text = racerName;
        powerLabel.SetText(racerPower.ToString("#,0"));
        scoreLabel.SetText(score.ToString());
        addScoreLabel.gameObject.SetActive(RaceModel.IsOnline);
        addScoreLabel.SetText((addscore > 0 ? "+" : string.Empty) + addscore);
        if (addscore < 0) addScoreLabel.target.color = Color.red;
        int league = GlobalConfig.Leagues.GetIndex(score, 0);
        leagueIcon.sprite = GlobalFactory.League.GetSmallIcon(league);
        return this;
    }
}
