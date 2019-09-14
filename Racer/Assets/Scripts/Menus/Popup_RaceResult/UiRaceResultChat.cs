using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRaceResultChat : Base
{
    [SerializeField] private UiRaceResultChatItem itemPrefab = null;
    [SerializeField] private Button sendButton = null;

    private void Awake()
    {

        if (RaceModel.IsOnline)
        {
            itemPrefab.gameObject.SetActive(false);

            sendButton.onClick.AddListener(() =>
            {
                Game.Instance.OpenPopup<Popup_ChatPool>().Setup(index => PlayerPresenter.local.SendChat(index));
                sendButton.SetInteractable(false);
                DelayCall(2, () => sendButton.SetInteractable(true));
            });

        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        var item = ChatLogic.Peek();
        if (item == null) return;
        itemPrefab.Clone<UiRaceResultChatItem>().Setup(item.name, item.chat).gameObject.SetActive(true);
    }
}
