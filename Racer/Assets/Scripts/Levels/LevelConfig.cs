using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class LevelConfig : ScriptableObject
{
    [System.Serializable]
    public class RacerInfo
    {
        public string racerId = "0";
        public int nitroLevel = 0;
        public int speedLevel = 0;
        public int steeringLevel = 0;
        public int colorModel = 0;
        public int bodyColor = 0;
        public int windowColor = 0;
        public int spoilerId = 0;
        public int spoilerColor = 0;
        public int vinylId = 0;
        public int vinylColor = 0;
        public int wheelId = 0;
    }

    public enum Environment : int { Desert = 0, Jungle = 1, Mountain = 2 }
    public enum Lighting : int { Day = 0, Night = 1 }

    [Header("Mission")]
    public bool beTheFirst = false;
    public bool donNotCrash = false;
    public int distanceMoreThan = 0;
    [Tooltip("Total left right in seconds")]
    public int noMoreVirajThan = 0;
    public int outpaceCount = 0;
    public int collectObstacles = 0;

    [Header("Config")]
    public Environment environment = Environment.Desert;
    public Lighting lighting = Lighting.Day;
    public int obstaclesId = 0;
    public int obstacleDistanceOffset = 0;
    public int trafficDistanceOffset = 0;
    public int startRacersDistance = 0;
    public int playerStartPosition = 0;
    public int playerMinimumRacerPower = 0;
    public RacerInfo playerRacerInfo = new RacerInfo();
    public RacerInfo[] botsRacerInfo = new RacerInfo[] { new RacerInfo(), new RacerInfo(), new RacerInfo(), new RacerInfo() };

    [Header("Reward")]
    public int gems = 0;
    public int coins = 0;
    public int racerCardId = 0;
    public int racerCardCount = 0;
}
