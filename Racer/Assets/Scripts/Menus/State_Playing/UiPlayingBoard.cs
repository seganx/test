using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingBoard : Base
{
    [System.Serializable]
    public class PlayerItem
    {
        public RectTransform transform = null;
        public Text nameLabel = null;
        public Image numberImage = null;
        private PlayerData data = null;

        public bool IsLeft { get; set; }
        public PlayerData Player { get { return data; } set { data = value; } }
        public bool IsPlayer { get { return data == null ? false : data.IsPlayer; } }
        public int Position { get { return data == null ? -100 : Mathf.RoundToInt(data.CurrPosition * 100); } }
    }

    [SerializeField] private LocalText positionLabel = null;
    [SerializeField] private Sprite[] positionSprites = null;
    [SerializeField] private List<PlayerItem> items = new List<PlayerItem>(4);

    private void Awake()
    {
        instance = this;

        foreach (var item in items)
            item.transform.gameObject.SetActive(false);

        foreach (var item in PlayerPresenter.all)
            AddPlayer(item.player);

        UpdatePositions();
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    private static UiPlayingBoard instance = null;

    public static void AddPlayer(PlayerData player)
    {
        if (instance == null) return;
        if (instance.items.Exists(x => x.Player == player)) return;
        var item = instance.items.Find(x => x.Player == null);
        if (item == null) return;

        item.Player = player;
        item.nameLabel.SetText(player.name, true);
        item.transform.gameObject.SetActive(true);
        UpdateItemColor(item);
        UpdatePositions();
    }

    public static void RemovePlayer(PlayerData player)
    {
        if (instance == null) return;
        var item = instance.items.Find(x => x.Player == player);
        if (item == null) return;
        item.IsLeft = true;
        UpdateItemColor(item);
        UpdatePositions();
    }

    public static void UpdatePositions()
    {
        if (instance == null) return;
        instance.items.Sort((x, y) => y.Position - x.Position);
        for (int i = 0; i < instance.items.Count; i++)
        {
            var item = instance.items[i];
            item.transform.SetAsLastSibling();
            item.numberImage.sprite = instance.positionSprites[i % instance.positionSprites.Length];
            UpdateItemColor(item);
            if (item.IsPlayer)
                instance.positionLabel.SetFormatedText(i + 1, RaceModel.specs.maxPlayerCount);
        }
    }

    private static void UpdateItemColor(PlayerItem item)
    {
        if (item.Player == null) return;
        if (item.Player.IsPlayer)
            item.nameLabel.color = Color.green;
        else
            item.nameLabel.color = item.IsLeft ? Color.gray : Color.white;
    }
}
