using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Game : GameManager<Game>
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 25;
    }

    private IEnumerator Start()
    {
        GameMap.Load(0);
        Popup_Loading.Display();
        yield return new WaitForSeconds(1);
        PurchaseSystem.Initialize(GlobalConfig.Instance.cafeBazaarKey, GlobalConfig.Socials.storeUrl, (success, msg) => Debug.Log("Purchase system initialized: " + success + " " + msg));

#if DATABEEN
        OnOpenState += gamestate => { if (gamestate != null) DataBeen.SendContentView(gamestate.name, "ok"); };
#endif

        //  first try to connect to internet
        Http.requestTimeout = GlobalConfig.Server.requestTimeout / 3;
        ProfileLogic.SyncWidthServer(false, success =>
        {
            Http.requestTimeout = GlobalConfig.Server.requestTimeout;
            Popup_Loading.Hide();
            OpenState<State_Home>();
        });

        GarageRacerImager.LoadCache();
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static void LoadMap(int id)
    {
        GameMap.Load(id);

        if (id == 0)
            UiHeader.Show();
        else
            UiHeader.Destroy();
    }


    public static void SpendGem(int value, Action onSuccess)
    {
        if (Profile.SpendGem(value))
            onSuccess();
        else
            Instance.OpenPopup<Popup_Shop>().SetupAsGems(() =>
            {
                //if (Profile.SpendGem(value))
                //    onSuccess();
            });
    }

    public static void SpendCoin(int value, Action onSuccess)
    {
        if (Profile.SpendCoin(value))
            onSuccess();
        else
            Instance.OpenPopup<Popup_Shop>().SetupAsCoins(() =>
            {
                //if (Profile.SpendCoin(value))
                //    onSuccess();
            });
    }
}
