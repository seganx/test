using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerManager : Base
{
    private const string serializeKey = "TimerManager.Timers";

    public enum Type // don't change the values!
    {
        Null = 0,
        AdTimer = 1, LoadingBoxItem0 = 2, FullFuelTimer = 3, StartDiscountTimer = 4, FinishDiscountTimer = 5,
        LegendShopActivatorTimer = 6, LegendShopTimer = 7, LeagueStartTimer = 8, LeagueEndTimer = 9, RacerSpecialOfferTimer = 10,
        CombinedShopItemTimer = 11, LoadingBoxItem1 = 12,
        ShopSpecialPackage0 = 20, 
        ShopSpecialPackage1 = 21, 
        ShopSpecialPackage2 = 22, 
        ShopSpecialPackage3 = 23, 
        ShopSpecialPackage4 = 24, 
        ShopSpecialPackage5 = 25, 
        ShopSpecialPackage6 = 26, 
        ShopSpecialPackage7 = 27, 
        ShopSpecialPackage8 = 28, 
        ShopSpecialPackage9 = 29,
    }

    [Serializable]
    private class Timer
    {
        public Type type = Type.Null;
        public long startTime = 0;
        public float duration = 0;
    }

    private void Awake()
    {
        IsTimeValid = false;
        Load();
        StartCoroutine(TryValidateTime());
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    IEnumerator TryValidateTime()
    {
        int tryCount = 0;
        while (!IsTimeValid && tryCount++ < 5)
        {
            ValidateTime();
            yield return new WaitUntil(() => isTimeValidating == true);
        }
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    [Serializable]
    private class SerializableData
    {
        public List<Timer> timers = new List<Timer>();
    }

    private static TimeSpan deltaTime;
    private static DateTime leagueEndTime;
    private static bool isTimeValidating = false;
    private static Dictionary<Type, Timer> timers = new Dictionary<Type, Timer>();

    public static bool IsTimeValid { get; private set; }
    public static DateTime ServerTime { get { return DateTime.Now + deltaTime; } }
    public static long ServerSeconds { get { return ServerTime.Ticks / TimeSpan.TicksPerSecond; } }

    private static void Save()
    {
        var data = new SerializableData();
        foreach (var item in timers)
            data.timers.Add(new Timer() { type = item.Key, startTime = item.Value.startTime, duration = item.Value.duration });
        PlayerPrefsEx.Serialize(serializeKey, data);
    }

    private static void Load()
    {
        // load data
        var data = PlayerPrefsEx.Deserialize(serializeKey, new SerializableData());

        // validate data
        foreach (Type timerType in Enum.GetValues(typeof(Type)))
            if (data.timers.Exists(x => x.type == timerType) == false)
                data.timers.Add(new Timer() { type = timerType });

        // convert data to dictunary
        timers.Clear();
        foreach (var item in data.timers)
            timers.Add(item.type, item);
    }

    public static void SetTimer(Type timerType, float duration, long startTime = 0)
    {
        if (!IsTimeValid)
        {
            //Debug.LogError("ServerTime is invalid");
            //return;
        }

        timers[timerType].startTime = startTime == 0 ? ServerSeconds : startTime;
        timers[timerType].duration = duration;

        Save();
    }

    public static void ValidateTime()
    {
        if (!isTimeValidating)
        {

            IsTimeValid = false;

            isTimeValidating = true;
            Network.GetConfig((msg, res) =>
            {
                isTimeValidating = false;
                if (msg == Network.Message.ok)
                {
                    IsTimeValid = true;
                    deltaTime = new DateTime(res.serverTime, DateTimeKind.Utc) - DateTime.Now;
                    //leagueStartTime = new DateTime(res.league1StartDate, DateTimeKind.Utc);
                    leagueEndTime = new DateTime(res.league1EndDate, DateTimeKind.Utc);

                    if (!TimerManagerInitOnce)
                    {
                        TimerManagerInitOnce = true;
                        InitDefaultValues();
                    }

                    Debug.Log("TimerManager Validated");
                }
            });
        }
    }

    public static int GetRemainTime(Type timerType)
    {
        return Mathf.FloorToInt(timers[timerType].startTime - ServerSeconds + timers[timerType].duration);
    }

    public static int GetLeagueRemainTime()
    {
        return Mathf.FloorToInt(leagueEndTime.Ticks / TimeSpan.TicksPerSecond - ServerSeconds);
        //return 9900;
    }

    private static void InitDefaultValues()
    {
        SetTimer(Type.LoadingBoxItem0, GlobalConfig.Shop.loadingBoxPackage[0].nextTime);
        SetTimer(Type.LoadingBoxItem1, GlobalConfig.Shop.loadingBoxPackage[1].nextTime);
        SetTimer(Type.LegendShopActivatorTimer, GlobalConfig.Shop.blackMarketRefreshTime);
    }

    static string timerManagerInitOnceString = "TimerManagerInitOnce";
    static bool TimerManagerInitOnce
    {
        get { return PlayerPrefs.GetInt(timerManagerInitOnceString, 0) == 1; }
        set { PlayerPrefs.SetInt(timerManagerInitOnceString, value ? 1 : 0); }
    }

#if OFF
    [Serializable]
    public class Test
    {
        public string datetime = string.Empty;
        public long unixtime = 0;

        public void TestTime()
        {
            Http.DownloadText("http://worldtimeapi.org/api/timezone/Etc/UTC", null, null, resjson =>
                {
                    var res = JsonUtility.FromJson<Test>(resjson);
                    Debug.Log(res.GetStringDebug());
                    var st = Utilities.UnixTimeToLocalTime(res.unixtime);
                    Debug.Log(st.Year + " " + st.Month + " " + st.Day + " " + st.Hour + " " + st.Minute + " " + st.Second);
                });
        }
    }
#endif
}