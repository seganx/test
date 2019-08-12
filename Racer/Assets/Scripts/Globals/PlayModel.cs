using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayModel
{
    public enum Mode { Online, Campain, Quests, FreeDrive }

    public static Mode mode = Mode.Online;

    public static int mapId = 1;
    public static int skyId = 0;
    public static int weatherId = 0;

    public static byte maxPlayerCount = 4;
    public static float maxPlayTime = 60;
    public static float minForwardSpeed = 50;
    public static float maxForwardSpeed = 100;

    public static class CurrentPlaying
    {
        public static float speed = 0;
        public static float forwardPosition = 0;
        public static float playerForwardPosition = 0;
    }

    public static class Traffic
    {
        public static float baseDistance = 0;
        public static float distanceRatio = 0;
    }

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
}
