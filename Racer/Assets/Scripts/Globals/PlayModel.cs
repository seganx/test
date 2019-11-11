﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RaceModel
{
    public enum Mode { Online, Campain, Quests, FreeDrive, Tutorial }
    public enum SteeringMode : int { Null, Default, Easy, Tilt }

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
        public float playTime = 0;
        public float globalSpeed = 0;
        public float playerSpeed = 0;
        public float playerPosition = 0;
        public int playerRank = 0;
        public float playerBehindDistance = 0;
        public float playerMaxBehindDistance = 0;
    }

    public class Traffic
    {
        public float baseDistance = 0;
        public float distanceRatio = 0;
    }


    public static Specifications specs = new Specifications();
    public static Stats stats = new Stats();
    public static Traffic traffic = new Traffic();

    private static SteeringMode steering = SteeringMode.Null;

    public static bool IsOnline { get { return mode == Mode.Online; } }
    public static bool IsCampain { get { return mode == Mode.Campain; } }
    public static bool IsQuests { get { return mode == Mode.Quests; } }
    public static bool IsFreeDrive { get { return mode == Mode.FreeDrive; } }
    public static bool IsTutorial { get { return mode == Mode.Tutorial; } }

    public static SteeringMode Steering
    {
        get
        {
            if (steering == SteeringMode.Null)
                steering = (SteeringMode)PlayerPrefs.GetInt("RaceModel.Steering", (int)SteeringMode.Default);
            return steering;
        }

        set
        {
            if (steering == value) return;
            steering = value;
            PlayerPrefs.SetInt("RaceModel.Steering", (int)value);
        }
    }

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
