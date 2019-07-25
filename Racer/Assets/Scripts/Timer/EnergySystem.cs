using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    /*
    static int energyCountDown;

    private static bool isUtcTimeValid = false;
    private static long startUtcSecond;
    private static int maxNumberOfEnergy = 10, chargeDuration = 300;

    private static string energyCountsString = "EnergyCounts";
    public static int EnergyCounts
    {
        get { return PlayerPrefs.GetInt(energyCountsString, maxNumberOfEnergy); }
        private set { PlayerPrefs.SetInt(energyCountsString, value); }
    }

    string lastDeliveryTimeString = "lastDeliveryTimeString";
    long lastEnergyDeliveryTime
    {
        set { PlayerPrefs.SetString(lastDeliveryTimeString, value.ToString()); }
        get { return Convert.ToInt64(PlayerPrefs.GetString(lastDeliveryTimeString, "0")); }
    }

    long GetServerUtcTime() { return startUtcSecond + (long)Time.time; }

    public static void UpdateUtcTime(long serverTime)
    {
        isUtcTimeValid = true;
        DateTime dateTime = new DateTime(1970, 1, 1).AddMilliseconds(serverTime);
        startUtcSecond = (dateTime.Ticks / TimeSpan.TicksPerSecond) - (long)Time.time;
    }

    bool alreadyStarted = false;
    public void StartCheckEnergy()
    {
        if (alreadyStarted)
            return;
        alreadyStarted = true;
        StartCoroutine(iCheckEnergy());
    }

    IEnumerator iCheckEnergy()
    {
        if (isUtcTimeValid)
        {
            int nEnergyCount = GetNewEnergyCount();
            if (nEnergyCount > 0)
                EnergyCounts = Mathf.Min(maxNumberOfEnergy, EnergyCounts + nEnergyCount);

            energyCountDown = (int)(lastEnergyDeliveryTime + chargeDuration - GetServerUtcTime());
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(iCheckEnergy());
    }

    int GetNewEnergyCount()
    {
        int result = (int)((GetServerUtcTime() - lastEnergyDeliveryTime) / chargeDuration);
        lastEnergyDeliveryTime += result * chargeDuration;
        return result;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            isUtcTimeValid = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            isUtcTimeValid = false;
    }*/
}
