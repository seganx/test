using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public static class RaceModel
{
    public enum Mode { Online, Campain, Quests, FreeDrive }

    public static Mode mode = Mode.Online;

    public class Specifications
    {
        public int mapId = 1;
        public int skyId = 0;
        public int weatherId = 0;

        public byte maxPlayerCount = 4;
        public float maxPlayTime = 60;
        public float minForwardSpeed = 50;
        public float maxForwardSpeed = 100;
        public int racersGroup = 0;
    }

    public class Stats
    {
        public float speed = 0;
        public float forwardPosition = 0;
        public float playerForwardPosition = 0;
        public int playerPosition = 0;
        

        public int TotalPassed
        {
            get { return PlayerPrefsEx.GetInt("PlayModel.TotalPassed", 1); }
            set { PlayerPrefsEx.SetInt("PlayModel.TotalPassed", value); }
        }

        public int TotalSuccessed
        {
            get { return PlayerPrefsEx.GetInt("PlayModel.TotalSuccessed", 0); }
            set { PlayerPrefsEx.SetInt("PlayModel.TotalSuccessed", value); }
        }
    }

    public class Traffic
    {
        public float baseDistance = 0;
        public float distanceRatio = 0;
    }


    public static Specifications specs = new Specifications();
    public static Stats stats = new Stats();
    public static Traffic traffic = new Traffic();

    public static bool IsOnline { get { return mode == Mode.Online; } }
    public static bool IsCampain { get { return mode == Mode.Campain; } }
    public static bool IsQuests { get { return mode == Mode.Quests; } }
    public static bool IsFreeDrive { get { return mode == Mode.FreeDrive; } }

    public static int SelectRandomMap()
    {
        var r = Random.Range(0, 100);
        if (r < 20) return 3;
        if (r > 60) return 1;
        return 2;
    }

    public static void Reset()
    {
        specs = new Specifications();
        stats = new Stats();
        traffic = new Traffic();
    }
}
