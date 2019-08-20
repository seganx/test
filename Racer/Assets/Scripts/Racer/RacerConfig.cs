using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Racer")]
public class RacerConfig : ScriptableObject
{
    [SpritePreview(100)]
    public Sprite icon = null;
    public Sprite halfIcon = null;

    [Header("Defaults:")]
    public int defaultWheelId = 1;
    public int defaultBodyColorId = 1;


    [Header("Accessories:")]
    public string hoods = string.Empty;
    public string horns = string.Empty;
    public string roofs = string.Empty;
    public string spoilers = string.Empty;
    public string vinyls = string.Empty;
    public string wheels = string.Empty;

    private RacerGlobalConfigs.Racer data = null;

    public int Id { get; set; }
    public string Name { get { return data.name; } }
    public int GroupId { get { return data.groupId; } }
    public int CardCount { get { return data.cardCount; } }

    public RacerCustomData DefaultRacerCustom
    {
        get
        {
            return new RacerCustomData()
            {
                Wheel = defaultWheelId,
                BodyColor = defaultBodyColorId,
                RoofColor = defaultBodyColorId,
                SpoilerColor = defaultBodyColorId,
                HoodColor = defaultBodyColorId,
                RimColor = 10,
                VinylColor = 10,
                WindowColor = 70,
                LightsColor = 70,
                Height = 1,
                ColorModel = 1
            };
        }
    }

    public int Price
    {
        get { return data.price; }
    }

    public int MaxUpgradeLevel
    {
        get { return RacerGlobalConfigs.Data.maxUpgradeLevel[0]; }
    }

    public int MinPower
    {
        get { return ComputePower(0, 0, 0, 0); }
    }

    public int MaxPower
    {
        get { return ComputePower(MaxUpgradeLevel, MaxUpgradeLevel, MaxUpgradeLevel, MaxUpgradeLevel); }
    }

    public int BodyColorCost
    {
        get { return Mathf.RoundToInt(data.price * GlobalConfig.Shop.racerCosts.bodyColorCostRatio); }
    }

    public int WindowColorCost
    {
        get { return Mathf.RoundToInt(data.price * GlobalConfig.Shop.racerCosts.windowColorCostRatio); }
    }

    public int LightsColorCost
    {
        get { return Mathf.RoundToInt(data.price * GlobalConfig.Shop.racerCosts.lightColorCostRatio); }
    }

    public int UpgradeCostSpeed(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return Mathf.RoundToInt(data.price * GlobalConfig.Shop.gemToCoin * GlobalConfig.Shop.racerCosts.speedUpgradeCostRatio * GlobalConfig.Shop.racerCosts.upgradeCostsRatio[upgradeLevel]);
    }

    public int UpgradeCostNitro(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return Mathf.RoundToInt(data.price * GlobalConfig.Shop.gemToCoin * GlobalConfig.Shop.racerCosts.nitroUpgradeCostRatio * GlobalConfig.Shop.racerCosts.upgradeCostsRatio[upgradeLevel]);
    }

    public int UpgradeCostSteering(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return Mathf.RoundToInt(data.price * GlobalConfig.Shop.gemToCoin * GlobalConfig.Shop.racerCosts.steeringUpgradeCostRatio * GlobalConfig.Shop.racerCosts.upgradeCostsRatio[upgradeLevel]);
    }

    public int UpgradeCostBody(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return Mathf.RoundToInt(data.price * GlobalConfig.Shop.gemToCoin * GlobalConfig.Shop.racerCosts.bodyUpgradeCostRatio * GlobalConfig.Shop.racerCosts.upgradeCostsRatio[upgradeLevel]);
    }

    public float ComputeSpeed(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return data.speedBaseValue + RacerGlobalConfigs.Data.speedUpgradeValue[upgradeLevel];
    }

    public float ComputeNitro(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return data.nitroBaseValue + RacerGlobalConfigs.Data.nitroUpgradeValue[upgradeLevel];
    }

    public float ComputeSteering(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return data.steeringBaseValue + RacerGlobalConfigs.Data.steeringUpgradeValue[upgradeLevel];
    }

    public float ComputeBody(int upgradeLevel)
    {
        upgradeLevel = Mathf.Clamp(upgradeLevel, 0, MaxUpgradeLevel);
        return data.bodyBaseValue + RacerGlobalConfigs.Data.bodyUpgradeValue[upgradeLevel];
    }

    public int ComputePower(int speedLevel, int nitroLevel, int steeringLevel, int bodyLevel)
    {
        var res =
            ComputeSpeed(speedLevel) * RacerGlobalConfigs.Data.speedPowerRatio +
            ComputeNitro(nitroLevel) * RacerGlobalConfigs.Data.nitroPowerRatio +
            ComputeSteering(steeringLevel) * RacerGlobalConfigs.Data.steeringPowerRatio +
            ComputeBody(bodyLevel) * RacerGlobalConfigs.Data.bodyPowerRatio;
        return res.ToInt();
    }

    public void Awake()
    {
        Id = name.Split('_')[0].ToInt(-1);
        data = RacerGlobalConfigs.GetConfig(Id);
    }
}
