using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCustomeItem : Base
{
    private static Color gray = new Color(1, 1, 1, 0);

    public ScrollRect scroller = null;
    public Image background = null;
    public Image image = null;
    public LocalText caption = null;

    private int id = 0;

    public UiCustomeItem Setup(RacerCustomPresenter custome, string caption, bool selected, System.Action<RacerCustomPresenter> onSelect)
    {
        id = custome.Id;

        image.sprite = custome.icon;
        this.caption.SetText(caption);
        var toggle = GetComponent<Toggle>();
        toggle.isOn = false;

        toggle.onValueChanged.AddListener((ison) =>
        {
            SetCustomeSelected();
            onSelect(custome);
        });

        if (selected)
            DelayCall(0.01f, () => toggle.isOn = true);

        return this;
    }

    public UiCustomeItem Setup(Color color, int itemId, System.Action<int, Vector2> onSelect)
    {
        id = itemId;
        image.color = color;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            onSelect(id, rectTransform.anchoredPosition);
        });
        return this;
    }

    private void SetCustomeSelected()
    {
        scroller.movementType = ScrollRect.MovementType.Clamped;
        scroller.content.SetAnchordPositionX(scroller.viewport.rect.width * 0.5f - rectTransform.anchoredPosition.x);
        DelayCall(0.01f, () => scroller.movementType = ScrollRect.MovementType.Elastic);
    }

    private void SetColorSelected()
    {
        scroller.movementType = ScrollRect.MovementType.Clamped;
        scroller.content.SetAnchordPositionY(-scroller.viewport.rect.height * 0.5f - rectTransform.anchoredPosition.y);
        DelayCall(0.01f,()=> scroller.movementType = ScrollRect.MovementType.Elastic);
    }

    public UiCustomeItem SetIntractable(bool value)
    {
        background.color = value ? Color.white : gray;
        return this;
    }

    public static void SetDefaultColorSelected(Transform content, int id)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            var item = content.GetChild<UiCustomeItem>(i);
            if (item != null && item.id == id)
            {
                item.SetColorSelected();
                item.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}
