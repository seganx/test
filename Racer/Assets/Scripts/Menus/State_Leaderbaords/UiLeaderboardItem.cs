using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLeaderboardItem : MonoBehaviour
{
    [SerializeField] private Text nicknameLabel = null;
    [SerializeField] private Text userIdLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText positionLabel = null;
    [SerializeField] private Image leagueIcon = null;
    [SerializeField] private Button garageButton = null;

    public UiLeaderboardItem Setup(string nickname, string userid, int score, int position)
    {
        bool hasBadWord = BadWordsFinder.HasBadWord(nickname);
        if (hasBadWord) nickname = LocalizationService.Get(111147);

        nicknameLabel.SetText(nickname);
        userIdLabel.text = userid;
        scoreLabel.SetText(score.ToString("#,0"));
        positionLabel.SetText(position.ToString("#,0"));
        int league = GlobalConfig.Leagues.GetIndex(score, position);
        leagueIcon.sprite = GlobalFactory.League.GetSmallIcon(league);

        if (userid == Profile.UserId)
            GetComponent<Image>().color = Color.red;
        else
            garageButton.onClick.AddListener(() =>
            {
                if (hasBadWord)
                {
                    Game.Instance.OpenPopup<Popup_Confirm>().Setup(111148, true, false, null);
                }
                else
                {
                    Popup_Loading.Display();
                    Network.GetPlayerInfo(userid, pdata =>
                    {
                        Popup_Loading.Hide();

                        if (pdata != null)
                            Game.Instance.OpenState<State_OtherUserAccount>().Setup(pdata, nickname, score, position);
                        else
                        {
                            var warning = string.Format(LocalizationService.Get(111143), nickname);
                            Game.Instance.OpenPopup<Popup_Confirm>().Setup(warning, false, true, null);
                        }
                    });
                }
            });

        return this;
    }

}
