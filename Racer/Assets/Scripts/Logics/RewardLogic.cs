using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RewardLogic
{
    public class RacerCustomReward
    {
        public RacerCustomeType type = RacerCustomeType.None;
        public int racerId = 0;
        public int customId = 0;
    }

    public class RaceReward
    {
        public int gem = 0;
        public int racerId = 0;
        public RacerCustomReward custome = null;
    }

    //private static List<int> chances = new List<int>(100);

    public static bool IsFirstRace
    {
        get { return PlayerPrefs.GetInt("RewardLogic.IsFirstRace", 0) > 0; }
        set { PlayerPrefs.SetInt("RewardLogic.IsFirstRace", value ? 1 : 0); }
    }

    public static RaceReward GetRaceReward(int racerCardChance, int customeChance, int gemChance, int gems)
    {
        var res = new RaceReward();
        if (Random.Range(0, 100) < gemChance)
            res.gem = gems;
        if (Random.Range(0, 100) < racerCardChance)
            res.racerId = SelectRacerReward();
        if (Random.Range(0, 100) < customeChance)
            res.custome = GetCustomReward();
        return res;
    }

    //! return the index of first locked racer in garage just front of unlocked one
    public static int FindSelectRacerCenter(int playerIndex = 0)
    {
        var playerselected = Profile.SelectedRacer > 0 ? Profile.SelectedRacer : 0;
        var center = playerIndex == 0 ? RacerFactory.Racer.AllConfigs.FindIndex(x => x.Id == playerselected) : playerIndex;
        for (int i = center + 1; i < RacerFactory.Racer.AllConfigs.Count && Profile.IsUnlockedRacer(RacerFactory.Racer.AllConfigs[i].Id); i++)
            center = i;
        return center;
    }

    public static int SelectRacerReward()
    {
        var center = FindSelectRacerCenter();
        center += GlobalConfig.Probabilities.rewardRacerRadius / 2;
        var index = SelectProbability(RacerFactory.Racer.AllConfigs.Count, center, GlobalConfig.Probabilities.rewardRacerRadius);
        return RacerFactory.Racer.AllConfigs[index].Id;
    }

    public static RacerCustomReward GetCustomReward(int racerId = 0)
    {
        if (racerId < 1)
        {
            var config = RacerFactory.Racer.AllConfigs.FindAll(x => Profile.IsUnlockedRacer(x.Id)).RandomOne();
            racerId = config != null ? config.Id : Profile.SelectedRacer;
        }

        switch (Random.Range(0, 100) % 5)
        {
            case 0: return FindCustomReward(RacerCustomeType.Hood, racerId, RacerFactory.Hood.GetPrefabs(racerId));
            case 1: return FindCustomReward(RacerCustomeType.Roof, racerId, RacerFactory.Roof.GetPrefabs(racerId));
            case 2: return FindCustomReward(RacerCustomeType.Spoiler, racerId, RacerFactory.Spoiler.GetPrefabs(racerId));
            case 3: return FindCustomReward(RacerCustomeType.Vinyl, racerId, RacerFactory.Vinyl.GetPrefabs(racerId));
            case 4: return FindCustomReward(RacerCustomeType.Wheel, racerId, RacerFactory.Wheel.GetPrefabs(racerId));
        }

        return null;
    }

    private static RacerCustomReward FindCustomReward(RacerCustomeType type, int racerId, List<RacerCustomPresenter> list)
    {
        var res = new RacerCustomReward();
        if (list.Count > 0)
        {
            res.type = type;
            res.racerId = racerId;
            var lockedList = list.FindAll(x => Profile.IsUnlockedCustome(type, racerId, x.Id) == false);
            res.customId = (lockedList.Count > 0) ? lockedList.RandomOne().Id : list.RandomOne().Id;
        }
        return res;
    }



    public static int SelectProbability(int lenght, int center, int radius, float heightFactor = 0.5f)
    {
        var list = new List<int>(lenght * 3);
        for (int i = 0; i < lenght; i++)
            list.Add(i);

        float height = lenght * heightFactor;
        float m = radius / height;
        for (int h = 0; h <= height; h++)
        {
            int rad = Mathf.CeilToInt(radius - m * h);
            for (int r = center - rad; r <= center + rad; r++)
                if (r.Between(0, lenght - 1))
                    list.Add(r);
        }

        var index = Random.Range(0, list.Count);
        return list[index];
    }

    public static int SelectProbabilityForward(int lenght, int center, int radius, float heightFactor = 0.5f)
    {
        var list = new List<int>(lenght * 3);
        for (int i = 0; i < lenght; i++)
            list.Add(i);

        float height = lenght * heightFactor;
        float m = radius / height;
        for (int h = 0; h <= height; h++)
        {
            int rad = Mathf.CeilToInt(radius - m * h) * 2;
            for (int r = center; r <= center + rad; r++)
                if (r.Between(0, lenght - 1))
                    list.Add(r);
        }

        var index = Random.Range(0, list.Count);
        return list[index];
    }
}
