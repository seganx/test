﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPresenter : Base
{
    private PlayerPresenterOnline player = null;
    private int defaultSteering = 1;
    private float nosTimer = 0;
    private bool CanControl { get { return player.player.IsPlayer || PlayNetwork.IsMaster; } }

    private IEnumerator Start()
    {
        player = GetComponent<PlayerPresenterOnline>();

        var waitWhile = new WaitForSeconds(2);
        yield return waitWhile;

        while (true)
        {
            defaultSteering = Random.Range(0, 100) > 50 ? 1 : -1;
            yield return waitWhile;
        }
    }

    private void FixedUpdate()
    {
        if (CanControl == false) return;

        var isleft = player.racer.transform.localPosition.x < -RoadPresenter.RoadWidth * 0.4f;
        var isright = player.racer.transform.localPosition.x > RoadPresenter.RoadWidth * 0.4f;
        var left = WatchOut(false);
        var right = WatchOut(true);
        if (left && right)
            player.SteeringValue = isleft ? 1 : (isright ? -1 : defaultSteering);
        else if (left)
            player.SteeringValue = isright ? -1 : 1;
        else if (right)
            player.SteeringValue = isleft ? 1 : -1;
        else
            player.SteeringValue = Mathf.MoveTowards(player.SteeringValue, 0, Time.deltaTime * 2);

        var nosMaxTime = 1;
        if (RaceModel.IsTutorial)
        {
            if (player.player.CurrRank == 0)
                player.player.CurrNitrous = 0;
            else
                nosMaxTime = 0;
        }
        else if (RaceModel.IsFreeDrive)
        {
            if (Profile.TotalRaces < 5)
                nosMaxTime = 8 - Profile.TotalRaces;
        }

        if (player.IsNitrosFull)
        {
            nosTimer += Time.fixedDeltaTime;
            if (nosTimer >= nosMaxTime)
            {
                nosTimer = 0;
                player.UseNitrous();
            }
        }
    }

    private bool WatchOut(bool right)
    {
        var pos = (right ? transform.right : -transform.right) * player.racer.Size.x * 0.5f;
        RaycastHit hit;
        return Physics.Raycast(player.racer.transform.position + pos, transform.forward, out hit, GlobalConfig.Race.bots.rayDistance + GlobalConfig.Race.bots.raySpeedFactor * RaceModel.specs.maxForwardSpeed, 1 << 9);
    }

    ////////////////////////////////////////////////////////////////////////////////////
    //  STATIC MEMEBRS
    ////////////////////////////////////////////////////////////////////////////////////
    public static void InitializeBots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var botScore = Random.Range(Profile.Score - 50, Profile.Score + 50);
            var botRank = Random.Range(Profile.Position - 50, Profile.Position + 50);

            RacerProfile botRacer = null;
            if (RaceModel.IsTutorial)
            {
                var config = RacerFactory.Racer.GetConfig(370);
                botRacer = CreateRandomRacerProfile(i, Random.Range(config.MinPower, config.MaxPower), config.Id, Random.Range(config.MinPower, config.MaxPower));
            }
            else botRacer = CreateRandomRacerProfile(i, Profile.Score, Profile.SelectedRacer, Profile.CurrentRacerPower);

            var pdata = new PlayerData(RaceModel.IsOnline ? GlobalFactory.GetRandomName() : "Player " + i, botScore, botRank, botRacer);
            PlayerPresenterOnline.Create(pdata, true);
        }
    }

    private static RacerProfile CreateRandomRacerProfile(int index, int playerScore, int playerRacerId, int playerPower)
    {
        int targetPower = 0;
        if (RaceModel.IsOnline)
        {
            if (index == 0)
            {
                targetPower = playerPower;
            }
            else if (index == 1)
            {
                var factor = GlobalConfig.Race.bots.powers[RacerFactory.Racer.GetConfig(playerRacerId).GroupId];
                targetPower = Mathf.RoundToInt(factor.x * playerScore + factor.y);
            }
            else
            {
                var factor = GlobalConfig.Race.bots.powers[RacerFactory.Racer.GetConfig(playerRacerId).GroupId];
                targetPower = Mathf.RoundToInt(factor.x * playerScore + factor.y + GlobalConfig.Race.bots.powers[0].y);

                //var factor = GlobalConfig.Race.bots.powers[0];
                //targetPower = Mathf.RoundToInt(factor.x * playerScore + factor.y);
            }
        }
        else
        {
            if (index == 0)
                targetPower = playerPower;
            else if (index == 1)
                targetPower = Mathf.RoundToInt(playerPower + GlobalConfig.Race.bots.powers[0].y);
            else
                targetPower = Mathf.RoundToInt(playerPower + 2 * GlobalConfig.Race.bots.powers[0].y);
        }

        var res = new RacerProfile() { id = SelectRacer(targetPower) };
        var config = RacerFactory.Racer.GetConfig(res.id);
        res.cards = config.CardCount;
        res.level.Level = 1;

        if (Profile.TotalRaces > 4)
        {
            var maxUpgradeLevel = RacerGlobalConfigs.Data.maxUpgradeLevel[res.level.Level] + 1;
            res.level.SpeedLevel = Random.Range(0, maxUpgradeLevel / 2);
            res.level.NitroLevel = Random.Range(0, maxUpgradeLevel / 2);
            res.level.BodyLevel = Random.Range(0, maxUpgradeLevel / 2);
            res.level.SteeringLevel = Random.Range(0, maxUpgradeLevel / 2);
        }

        res.custom = config.DefaultRacerCustom;
        res.custom.BodyColor = RacerFactory.Colors.AllColors.RandomOne().id;
        res.custom.Wheel = RacerFactory.Wheel.GetPrefabs(config.Id).RandomOne().Id;
        res.custom.Spoiler = Random.Range(0, 100) < 10 ? RacerFactory.Spoiler.GetPrefabs(config.Id).RandomOne().Id : 0;
        res.custom.Vinyl = Random.Range(0, 100) < 10 ? RacerFactory.Vinyl.GetPrefabs(config.Id).RandomOne().Id : 0;
        res.custom.Hood = Random.Range(0, 100) < 10 && RacerFactory.Hood.GetPrefabs(config.Id).Count > 0 ? RacerFactory.Hood.GetPrefabs(config.Id).RandomOne().Id : 0;
        res.custom.Roof = Random.Range(0, 100) < 10 && RacerFactory.Roof.GetPrefabs(config.Id).Count > 0 ? RacerFactory.Roof.GetPrefabs(config.Id).RandomOne().Id : 0;

        return res;
    }

    private static int SelectRacer(int targetPower)
    {
        targetPower = Mathf.Max(targetPower, RacerFactory.Racer.AllConfigs[0].MinPower);
        var list = RacerFactory.Racer.AllConfigs.FindAll(x => x.MinPower.Between(targetPower - 400, targetPower + 100));
        if (list.Count < 1) list = RacerFactory.Racer.AllConfigs;
        var center = list.FindIndex(x => x.Id == Profile.SelectedRacer);
        var index = SelectProbability(list.Count, center - 1, 4, 0.5f);
        return list[index].Id;
    }

    private static int SelectProbability(int lenght, int center, int radius, float heightFactor)
    {
        var list = new List<int>(lenght * 3);
        for (int i = 0; i < lenght; i++)
            list.Add(i);

        float height = lenght * heightFactor;
        float m = radius / height;
        for (int h = 0; h <= height; h++)
        {
            int rad = Mathf.CeilToInt(radius - m * h);
            for (int r = center - rad * 2; r <= center + rad; r++)  // increase radius to select prev racers
                if (r.Between(0, lenght - 1))
                    list.Add(r);
        }

        var index = Random.Range(0, list.Count);
        return list[index];
    }
}
