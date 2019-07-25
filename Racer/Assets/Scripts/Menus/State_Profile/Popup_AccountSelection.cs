using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_AccountSelection : GameState
{
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button keepCurrentButton = null;
    [SerializeField] private Button fromServerButton = null;

    private System.Action<bool> callbackFunc = null;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public Popup_AccountSelection Setup(System.Action<bool> callback)
    {
        callbackFunc = callback;
        closeButton.onClick.AddListener(Back);
        keepCurrentButton.onClick.AddListener(Back);

        fromServerButton.onClick.AddListener(() =>
        {
            gameManager.OpenPopup<Popup_Confirm>().Setup(111062, true, yes =>
            {
                if (yes) callback(true);
            });
        });
        return this;
    }

    public override void Back()
    {
        gameManager.OpenPopup<Popup_Confirm>().Setup(111069, true, yes =>
        {
            if (yes)
            {
                base.Back();
                callbackFunc(false);
            }
        });
    }
}
