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

    private PlayerPresenter presenter = null;
    private int position = 0;

    public UiRaceResultItem Setup(PlayerPresenter playerPresenter, int position, string nickName, string racerName, int racerPower, int score, int addscore)
    {
        presenter = playerPresenter;
        this.position = position;
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

    private IEnumerator Start()
    {
        // prepare chat list
        var chatlist = new List<int>();
        for (int i = 0; i < GlobalConfig.Chats.Count; i++)
        {
            if (GlobalConfig.Chats[i].IsPositionMatch(position))
                chatlist.Add(i);
        }

        yield return new WaitForSeconds(Random.Range(4f, 6f));
        while (true)
        {
            if (chatlist.Count > 1 &&
                RaceModel.IsOnline &&
                presenter != null &&
                presenter.player.LeagueRank < 3 &&
                presenter.IsSceneObject &&
                PlayNetwork.IsMaster &&
                presenter.GetComponent<BotPresenter>(true, true) != null)
            {
                if (Random.Range(0, 100) < GlobalConfig.Race.bots.chatLeaveChance)
                    presenter = null;

                if (presenter != null && Random.Range(0, 100) < GlobalConfig.Race.bots.chatChance)
                {
                    int chatIndex = chatlist.RandomOne();
                    chatlist.Remove(chatIndex);
                    presenter.SendChat(chatIndex);
                }
            }
            else presenter = null;

            nameLabel.color = presenter != null ? Color.white : Color.gray;
            yield return new WaitForSeconds(Random.Range(2.5f, 4.5f));
        }
    }
}
