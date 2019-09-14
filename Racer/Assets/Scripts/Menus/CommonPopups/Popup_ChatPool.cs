using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ChatPool : GameState
{
    [SerializeField] private Button itemPrefab = null;

    private System.Action<int> onClickFunc = null;

    public Popup_ChatPool Setup(System.Action<int> onClick)
    {
        onClickFunc = onClick;
        return this;
    }

    private void Start()
    {
        for (int i = 0; i < GlobalConfig.Chats.Length; i++)
        {
            int index = i;
            var obj = itemPrefab.Clone<Button>();
            obj.GetComponentInChildren<LocalText>().SetText(GlobalConfig.Chats[i]);
            obj.onClick.AddListener(() =>
            {
                Back();
                onClickFunc(index);
            });
        }
        Destroy(itemPrefab.gameObject);

        UiShowHide.ShowAll(transform);
    }
}
