using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPresenter : Base
{
    private PlayerPresenterOnline player = null;
    private bool doViraj = false;
    private int defaultSteering = 1;
    private float nosTimer = 0;

    private bool CanControl { get { return player.player.IsPlayer || PlayNetwork.IsMaster; } }
    private bool IsTimeValid { get { return RaceModel.stats.playTime > 5; } }

    private IEnumerator Start()
    {
        nosTimer = Random.Range(0, 0.5f);
        player = GetComponent<PlayerPresenterOnline>();

        var trafficCounter = player.racer.gameObject.AddComponent<RacerTrafficCounter>();
        if (trafficCounter)
        {
            trafficCounter.MinSizeDistance = 15;
            trafficCounter.MaxSizeDistance = 20;
        }

        var waitWhile = new WaitForSeconds(1);
        yield return waitWhile;
        bool canViraj = RaceModel.IsOnline && Random.Range(0, 100) < GlobalConfig.Race.bots.canVirajChance;

        while (true)
        {
            doViraj = canViraj && Random.Range(0, 100) < GlobalConfig.Race.bots.doVirajChance;
            defaultSteering = Random.Range(0, 100) < 50 ? 1 : -1;
            if (Random.Range(0, 100) < GlobalConfig.Race.bots.crashChance) player.OnCrashed();
            yield return waitWhile;
        }
    }

    private void FixedUpdate()
    {
        if (IsTimeValid == false || CanControl == false) return;

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
        else if (player.player.CurrPosition < 400)
            player.SteeringValue = Mathf.MoveTowards(player.SteeringValue, doViraj ? (isleft ? 1 : (isright ? -1 : defaultSteering)) : 0, Time.deltaTime * 2);
        else
            player.SteeringValue = Mathf.MoveTowards(player.SteeringValue, 0, Time.deltaTime * 2);


        var nosMaxTime = 0;
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
#if OFF
    private static List<ProfileData> candidates = new List<ProfileData>();
    private static int candidatesBaseScore = 0;
    public static void UpdateCandidates()
    {
        if (Mathf.Abs(Profile.Score - candidatesBaseScore) < 100) return;
    }
#endif

    public static void InitializeBots(int count, int playerScore, int playerRacerId, int playerPower)
    {
        for (int i = 0; i < count; i++)
        {
            var botScore = Random.Range(playerScore - 50, playerScore + 50);
            var botRank = Random.Range(Profile.Position - 50, Profile.Position + 50);

            RacerProfile botRacer = null;
            if (RaceModel.IsTutorial)
            {
                var config = RacerFactory.Racer.GetConfig(370);
                botRacer = CreateRandomRacerProfile(config.Id, Random.Range(config.MinPower, config.MaxPower), playerScore);
            }
            else botRacer = CreateRandomRacerProfile(playerRacerId, playerPower, playerScore);

            var pdata = new PlayerData(RaceModel.IsOnline ? GlobalFactory.GetRandomName() : "Player " + i, botScore, botRank, botRacer);
            Debug.LogWarning("bot " + pdata.name + " joined.");
            PlayerPresenterOnline.Create(pdata, true);
        }
    }

    private static RacerProfile CreateRandomRacerProfile(int playerRacerId, int playerPower, int playerScore)
    {
        int targetPower = 0;
        int groupId = RacerFactory.Racer.GetConfig(playerRacerId).GroupId;
        if (RaceModel.IsOnline)
        {
            var factor = GlobalConfig.Race.bots.powers[groupId];
            targetPower = Mathf.RoundToInt(factor.x * playerScore + factor.y);
        }
        else
        {
            var factor = GlobalConfig.Race.bots.powers[0];
            targetPower = Mathf.RoundToInt(factor.x * playerPower + factor.y);
        }

        var res = new RacerProfile() { id = SelectRacer(groupId, targetPower, playerRacerId) };
        var config = RacerFactory.Racer.GetConfig(res.id);
        res.cards = config.CardCount;
        res.level.Level = 1;

        var maxUpgradeLevel = RacerGlobalConfigs.Data.maxUpgradeLevel[res.level.Level];
        res.level.SpeedLevel = Random.Range(0, maxUpgradeLevel);
        res.level.NitroLevel = Random.Range(0, maxUpgradeLevel);
        res.level.SteeringLevel = Random.Range(0, maxUpgradeLevel);
        res.level.BodyLevel = Random.Range(0, maxUpgradeLevel);

        res.custom = config.DefaultRacerCustom;
        res.custom.BodyColor = RacerFactory.Colors.AllColors.RandomOne().id;
        res.custom.Wheel = RacerFactory.Wheel.GetPrefabs(config.Id).RandomOne().Id;
        res.custom.Spoiler = Random.Range(0, 100) < 10 ? RacerFactory.Spoiler.GetPrefabs(config.Id).RandomOne().Id : 0;
        res.custom.Vinyl = Random.Range(0, 100) < 10 ? RacerFactory.Vinyl.GetPrefabs(config.Id).RandomOne().Id : 0;
        res.custom.Hood = Random.Range(0, 100) < 10 && RacerFactory.Hood.GetPrefabs(config.Id).Count > 0 ? RacerFactory.Hood.GetPrefabs(config.Id).RandomOne().Id : 0;
        res.custom.Roof = Random.Range(0, 100) < 10 && RacerFactory.Roof.GetPrefabs(config.Id).Count > 0 ? RacerFactory.Roof.GetPrefabs(config.Id).RandomOne().Id : 0;

        return res;
    }

    private static int SelectRacer(int groupId, int targetPower, int playerRacerId)
    {
        targetPower = Mathf.Max(targetPower, RacerFactory.Racer.AllConfigs[0].MinPower);
        var list = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId == groupId && x.MinPower.Between(targetPower + GlobalConfig.Race.bots.powerRange.x, targetPower + GlobalConfig.Race.bots.powerRange.y));
        if (list.Count < 1) list = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId == groupId);
        if (list.Count < 1) list = RacerFactory.Racer.AllConfigs;
        var center = list.FindIndex(x => x.Id == playerRacerId);
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
