using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerGlobalConfigs : StaticConfig<RacerGlobalConfigs>
{
    [System.Serializable]
    public class Racer
    {
        public string name = string.Empty;
        public int id = 0;
        public int groupId = 0;
        public int cardCount = 0;
        public int price = 0;
        public float nitroBaseValue = 0;
        public float steeringBaseValue = 0;
        public float bodyBaseValue = 0;
    }

    [System.Serializable]
    public class ConfigData
    {
        public float nitroUpgradeRatio = 0;
        public float steeringUpgradeRatio = 0;
        public float bodyUpgradeRatio = 0;
        public float nitroPowerRatio = 0;
        public float steeringPowerRatio = 0;
        public float bodyPowerRatio = 0;
        public List<float> nitroUpgradeValue = new List<float>();
        public List<float> steeringUpgradeValue = new List<float>();
        public List<float> bodyUpgradeValue = new List<float>();
        public List<int> maxUpgradeLevel = new List<int> { 8, 5, 3 };
        public List<Racer> racers = new List<Racer>();

        public int TotalUpgradeLevels { get { return maxUpgradeLevel[0] + maxUpgradeLevel[1] + maxUpgradeLevel[3]; } }
    }

    public ConfigData data = new ConfigData();

    protected override void OnInitialize()
    {
        data = LoadData(data);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (data.maxUpgradeLevel.Count < 5)
            data.maxUpgradeLevel = new List<int>(5) { 8, 5, 3, 1, 1 };

        while (data.nitroUpgradeValue.Count < data.TotalUpgradeLevels)
            data.nitroUpgradeValue.Add(data.nitroUpgradeValue.Count);
        while (data.steeringUpgradeValue.Count < data.TotalUpgradeLevels)
            data.steeringUpgradeValue.Add(data.steeringUpgradeValue.Count);
        while (data.bodyUpgradeValue.Count < data.TotalUpgradeLevels)
            data.bodyUpgradeValue.Add(data.bodyUpgradeValue.Count);
    }
#endif

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static ConfigData Data { get { return Instance.data; } }

    public static void SetData(ConfigData newdata)
    {
        Instance.data = newdata;
        SaveData(newdata);
    }

    public static Racer GetConfig(int id)
    {
        return Instance.data.racers.Find(x => x.id == id);
    }


#if UNITY_EDITOR
    ////////////////////////////////////////////////////////////
    /// EDITOR MEMBERS
    ////////////////////////////////////////////////////////////
    public Vector3 editorGroupParam = Vector3.zero;
    public Vector3 editorCardsParam = Vector3.zero;
    public Vector3 editorPriceParam = Vector3.zero;
    public Vector3 editorNitroParam = Vector3.zero;
    public Vector3 editorSteeringParam = Vector3.zero;
    public Vector3 editorBodyParam = Vector3.zero;
#endif
}
