using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VersionPlayerData
{
    public int ver = 1;
}

[System.Serializable]
public class PlayerData : VersionPlayerData
{
    public int[] id = new int[4];
    public float[] fd = new float[4];
    public RacerCustomData rc = null;
    public string name = string.Empty;

    public int LeagueRank { get { return id[1]; } set { id[1] = value; } }
    public int Score { get { return id[1]; } set { id[1] = value; } }
    public int RacerId { get { return id[2]; } set { id[2] = value; } }
    public int RacerPower { get { return id[3]; } set { id[3] = value; } }
    public float RacerSpeed { get { return fd[0]; } set { fd[0] = value; } }
    public float RacerNitrous { get { return fd[1]; } set { fd[1] = value; } }
    public float RacerSteering { get { return fd[2]; } set { fd[2] = value; } }
    public float RacerBody { get { return fd[3]; } set { fd[3] = value; } }
    public RacerCustomData RacerCustom { get { return rc; } set { rc = value; } }

    //  dynamic properties
    public bool IsPlayer { get; set; }
    public float CurrPosition { get; set; }
    public int CurrRank { get; set; }
    public float CurrNitrous { get; set; }

    public PlayerData (string playerName, int score, int leagueRank, RacerProfile racer)
    {
        name = playerName;
        LeagueRank = leagueRank;
        Score = score;
        RacerId = racer.id;
        RacerCustom = racer.custom;
        var config = RacerFactory.Racer.GetConfig(RacerId);
        RacerSpeed = config.ComputeSpeed(racer.level.NitroLevel);
        RacerNitrous = config.ComputeNitro(racer.level.NitroLevel);
        RacerSteering = config.ComputeSteering(racer.level.SteeringLevel);
        RacerBody = config.ComputeBody(racer.level.BodyLevel);
        RacerPower = config.ComputePower(racer.level.SpeedLevel, racer.level.NitroLevel, racer.level.SteeringLevel, racer.level.BodyLevel);
    }

    public static PlayerData FromJson(string json)
    {
        var version = JsonUtility.FromJson<VersionPlayerData>(json);
        if (version.ver == 1)
            return JsonUtility.FromJson<PlayerData>(json);
        else
            return new PlayerData(string.Empty, 0, 0, new RacerProfile());
    }
}
