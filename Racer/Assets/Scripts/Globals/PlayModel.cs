using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public static class RaceModel
{
    public enum Mode { Online, Campain, Quests, FreeDrive, Tutorial }

    public static Mode mode = Mode.Online;

    public class Specifications
    {
        public int mapId = 1;
        public int skyId = 0;
        public int weatherId = 0;

        public byte maxPlayerCount = 4;
        public float maxPlayTime = 60;
        public float minForwardSpeed = 50;
        public float maxForwardSpeed = 0;
        public int racersGroup = 0;
    }

    public class Stats
    {
        public float playerSpeed = 0;
        public float playerPosition = 0;
        public int playerRank = 0;
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
    public static bool IsTutorial { get { return mode == Mode.Tutorial; } }

    public static int SelectRandomMap()
    {
        var r = Random.Range(0, 100);
        if (r < 20) return 3;
        if (r > 60) return 1;
        return 2;
    }

    public static void Reset(Mode raceMode)
    {
        mode = raceMode;

        specs = new Specifications();
        stats = new Stats();
        traffic = new Traffic();
    }
}
