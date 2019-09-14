using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public class UiRaceResultChatItem : MonoBehaviour
{
    [SerializeField] private LocalText nameLabel = null;
    [SerializeField] private LocalText chatLabel = null;

    public UiRaceResultChatItem Setup(string userName, string chat)
    {
        nameLabel.SetText(userName);
        chatLabel.SetText(chat);
        return this;
    }
}
