using SeganX;
using SeganX.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Leaderboards : GameState
{
    [SerializeField] private Image playerLeagueIcon = null;
    [SerializeField] private Toggle playerLeagueToggle = null;
    [SerializeField] private Toggle topLeagueToggle = null;
    [SerializeField] private UiLeaderboardItem prefabItem = null;

    private static List<LeaderboardProfileResponse> playerList = null;
    private static List<LeaderboardProfileResponse> topList = null;

    private void Start()
    {
        UiHeader.Show();
        UiShowHide.ShowAll(transform);

        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(33));

        playerLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(Profile.League);

        prefabItem.gameObject.SetActive(false);
        topLeagueToggle.isOn = false;
        playerLeagueToggle.isOn = false;

        topLeagueToggle.onValueChanged.AddListener(ison =>
        {
            if (topList == null)
            {
                Popup_Loading.Display();
                Network.GetLeaderboard(true, (msg, res) =>
                {
                    if (msg == Network.Message.ok)
                        DisplayList(topList = res);
                    Popup_Loading.Hide();
                });
            }
            else DisplayList(topList);
        });

        playerLeagueToggle.onValueChanged.AddListener(ison =>
        {
            if (playerList == null)
            {
                Popup_Loading.Display();
                Network.GetLeaderboard(false, (msg, res) =>
                {
                    if (msg == Network.Message.ok)
                        DisplayList(playerList = res);
                    Popup_Loading.Hide();
                });
            }
            else DisplayList(playerList);
        });

        var tabIndex = PlayerPrefs.GetInt("Leaderboard.TabIndex", 0);
        if (tabIndex == 0)
            playerLeagueToggle.isOn = true;
        else
            topLeagueToggle.isOn = true;
    }

    private void DisplayList(List<LeaderboardProfileResponse> list)
    {
        prefabItem.transform.parent.RemoveChildrenBut(0);
        foreach (var item in list)
            prefabItem.Clone<UiLeaderboardItem>().Setup(item.nickname, item.profileId, item.score, item.position).gameObject.SetActive(true);
    }

    public override float PreClose()
    {
        var lastTime = PlayerPrefs.GetFloat("Leaderboard.UpdateTime", 0);
        if (System.DateTime.Now.Minute - lastTime > 15)
        {
            playerList = null;
            topList = null;
            PlayerPrefs.SetFloat("Leaderboard.UpdateTime", System.DateTime.Now.Minute);
        }
        PlayerPrefs.SetInt("Leaderboard.TabIndex", playerLeagueToggle.isOn ? 0 : 1);
        return base.PreClose();
    }
}
