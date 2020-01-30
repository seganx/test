using SeganX;
using SeganX.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Leaderboards : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private Image playerLeagueIcon = null;
    [SerializeField] private Toggle playerLeagueToggle = null;
    [SerializeField] private Toggle topLeagueToggle = null;
    [SerializeField] private UiLeaderboardItem prefabItem = null;

    private const int listUpdateDuration = 5;

    private void OnDestroy()
    {
        LastPosition = prefabItem.transform.parent.AsRectTransform().anchoredPosition.y;
        PlayerPrefs.SetInt("Leaderboard.TabIndex", playerLeagueToggle.isOn ? 0 : 1);
    }

    private void Start()
    {
        UiHeader.Show();
        title.SetFormatedText(listUpdateDuration);

        ValidateLists();

        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(33));

        playerLeagueIcon.sprite = GlobalFactory.League.GetBigIcon(Profile.League);
        prefabItem.transform.parent.SetChilderenActive(false);
        topLeagueToggle.isOn = false;
        playerLeagueToggle.isOn = false;

        topLeagueToggle.onValueChanged.AddListener(ison =>
        {
            if (ison == false) return;
            if (topList == null)
            {
                Popup_Loading.Display();
                Network.GetLeaderboard(true, (msg, res) =>
                {
                    if (msg == Network.Message.ok)
                        DisplayList(topList = res, true);
                    Popup_Loading.Hide();
                });
            }
            else DisplayList(topList, true);
        });

        playerLeagueToggle.onValueChanged.AddListener(ison =>
        {
            if (ison == false) return;

            if (playerList == null)
            {
                Popup_Loading.Display();
                Network.GetLeaderboard(false, (msg, res) =>
                {
                    if (msg == Network.Message.ok)
                        DisplayList(playerList = res, false);
                    Popup_Loading.Hide();
                });
            }
            else DisplayList(playerList, false);
        });

        var tabIndex = PlayerPrefs.GetInt("Leaderboard.TabIndex", 0);
        if (tabIndex == 0)
            playerLeagueToggle.isOn = true;
        else
            topLeagueToggle.isOn = true;

        UiShowHide.ShowAll(transform);
    }

    private void ValidateLists()
    {
        if (playerList == null || topList == null) return;
        if ((System.DateTime.Now - lastListUpdate).TotalMinutes < listUpdateDuration) return;
        lastListUpdate = System.DateTime.Now;
        playerList = null;
        topList = null;
    }

    private void DisplayList(List<LeaderboardProfileResponse> list, bool isTopList)
    {
        var content = prefabItem.transform.parent.RemoveChildren(3);
        content.GetChild(0).gameObject.SetActive(isTopList);
        content.GetChild(1).gameObject.SetActive(isTopList);

        foreach (var item in list)
            prefabItem.Clone<UiLeaderboardItem>().Setup(item.nickname, item.profileId, item.score, item.position).gameObject.SetActive(true);

        DelayCall(0.1f, () => prefabItem.transform.parent.SetAnchordPositionY(LastPosition));
    }


    ///////////////////////////////////////////////////////////////////////////////////
    //  STATIC MEMBERS
    ///////////////////////////////////////////////////////////////////////////////////
    private static List<LeaderboardProfileResponse> playerList = null;
    private static List<LeaderboardProfileResponse> topList = null;
    private static System.DateTime lastListUpdate = new System.DateTime(0);
    private static float LastPosition { get; set; }

}
