using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_TimerSkip : GameState
{
    [SerializeField] private LocalText timerText;
    [SerializeField] private LocalText rechargCostText;
    [SerializeField] private Button cancelButton = null;
    [SerializeField] private Button buyButton = null;
    private int price;

    public Popup_TimerSkip Setup(System.Action onBuy)
    {
        buyButton.SetInteractable(false);
        cancelButton.onClick.AddListener(() => base.Back());
        buyButton.onClick.AddListener(() =>
        {
            Game.SpendGem(price, () =>
            {
                onBuy();
                Back();
            });
        });
        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public void UpdateTimerText(int remainTime)
    {
        buyButton.SetInteractable(true);
        price = Mathf.CeilToInt(remainTime / (float)GlobalConfig.Shop.gemToTime);
        rechargCostText.SetFormatedText(price);
        timerText.SetFormatedText(remainTime / 60, remainTime % 60);
    }
}
