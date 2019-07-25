using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using SeganX;

public class UiLeaderboardItem : MonoBehaviour
{
    [SerializeField] private Text nicknameLabel = null;
    [SerializeField] private Text userIdLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText positionLabel = null;
    [SerializeField] private Image leagueIcon = null;

    public UiLeaderboardItem Setup(string nickname, string userid, int score, int position)
    {
        nicknameLabel.SetText(nickname);
        userIdLabel.text = userid;
        scoreLabel.SetText(score.ToString("#,0"));
        positionLabel.SetText(position.ToString("#,0"));
        int league = GlobalConfig.Leagues.GetIndex(score, position);
        leagueIcon.sprite = GlobalFactory.League.GetSmallIcon(league);

        if (userid == Profile.UserId)
            GetComponent<Image>().color = Color.red;

        return this;
    }

}
