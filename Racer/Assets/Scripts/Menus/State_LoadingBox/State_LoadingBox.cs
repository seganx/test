using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_LoadingBox : GameState
{
    [SerializeField] private Button helpButton = null;

    private void Start()
    {
        helpButton.onClick.AddListener(() =>
        {
            gameManager.OpenPopup<Popup_Confirm>().Setup(111123, true, null);
        });

        UiShowHide.ShowAll(transform);
    }
}
