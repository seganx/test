using UnityEngine;
using System.Collections;
using System;
using DataBeenConnection;
using System.Net;

public class DataBeen : MonoBehaviour {

    private static DataBeen instance;
    public string apiKey;
    public MarketType target;
    public string customTarget;
    private static bool autoSave;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        Init();
    }




    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        WebConnection.Init();
        PacketsIdentity.OnInitialized += OnInited;
        PacketsIdentity.Init();
        StartCoroutine(_StartSaving());
        //ReleaseLocationCatcher();
    }

    public void ReleaseLocationCatcher()
    {
        LocationCatcher lc = new GameObject("LocationCatcher").AddComponent<LocationCatcher>();
        DontDestroyOnLoad(lc);
        lc.gameObject.hideFlags = HideFlags.HideInHierarchy;
        lc.SetMethod(LocationGeted);
    }

    private void LocationGeted(string lc)
    {
        PacketsIdentity.SetData(target == MarketType.Custom? customTarget : target.ToString(), lc, apiKey);
    }

    IEnumerator _StartSaving()
    {
        autoSave = true;
        for (;;)
        {
            yield return new WaitForSeconds(BeenDatas.SAVEDELAY);
            SaveData();
        }
    }

    public void SaveData()
    {
        if (!autoSave)
            return;
        BeenDatas.LastSavedTime = PacketManager.GetActiveHistoryPacket(true);
    }


    public void SendActiveHistory()
    {
        if (BeenDatas.LastSavedTime != string.Empty)
        {
            WebConnection.SendPacket(BeenDatas.LastSavedTime);
            ClearLastData();
        }
    }

    public static void ClearLastData()
    {
        BeenDatas.LastSavedTime = string.Empty;
    }

    private void OnInited()
    {
        Debug.Log("[DataBeen] SDK v" + PacketsIdentity.sdkVersion + " initialized successfully.");
        if (BeenDatas.IntroductSended == 0)
        {
            SendManager.SendPlayerData();
            BeenDatas.IntroductSended = 1;
        }
        else
        {
            SendActiveHistory();
        }
        SaveData();
    }

    public static void Quited()
    {
        autoSave = false;
        SendManager.SendActiveHistoryData(false);

        ClearLastData();
    }

    public static void SendPurchaseData(string productID, string token, CustomEventInfo[] infos = null)
    {
        SendManager.SendPurchaseData(productID, token, infos);
    }

    public static void SendStartLevelData(string levelName, CustomEventInfo[] infos = null)
    {
        SendManager.SendStartLevelData(levelName, infos);
    }

    public static void SendEndLevelData(string levelName, double score, bool success, CustomEventInfo[] infos = null)
    {
        SendManager.SendEndLevelData(levelName, score, success, infos);
    }
    public static void SendContentViewlData(string contentType, string result, CustomEventInfo[] infos = null)
    {
        SendManager.SendContentViewData(contentType, result, infos);
    }
    public static void SendSharedData(string method, string sharedName, CustomEventInfo[] infos = null)
    {
        SendManager.SendSharedData(method, sharedName, infos);
    }

    public static void SendRatedlData(string ratedName, int rate, CustomEventInfo[] infos = null)
    {
        SendManager.SendRatedData(ratedName, rate, infos);
    }

    public static void SendCustomEventData(string eventName,CustomEventInfo[] infos)
    {
        SendManager.SendCustomEventData(eventName, infos);
    }
}
