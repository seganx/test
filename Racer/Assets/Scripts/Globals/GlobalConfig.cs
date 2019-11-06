using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Market : int
{
    Null = 0,
    Irancell = 1,
    Cafebazaar = 2,
    GoogleplayPersian = 2,
    GoogleplayEnglish = 3,
}

public class GlobalConfig : StaticConfig<GlobalConfig>
{
#if UNITY_EDITOR
    [ContextMenu("Clone Data")]
    public void CloneData()
    {
        //Instance.data.shop[0].CloneData();
    }
#endif

    [System.Serializable]
    public class Data
    {
        [System.Serializable]
        public class Server
        {
            public string address = string.Empty;
            public int requestTimeout = 10;
            public int getTopPlayersCount = 20;
            public bool debug = true;
        }

        [System.Serializable]
        public class Photon
        {
            public string address = string.Empty;
            public string name = "rc";
            public string version = "3";
        }

        [System.Serializable]
        public class MatchMaking
        {
            public int[] joinTimeouts;
            public int eloScoreCount = 8;
            public int giftRacerCount = 15;
            public Vector2 eloScoreParams = new Vector2(0.5f, -500);
            public Vector2 eloPowerParams = new Vector2(0, 1000);
            public Vector2 eloGroupParams = new Vector2(0, 0);
            public string eloSqlLobby = "C0 = \"{0}\" AND C1 >= {1} AND C1 <= {2} AND C2 >= {3} AND C2 <= {4} AND C3 >= {5} AND C3 <= {6}";
        }

        [System.Serializable]
        public class Socials
        {
            public string storeUrl = string.Empty;
            public string rateUrl = string.Empty;
            public string instagramUrl = string.Empty;
            public string telegramUrl = string.Empty;
            public string contactSurveyUrl = string.Empty;
            public string contactTelegramUrl = string.Empty;
            public string contactEmailUrl = string.Empty;
        }

        [System.Serializable]
        public class Recharge
        {
            public int count = 5;
            public int time = 10;
        }

        [System.Serializable]
        public class Race
        {
            [System.Serializable]
            public class Bots
            {
                public float rayDistance = -14;
                public float raySpeedFactor = 0.75f;
                public int crashChance = 10;
                public int canVirajChance = 30;
                public int doVirajChance = 30;
                public int chatLeaveChance = 30;
                public int chatChance = 60;
                public Vector2Int powerRange = new Vector2Int(-300, 400);
                public Vector2[] powers = new Vector2[] { Vector2.one, Vector2.one, Vector2.one, Vector2.one, Vector2.one, Vector2.one, Vector2.one };
            }

            [System.Serializable]
            public class Traffics
            {
                public float carsSpeed = 20;
                public float startDistance = 256;
                public float baseDistance = -10;
                public float distanceVariance = 0;
                public float speedFactor = 1;
                public float roadWidthFactor = 0.55f;
                public float positionVariance = 0.5f;
                public int doubleCarChance = 0;
            }

            [System.Serializable]
            public class Rewards
            {
                public int gems = 0;
                public int coins = 0;
                public int gemChance = 0;
                public int racerCardChance = 0;
                public int customeChance = 0;
                public int cardLootFactor = 100;
            }

            public float maxTime = 60;
            public float startSpeed = 50;
            public float startRacerDistance = 1.6f;
            public float startNosFactor = 0.25f;
            public float nosTrafficMinDistance = 1;
            public float nosTrafficMaxDistance = 2;
            public float nosTrafficFactor = 0.03f;
            public float nosTimeFactor = 0.025f;
            public int nosBonusWidth = 40;
            public int nosBonusChance = 100;
            public int[] positionScore = new int[] { 10, 5, 1, -1 };
            public float[] groupMaxSpeed = new float[] { 100 };
            public Bots bots = new Bots();
            public Traffics traffics = new Traffics();
            public List<Rewards> rewardsOnline = new List<Rewards>();
            public List<Rewards> rewardsOffline = new List<Rewards>();
            public float GetGroupMaxSpeed(int groupIndex) { return groupMaxSpeed[Mathf.Clamp(groupIndex, 0, groupMaxSpeed.Length - 1)]; }
        }

        [System.Serializable]
        public class League
        {
            public int startScore = 0;
            public int startRank = 0;
            public int startGroup = 0;
            public int rewardGem = 0;
            public int rewardCoin = 0;
            public int rewardCards = 0;
            public Vector2Int cardsGroups = new Vector2Int(0, 20);
            public Vector3Int lootboxRacerIds = Vector3Int.zero;
            public int lootboxValue = 10;
        }

        [System.Serializable]
        public class Shop
        {
            [System.Serializable]
            public class LoadingBox
            {
                public int gemPrice = 0;
                public int nextTime = 0;
                public int dailyCount = 0;
                public int cardChance = 50;
                public List<int> gemValues = new List<int>();
                public List<int> coinValues = new List<int>();
                public Vector2Int cardsGroups = new Vector2Int(0, 20);
            }

            [System.Serializable]
            public class SpecialPackage
            {
                public string sku = string.Empty;
                public int price = 0;
                public int realPrice = 0;
                public int gem = 0;
                public int coin = 0;
                public int customes = 0;
                public int discount = 0;
                public Vector3Int racerIds = new Vector3Int(0, 0, 0);
            }

            [System.Serializable]
            public class CombinedPackage
            {
                public int coin = 0;
                public int coinDiscount = 0;
                public int coinBackgroundIndex = 0;
                public int customes = 0;
                public int racers = 4;
                public int racersCenter = 5;
                public int racersRaduis = 5;
                public int price = 200;
            }

            [System.Serializable]
            public class GemPackage
            {
                public string sku = string.Empty;
                public int price = 0;
                public int gems = 0;
                public int discount = 0;
            }

            [System.Serializable]
            public class CoinPackage
            {
                public int price = 0;
                public int coins = 0;
                public int discount = 0;
            }

            [System.Serializable]
            public class RacerCosts
            {
                [System.Serializable]
                public class CustomCost
                {
                    public int count = 5;
                    public float baseCostFactor = 0.07f;
                    public float costRatio = 0.02f;
                }

                [Header("Custom Costs")]
                public CustomCost hood = new CustomCost();
                public CustomCost roof = new CustomCost();
                public CustomCost spoiler = new CustomCost();
                public CustomCost vinyl = new CustomCost();
                public CustomCost wheel = new CustomCost();

                [Header("Color Cost Ratios")]
                public float bodyColorCostRatio = 0.03f;
                public float windowColorCostRatio = 0.02f;
                public float lightColorCostRatio = 0.01f;

                [Header("Upgrade Cost Ratios")]
                public float speedUpgradeCostRatio = 0.05f;
                public float nitroUpgradeCostRatio = 0.05f;
                public float steeringUpgradeCostRatio = 0.03f;
                public float bodyUpgradeCostRatio = 0.04f;

                [Space()]
                public float[] upgradeCostsRatio = new float[] { 1, 1.630435f, 2.608696f, 3.913043f, 5.652174f, 7.913043f, 11.08696f, 15.5f, 21.71739f, 30.3913f, 42.56522f, 59.56522f, 97.82609f };
            }

            [System.Serializable]
            public class BlackMarket
            {
                [System.Serializable]
                public class InterestedSearch
                {
                    public int minCandidateCount = 3;
                    public int[] minReqCards = new int[] { 90, 90, 90, 10, 10, 20, 20 };
                }

                [System.Serializable]
                public class SuggestionSearch
                {
                    public List<int> bestRacerIds = new List<int>();
                    public int[] maxReqCards = new int[] { 90, 90, 90, 10, 10, 20, 20 };
                }

                [System.Serializable]
                public class Package
                {
                    public int maxCount = 5;
                    public float basePriceFactor = 0.05f;
                    public float priceRatio = 0.015f;
                }

                public int refreshTime = 21600;
                public int[] refreshPrices = new int[] { 200, 80, 20 };
                public List<Package> packages = new List<Package>();
                public InterestedSearch interestedSearch = new InterestedSearch();
                public SuggestionSearch suggestionSearch = new SuggestionSearch();
            }

            public int gemToTime = 90;
            public int gemToCoin = 200;
            public int instaToGem = 100;
            public int nicknamePrice = 1200;
            public float inventorySellFactor = 2.0f;
            public int leagueSpecialPackagesNextTime = 172800;
            public BlackMarket blackMarket = new BlackMarket();
            public List<LoadingBox> loadingBoxPackage = new List<LoadingBox>();
            public List<CombinedPackage> combinedPackages = new List<CombinedPackage>();
            public List<SpecialPackage> leagueSpecialPackages = new List<SpecialPackage>();
            public List<GemPackage> gemPackages = new List<GemPackage>();
            public RacerCosts racerCosts = new RacerCosts();

            /*
            public void CloneData()
            {
                leagueSpecialPackages = new List<SpecialPackage>();
                for (int i = 0; i < oldPackages.Count; i++)
                {
                    SpecialPackage sp = new SpecialPackage();
                    SpecialPackage item = oldPackages[i];
                    sp.coin = item.coin;
                    sp.customes = item.customes;
                    sp.discount = item.discount;
                    sp.gem = item.gem;
                    sp.price = item.price;
                    sp.racerIds = item.racerIds;
                    sp.realPrice = item.realPrice;
                    sp.sku = item.sku;
                    leagueSpecialPackages.Add(sp);
                }
            }
            */
        }

        [System.Serializable]
        public class ProfilePreset
        {
            public int gems = 100;
            public int coins = 500;
            public int racerId = 10;
            public int rndCards = 5;
        }

        [System.Serializable]
        public class Update
        {
            public bool whole = false;
            public bool shop = false;
            public bool league = false;
        }

        [System.Serializable]
        public class Probabilities
        {
            public int rewardRacerRadius = 3;
            public int blackmarketRacerRadius = 5;
        }

        [System.Serializable]
        public class Chat
        {
            public enum Position : int { _1th, _2th, _3th, _4th }
            [PersianPreview]
            public string text = string.Empty;
            [EnumFlag]
            public Position botPosition = 0;
            public bool IsPositionMatch(int position) { return botPosition.IsFlagOn(1 << (position - 1)); }
        }

        public Update forceUpdate = new Update();
        public Server server = new Server();
        public Photon photon = new Photon();
        public MatchMaking matchMaking = new MatchMaking();
        public Socials socials = new Socials();
        public Recharge recharg = new Recharge();
        public Race race = new Race();
        public List<League> leagues = new List<League>();
        public List<Shop> shop = new List<Shop>();
        public Probabilities probabilities = new Probabilities();
        public List<ProfilePreset> profilePreset = new List<ProfilePreset>() { new ProfilePreset() };
        public List<Chat> chatList = new List<Chat>();
    }

    public int gamekey = 0;
    public Market market = 0;
    public string address = "http://seganx.com/development/racer/";
    public string cafeBazaarKey = "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDkPD34tNo67rREJH1ShHePHSR8fNTm0cFgYl1fJl6MWautFfu5lZzV9+ieWtPinm//94JifZVoEkHbjjjUBu0QDFnvyo6cZorzIkyfj8sui/xbpnwOCVnOoWnsF0KSiRCbI1XzUQEYHQDLTOkMFgyecTp/+GVCjX8yZCko4jVVF+2Ne82tb58FaL3dgBaih5Lhtql904sF9gpu7DfaLescCr4jh8/YrtzIdVXAcs8CAwEAAQ==";

    [Header("Dynamic Data")]
    [SerializeField] private Data data = new Data();

    protected override void OnInitialize()
    {
#if UNITY_EDITOR
        if (offline)
        {
            SaveData(data);
            Cohort = offlineCohort;
        }
        else
#endif
            data = LoadData(data);

        if (DebugMode) SeganX.Console.Logger.Enabled = true;
    }

#if UNITY_EDITOR
    [Space()]
    [InspectorButton(100, "Export as", "OnExport")]
    public bool offline = false;
    public int offlineCohort = 0;

    public void OnExport(object sender)
    {
        var filename = UnityEditor.EditorUtility.SaveFilePanel("Save exported data", System.IO.Path.GetDirectoryName(Application.dataPath), "config", "txt");
        if (filename.HasContent(4))
            System.IO.File.WriteAllText(filename, JsonUtility.ToJson(data, false), System.Text.Encoding.UTF8);
    }
#endif

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static Data.Update ForceUpdate { get { return Instance.data.forceUpdate; } }
    public static Data.Server Server { get { return Instance.data.server; } }
    public static Data.Photon Photon { get { return Instance.data.photon; } }
    public static Data.MatchMaking MatchMaking { get { return Instance.data.matchMaking; } }
    public static Data.Recharge Recharg { get { return Instance.data.recharg; } }
    public static Data.Race Race { get { return Instance.data.race; } }
    public static Data.Socials Socials { get { return Instance.data.socials; } }
    public static Data.Shop Shop { get { return Instance.data.shop[Cohort % Instance.data.shop.Count]; } }
    public static List<Data.ProfilePreset> ProfilePresets { get { return Instance.data.profilePreset; } }
    public static Data.Probabilities Probabilities { get { return Instance.data.probabilities; } }
    public static List<Data.Chat> Chats { get { return Instance.data.chatList; } }

    private static int Cohort { get; set; }

    public static bool DebugMode
    {
        get { return PlayerPrefsEx.GetInt("GlobalConfigs.DebugMode", 0) > 0; }
        set
        {
            PlayerPrefsEx.SetInt("GlobalConfigs.DebugMode", value ? 1 : 0);
            SeganX.Console.Logger.Enabled = value;
        }
    }

    public static bool SetData(string json)
    {
#if UNITY_EDITOR
        if (Instance.offline)
        {
            //SeganX.Console.Logger.Enabled = Server.debug;
            return true;
        }
#endif

        var newdata = JsonUtility.FromJson<Data>(json);
        if (newdata == null) return false;

        //  set new data
        Instance.data = newdata;
        SaveData(newdata);
        SeganX.Console.Logger.Enabled = DebugMode || Server.debug;

        // select cohort
        Cohort = PlayerPrefsEx.GetInt("GlobalConfig.Cohort", -1);
        if (Cohort < 0)
        {
            Cohort = Random.Range(0, 100) % Instance.data.shop.Count;
            PlayerPrefsEx.SetInt("GlobalConfig.Cohort", Cohort);
        }
        return true;
    }

    public static string GetChat(int index)
    {
        return Chats[index % Chats.Count].text;
    }

    public static class Leagues
    {
        public static List<Data.League> list { get { return Instance.data.leagues; } }

        public static Data.League GetByIndex(int index)
        {
            index = Mathf.Clamp(index, 0, list.Count);
            return list[index];
        }

        public static int GetIndex(int score, long position)
        {
            if (position < 1) position = int.MaxValue;
            int res = 0;
            for (int i = 0; i < list.Count; i++)
            {
                long startpos = list[i].startRank == 0 ? long.MaxValue : list[i].startRank;
                if (score >= list[i].startScore && position <= startpos)
                    res = i;
            }
            return res;
        }
    }

}
