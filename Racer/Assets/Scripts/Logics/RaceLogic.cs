﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RaceLogic
{
    public class OnlineResult
    {
        public int lastScore = 0;
        public int lastLeague = 0;
        public int rewardScore = 0;
        public int newScore { get { return lastScore + rewardScore; } }
    }

    public static OnlineResult onlineResult = new OnlineResult();

    public static void OnRaceFinished()
    {
        switch (RaceModel.mode)
        {
            case RaceModel.Mode.Online: UpdateOnlineResult(); break;
            case RaceModel.Mode.Campain: break;
            case RaceModel.Mode.Quests: break;
            case RaceModel.Mode.FreeDrive: break;
        }
    }

    private static void UpdateOnlineResult()
    {
        onlineResult = new OnlineResult();
        onlineResult.lastScore = Profile.Score + 1;
        onlineResult.lastLeague = Profile.League;
        onlineResult.rewardScore = GlobalConfig.Race.positionScore[RaceModel.stats.playerPosition];

        Profile.Score = onlineResult.newScore;
        Network.SendScore(Profile.Score);

        var tcounter = PlayerPresenter.local.racer.GetComponent<RacerTrafficCounter>();
        RaceModel.stats.TotalPassed = tcounter.TotalTrafficPassed;
        RaceModel.stats.TotalSuccessed = tcounter.TotalTrafficSuccess;
    }
}
