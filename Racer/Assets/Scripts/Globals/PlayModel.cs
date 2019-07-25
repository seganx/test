using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayModel
{
    private const int mapCount = 3;

    public static bool OfflineMode = false;
    public static int eloScore = 1;
    public static int eloPower = 1;
    public static byte maxPlayerCount = 4;
    public static float maxGameTime = 60;
    public static float minForwardSpeed = 50;
    public static float maxForwardSpeed = 100;
    public static int selectedMapId
    {
        get { return PlayerPrefs.GetInt("PlayModel.SelectedMapId", 1); }
        set { PlayerPrefs.SetInt("PlayModel.SelectedMapId", value); }
    }

    public static int SelectRandomMap()
    {
        var r = Random.Range(0, 100);
        if (r < 20) return 3;
        if (r > 60) return 1;
        return 2;
    }
}
