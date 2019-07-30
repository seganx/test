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
        AdTimer = 1, FreeShopItemTimer = 2, FullFuelTimer = 3, StartDiscountTimer = 4, FinishDiscountTimer = 5,
        LegendShopActivatorTimer = 6, LegendShopTimer = 7, LeagueStartTimer = 8, LeagueEndTimer = 9, RacerSpecialOfferTimer = 10,
        CombinedShopItemTimer = 11
    }

    [Serializable]
    public class Timer
    {
        public long startTime = 0;
        public float duration = 0;
    }

    private void Awake()
    {
        IsTimeValid = false;
        timers = PlayerPrefsEx.DeserializeBinary(serializeKey, new Dictionary<Type, Timer>());
        foreach (Type timerType in Enum.GetValues(typeof(Type)))
            if (!timers.ContainsKey(timerType))
                timers.Add(timerType, new Timer());
        PlayerPrefsEx.SerializeBinary(serializeKey, timers);

        StartCoroutine(TryValidateTime());
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
    /*
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            IsTimeValid = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            IsTimeValid = false;
    }
    */

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    private static TimeSpan deltaTime;
    //private static DateTime leagueStartTime;
    private static DateTime leagueEndTime;
    private static bool isTimeValidating = false;
    private static Dictionary<Type, Timer> timers = new Dictionary<Type, Timer>();

    public static bool IsTimeValid { get; private set; }
    public static DateTime ServerTime { get { return DateTime.Now + deltaTime; } }
    public static long ServerSeconds { get { return ServerTime.Ticks / TimeSpan.TicksPerSecond; } }

    public static void SetTimer(Type timerType, float duration, long startTime = 0)
    {
        if (!IsTimeValid)
        {
            //Debug.LogError("ServerTime is invalid");
            //return;
        }

        timers[timerType].startTime = startTime == 0 ? ServerSeconds : startTime;
        timers[timerType].duration = duration;

        PlayerPrefsEx.SerializeBinary(serializeKey, timers);
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
        SetTimer(Type.FreeShopItemTimer, GlobalConfig.Shop.loadingBoxPackage.nextTime);
        SetTimer(Type.LegendShopActivatorTimer, GlobalConfig.Shop.racerCardPackageTime);

        SetTimer(Type.CombinedShopItemTimer, GlobalConfig.Shop.combinedPackagesNextTime);
    }

    static string timerManagerInitOnceString = "TimerManagerInitOnce";
    static bool TimerManagerInitOnce
    {
        get { return PlayerPrefs.GetInt(timerManagerInitOnceString, 0) == 1; }
        set { PlayerPrefs.SetInt(timerManagerInitOnceString, value ? 1 : 0); }
    }




    [Console("shop", "legends")]
    public static void OpenLegendShop()
    {
        SetTimer(Type.LegendShopActivatorTimer, 5, 0);
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