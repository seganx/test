using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFactory : StaticConfig<GlobalFactory>
{
    [System.Serializable]
    public class LocalTextAsset
    {
        public TextAsset file_fa = null;
        public TextAsset file_en = null;
        public string Text { get { return LocalizationService.IsPersian ? file_fa.text : file_en.text; } }
    }

    [System.Serializable]
    public class PlayersName : LocalTextAsset
    {
        public List<string> names = new List<string>();
        public int range = 100;
        public int index = 0;
    }

    [System.Serializable]
    public class LeagueInfo
    {
        public Sprite bigIcon = null;
        public Sprite smallIcon = null;
    }

    [SerializeField] private UiRacerCard racerCardPrefab = null;
    [SerializeField] private PlayersName playersName = new PlayersName();
    [SerializeField] private List<LeagueInfo> leagues = new List<LeagueInfo>();

    protected override void OnInitialize()
    {
        try
        {
            playersName.names.AddRange(playersName.Text.Replace("\r", "").Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
            playersName.names.RemoveAll(x => x.Length < 3);
            playersName.names.Sort((x, y) => Random.Range(-10, 11));
            playersName.range = playersName.names.Count / 6;
        }
        catch { }
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class ProbabilityCurves
    {
        [InspectorButton(100, "Test Probability", "TestRacerProbability")]
        public int count = 100;
        public int lenght = 30;
        public int center = 10;
    }

    [SerializeField] private ProbabilityCurves probabilities = new ProbabilityCurves();
    private static List<Transform> list = null;
    public void TestRacerProbability(object sender)
    {
        if (list != null)
            foreach (var item in list)
                DestroyImmediate(item.gameObject);

        list = new List<Transform>(probabilities.count);
        for (int i = 0; i < probabilities.count; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var pr = RewardLogic.SelectProbability(probabilities.lenght, probabilities.center, GlobalConfig.Probabilities.rewardRacerRadius);
            go.transform.position = new Vector3(pr, list.FindAll(x => Mathf.Approximately(x.position.x, pr)).Count, 0);
            list.Add(go.transform);
        }
    }
#endif



    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static string GetRandomName()
    {
        var playersName = Instance.playersName;
        var res = playersName.names[Random.Range(playersName.index, playersName.index + playersName.range) % playersName.names.Count];
        playersName.index += playersName.range;
        return res;
    }

    public static UiRacerCard CreateRacerCard(int racerId, Transform parent)
    {
        var res = Instance.racerCardPrefab.Clone<UiRacerCard>(parent).Setup(racerId);
        res.transform.SetAnchordPosition(Vector3.zero);
        return res;
    }

    public static class League
    {
        public static Sprite GetBigIcon(int index)
        {
            index = Mathf.Clamp(index, 0, Instance.leagues.Count - 1);
            return Instance.leagues[index].bigIcon;
        }

        public static Sprite GetSmallIcon(int index)
        {
            index = Mathf.Clamp(index, 0, Instance.leagues.Count - 1);
            return Instance.leagues[index].smallIcon;
        }

        public static string GetName(int index)
        {
            index = Mathf.Clamp(index, 0, 4);
            return LocalizationService.Get(111050 + index);
        }
    }

    public static class TrafficCars
    {
        private static string dire = "TrafficCars";
        private static int count = 0;
        private static List<TrafficCar> prefabs = null;
        private static List<Transform> pool = null;

        public static int Count
        {
            get
            {
                if (count == 0)
                    count = ResourceEx.LoadAll(dire, true).Count;
                return count;
            }
        }

        public static void CreatePool()
        {
            pool = new List<Transform>(10);
            prefabs = ResourceEx.LoadAll<TrafficCar>(dire, true);
            foreach (var item in prefabs)
            {
                var obj = item.Clone<TrafficCar>();
                obj.transform.position = Vector3.back * 20;
                pool.Add(obj.transform);
            }
        }

        public static void ReleasePool()
        {
            foreach (var item in pool)
                if (item != null)
                    Destroy(item.gameObject);
            pool = null;
            prefabs = null;
        }

        public static TrafficCar Create(int id, float line, float ditanceVariance, Transform parent)
        {
            return prefabs[id % Count].Clone<TrafficCar>(parent).Setup(id, line, ditanceVariance);
        }
    }

}
