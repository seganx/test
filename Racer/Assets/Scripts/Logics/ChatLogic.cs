using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatLogic : MonoBehaviour
{
    public class ChatData
    {
        public string name = string.Empty;
        public string chat = string.Empty;
    }

    public static Queue<ChatData> chats = new Queue<ChatData>(50);

    public static void Add(string playerName, int chatIndex)
    {
        chats.Enqueue(new ChatData()
        {
            name = playerName,
            chat = GlobalConfig.GetChat(chatIndex)
        });
    }

    public static ChatData Peek()
    {
        if (chats.Count > 0)
        {
            return chats.Dequeue();
        }
        else return null;
    }

    public static void Clear()
    {
        chats.Clear();
    }
}
