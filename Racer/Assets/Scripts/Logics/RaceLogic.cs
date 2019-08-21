using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RaceLogic
{
    public class RaceResult
    {
        public int lastScore = 0;
        public int lastLeague = 0;
        public int rewardScore = 0;
        public int newScore { get { return lastScore + rewardScore; } }
        public RewardLogic.RaceReward rewards = null;
    }

    public static RaceResult raceResult = new RaceResult();

    public static void OnRaceFinished()
    {
        switch (RaceModel.mode)
        {
            case RaceModel.Mode.Online: UpdateOnlineResult(); break;
            case RaceModel.Mode.Campain: break;
            case RaceModel.Mode.Quests: break;
            case RaceModel.Mode.FreeDrive: UpdateFreeDriveResult(); break;
            case RaceModel.Mode.Tutorial: UpdateTutorialResult(); break;
        }

        Profile.TotalRaces++;
    }

    private static void UpdateOnlineResult()
    {
        raceResult = new RaceResult();
        raceResult.lastScore = Profile.Score + 1;
        raceResult.lastLeague = Profile.League;

        if (RaceModel.specs.racersGroup == PlayerPresenter.local.racer.GroupId)
            raceResult.rewardScore = GlobalConfig.Race.positionScore[RaceModel.stats.playerRank];
        else
            raceResult.rewardScore = 0;

        var rewardsList = RaceModel.IsOnline ? GlobalConfig.Race.rewardsOnline : GlobalConfig.Race.rewardsOffline;
        var preward = rewardsList[Mathf.Clamp(RaceModel.stats.playerRank, 0, rewardsList.Count - 1)];
        raceResult.rewards = RewardLogic.GetRaceReward(preward.racerCardChance, preward.customeChance, preward.gemChance, preward.gems, preward.coins);

        //  apply rewards to profile
        SetRewardsToProfile();

        Profile.Score = raceResult.newScore;
        Network.SendScore(Profile.Score);
    }

    private static void UpdateFreeDriveResult()
    {
        raceResult = new RaceResult();
        raceResult.lastScore = Profile.Score;
        raceResult.lastLeague = Profile.League;
        raceResult.rewardScore = 0;

        var rewardsList = GlobalConfig.Race.rewardsOffline;
        var preward = rewardsList[Mathf.Clamp(RaceModel.stats.playerRank, 0, rewardsList.Count - 1)];
        raceResult.rewards = RewardLogic.GetRaceReward(preward.racerCardChance, preward.customeChance, preward.gemChance, preward.gems, preward.coins);

        //  apply rewards to profile
        SetRewardsToProfile();
    }


    private static void UpdateTutorialResult()
    {
        raceResult = new RaceResult();
        raceResult.lastScore = Profile.Score;
        raceResult.lastLeague = Profile.League;
        raceResult.rewardScore = 0;

        //  verify that this is the first race
        if (Profile.SelectedRacer > 0) return;

        var preset = GlobalConfig.ProfilePresets.RandomOne();
        var racerconfig = RacerFactory.Racer.GetConfig(preset.racerId);
        raceResult.rewards = new RewardLogic.RaceReward();
        raceResult.rewards.gems = preset.gems;
        raceResult.rewards.coins = preset.coins;
        raceResult.rewards.racerId = preset.racerId;
        raceResult.rewards.racerCount = racerconfig.CardCount;

        Profile.EarnResouce(preset.gems, preset.coins);
        Profile.AddRacerCard(preset.racerId, racerconfig.CardCount);
        Profile.UnlockRacer(preset.racerId);
        Profile.SelectedRacer = preset.racerId;

        for (int i = 0; i < preset.rndCards; i++)
        {
            var racerid = RewardLogic.SelectRacerReward();
            Profile.AddRacerCard(racerid, 1);
            Popup_Rewards.AddRacerCard(racerid, 1);
        }
    }

    private static void SetRewardsToProfile()
    {
        if (raceResult.rewards.custome != null)
            Profile.AddRacerCustome(raceResult.rewards.custome.type, raceResult.rewards.custome.racerId, raceResult.rewards.custome.customId);
        if (raceResult.rewards.racerId > 0)
            Profile.AddRacerCard(raceResult.rewards.racerId, raceResult.rewards.racerCount);
        if (raceResult.rewards.gems > 0)
            Profile.EarnResouce(raceResult.rewards.gems, 0);
        if (raceResult.rewards.coins > 0)
            Profile.EarnResouce(0, raceResult.rewards.coins);
    }
}
