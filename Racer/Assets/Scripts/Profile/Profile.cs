using SeganX;

public static class Profile
{
    private static ProfileData data = new ProfileData();

    public static bool IsFirstSession { get; set; }

    public static RacerProfile CurrentRacer { get { return data.racers.Find(x => x.id == data.selectedRacer); } }

    public static string UserId { get { return data.userId; } }

    public static string Key { get { return data.key; } }

    public static int Gem { get { return data.gem; } }

    public static int Coin { get { return data.gold; } }

    public static bool HasName { get { return data.nickName.HasContent(); } }

    public static string Name
    {
        get { return HasName ? data.nickName : (data.userId.HasContent() ? data.userId : "player"); }
        set { data.nickName = value; }
    }

    public static int Score
    {
        get { return data.score; }
        set { data.score = value; }
    }

    public static int Skill
    {
        get { return data.skill > 1 ? data.skill : CurrentRacerPower; }
        set { data.skill = value; }
    }

    public static int Position
    {
        get { return data.position.Value > 0 ? data.position.Value : 1318976853 - data.score; }
    }

    public static string PositionString
    {
        get { return data.position.Value > 0 ? data.position.Value.ToString("#,0") : "-"; }
    }

    public static int League
    {
        get { return GlobalConfig.Leagues.GetIndex(data.score, data.position); }
    }

    public static bool LeagueResultExist
    {
        get { return data.rewardedPosition.Value != 0 && data.rewardedScore != 0; }
    }

    public static int LeagueResultScore
    {
        get { return data.rewardedScore; }
    }

    public static int LeagueResultPosition
    {
        get { return data.rewardedPosition; }
    }

    public static int EloScore
    {
        get { return Score; }
    }

    public static int CurrentRacerPower
    {
        get
        {
            var racerconfig = RacerFactory.Racer.GetConfig(SelectedRacer);
            return racerconfig == null ? 0 : racerconfig.ComputePower(CurrentRacer.level.NitroLevel, CurrentRacer.level.SteeringLevel, CurrentRacer.level.BodyLevel);
        }
    }

    public static int SelectedRacer
    {
        get { return data.selectedRacer; }
        set { if (IsUnlockedRacer(value)) data.selectedRacer = value; }
    }

    public static bool JoinedInstagram
    {
        get { return data.socials.IsFlagOn(2); }
        set { if (value) data.socials |= 2; }
    }

    public static ProfileData Data
    {
        get { return data; }
        set
        {
            if (value != null && value.data != null && value.version == 1)
            {
                foreach (var item in value.racers)
                    if (IsReadyToUnlock(item))
                        item.unlock = 1;
                value.version = 2;
            }

            data = value;
        }
    }

    public static bool IsUnlockingRacerExist
    {
        get
        {
            foreach (var item in data.racers)
                if (IsReadyToUnlock(item))
                    return true;
            return false;
        }
    }

    public static bool SpendGem(int value)
    {
        if (Gem >= value)
        {
            data.gem = data.gem - value;
            return true;
        }
        return false;
    }

    public static bool SpendCoin(int value)
    {
        if (Coin >= value)
        {
            data.gold = data.gold - value;
            return true;
        }
        return false;
    }

    public static void EarnResouce(int gem, int coin)
    {
        data.gem += gem;
        data.gold += coin;
    }

    public static RacerProfile GetRacer(int id)
    {
        return data.racers.Find(x => x.id == id);
    }

    public static int AddRacerCard(int racerId, int count)
    {
        var rp = GetRacer(racerId);
        if (rp == null)
        {
            var config = RacerFactory.Racer.GetConfig(racerId);
            rp = new RacerProfile() { id = config.Id, custom = config.DefaultRacerCustom };
            data.AddRacer(rp);
        }
        return (rp.cards += count);
    }

    public static void AddRacerCustome(RacerCustomeType type, int racerId, int customId)
    {
        data.AddPurchasedItem(type, racerId, customId);
    }

    public static bool IsUnlockingRacer(int id)
    {
        return IsReadyToUnlock(GetRacer(id));
    }

    private static bool IsReadyToUnlock(RacerProfile rp)
    {
        if (rp == null || rp.unlock == 1) return false;
        var rc = RacerFactory.Racer.GetConfig(rp.id);
        return rc.CardCount <= rp.cards;
    }

    public static bool UnlockRacer(int id)
    {
        var rp = GetRacer(id);
        if (rp == null) return false;
        var rc = RacerFactory.Racer.GetConfig(id);
        if (rc.CardCount <= rp.cards) rp.unlock = 1;
        return rp.unlock == 1;
    }

    public static bool IsUnlockedRacer(int id)
    {
        var rp = GetRacer(id);
        if (rp == null) return false;
        var rc = RacerFactory.Racer.GetConfig(id);
        return rc.CardCount <= rp.cards;
    }

    public static bool IsUnlockedCustome(RacerCustomeType type, int racerId, int customId)
    {
        if (customId == 0) return true;
        var sku = RacerProfile.GetCustomeSKU(type, racerId, customId);
        return IsPurchased(sku);
    }

    public static bool IsPurchased(string sku)
    {
        return data.purchasedItems.Exists(x => x == sku);
    }

    public static void ResetData(int modified = 1)
    {
        data.data = new ProfileData.NetData();
        data.modified = modified;
    }
}
