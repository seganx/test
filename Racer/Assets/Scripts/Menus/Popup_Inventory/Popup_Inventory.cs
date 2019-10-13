using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Inventory : GameState
{
    [SerializeField] private UiInventoryCardItem itemPrefab = null;
    [SerializeField] private LocalText bottomDesc = null;
    [SerializeField] private Button sellAllButton = null;

    private Transform container = null;
    private System.Action onNextTaskFunc = null;

    public Popup_Inventory Setup(System.Action onNextTask)
    {
        onNextTaskFunc = onNextTask;
        return this;
    }

    private IEnumerator Start()
    {
        UiInventoryCardItem.TotalCount = UiInventoryCardItem.TotalPrice = 0;

        foreach (var item in RacerFactory.Racer.AllConfigs)
        {
            var rp = Profile.GetRacer(item.Id);
            if (rp == null) continue;
            var count = rp.cards - item.CardCount;
            if (count < 1) continue;
            itemPrefab.Clone<UiInventoryCardItem>().Setup(item, count);
        }

        container = itemPrefab.transform.parent;
        itemPrefab.gameObject.DestroyNow();

        sellAllButton.onClick.AddListener(() =>
        {
            for (int i = 0; i < container.childCount; i++)
            {
                var item = container.GetChild<UiInventoryCardItem>(i);
                if (item != null) item.Sell();
            }
            Back();
        });

        UiShowHide.ShowAll(transform);

        var wait = new WaitForSeconds(0.2f);
        while (true)
        {
            bottomDesc.transform.parent.gameObject.SetActive(container.childCount > 1);
            bottomDesc.SetFormatedText(UiInventoryCardItem.TotalCount, UiInventoryCardItem.TotalPrice);
            yield return wait;
        }
    }

    public override void Back()
    {
        base.Back();
        ProfileLogic.SyncWidthServer(false, res => { });
        if (onNextTaskFunc != null) onNextTaskFunc();
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static int ComputeNumberOfCards()
    {
        int res = 0;
        foreach (var item in RacerFactory.Racer.AllConfigs)
        {
            var rp = Profile.GetRacer(item.Id);
            if (rp == null) continue;
            var count = rp.cards - item.CardCount;
            if (count < 1) continue;
            res += count;
        }
        return res;
    }
}
