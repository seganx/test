using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Loading : GameState
{
    [SerializeField] private Button forceUpdate = null;

    private void Start()
    {
        forceUpdate.onClick.AddListener(() =>
        {
            Application.OpenURL(GlobalConfig.Socials.storeUrl);
            Application.Quit();
        });

        UiShowHide.ShowAll(transform);
    }

    private void Update()
    {
        transform.SetActiveChild(GlobalConfig.ForceUpdate.whole ? 1 : 0);
    }

    public override void Back()
    {

    }

    ///////////////////////////////////////////////////////////////////////////////////
    //  STATIC MEMBERS
    ///////////////////////////////////////////////////////////////////////////////////
    private static Popup_Loading instance = null;
    private static int count = 0;

    public static void Display()
    {
        count++;
        if (instance != null) return;
        instance = gameManager.OpenPopup<Popup_Loading>();
    }

    public static void Hide()
    {
        if (instance == null) return;
        if (GlobalConfig.ForceUpdate.whole) return;
        count--;
        if (count > 0) return;
        gameManager.Back(instance);
        instance = null;
    }
}
