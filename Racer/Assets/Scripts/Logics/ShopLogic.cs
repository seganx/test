using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    //////////////////////////////////////////////////////////
    /// STATIC MEMBER
    //////////////////////////////////////////////////////////
    public static int GetCustomPackagePrice(int unlockedCount, GlobalConfig.Data.Shop.RacerCosts.CustomCost cc, RacerConfig rc)
    {
        var count = unlockedCount / cc.count;
        var basePrice = rc.Price * cc.baseCostFactor;
        var priceRatio = rc.Price * cc.costRatio;
        return Mathf.RoundToInt(basePrice + priceRatio * count);
    }


    //////////////////////////////////////////////////////////
    /// SPECIAL OFFER
    //////////////////////////////////////////////////////////
    public static class SpecialOffer
    {
        [System.Serializable]
        public class Package
        {
            public int packgIndex = 0;
            public int racerIndex = 0;

            public GlobalConfig.Data.Shop.SpecialPackage item { get; set; }
            public int racerId { get { return item.racerIds[racerIndex]; } }
        }

        [System.Serializable]
        private class SerializableData
        {
            public int lastRacerId = 0;
            public int lastIndex = -1;
            public List<Package> packages = new List<Package>();
        }

        private static SerializableData data = new SerializableData();

        public static List<Package> Packages { get { return data.packages; } }

        private static int CurrIndex
        {
            get
            {
                var baseIndex = Mathf.Min(Profile.League, 4);
                var leagueStartScore = GlobalConfig.Leagues.GetByIndex(baseIndex).startScore;
                var leagueEndScore = GlobalConfig.Leagues.GetByIndex(baseIndex + 1).startScore;
                if (leagueEndScore - leagueStartScore < 100) leagueEndScore += 500;
                var leagueMidScore = (leagueEndScore + leagueStartScore) / 2;
                var midlIndex = (Profile.Score > leagueMidScore) ? 1 : 0;
                return baseIndex * 2 + midlIndex;
            }
        }

        public static void Refresh()
        {
            const string key = "ShopLogic.SpecialOffer.Data";

            //  load data
            data = PlayerPrefsEx.Deserialize(key, data);
            foreach (var pack in data.packages)
                pack.item = GlobalConfig.Shop.leagueSpecialPackages[pack.packgIndex];

            // remove unused items
            data.packages.RemoveAll(x => CanDisplay(x.packgIndex) == false);

            // add new items
            if (CurrIndex != data.lastIndex)
            {
                data.lastIndex = CurrIndex;
                CreatePackage(data.lastIndex);
            }

            // save data
            PlayerPrefsEx.Serialize(key, data);
        }

        private static void CreatePackage(int index)
        {
            if (Packages.Exists(x => x.packgIndex == index)) return;

            var pack = new Package()
            {
                packgIndex = index,
                racerIndex = Random.Range(0, 100) % 3,
                item = GlobalConfig.Shop.leagueSpecialPackages[index]
            };

            //  verify that selected racer is locked
            if (data.lastRacerId == pack.racerId || Profile.IsUnlockedRacer(pack.racerId)) pack.racerIndex = ++pack.racerIndex % 3;
            if (data.lastRacerId == pack.racerId || Profile.IsUnlockedRacer(pack.racerId)) pack.racerIndex = ++pack.racerIndex % 3;
            if (data.lastRacerId == pack.racerId || Profile.IsUnlockedRacer(pack.racerId)) return;
            data.lastRacerId = pack.racerId;

            Packages.Add(pack);
            SetTimer(index);
        }

        public static bool CanDisplay(Package pack)
        {
            if (pack == null) return false;
            int index = Packages.IndexOf(pack);
            if (index < 0 || Profile.IsUnlockedRacer(pack.racerId) || GetRemainedTime(index) < 1)
            {
                if (index == data.lastIndex)
                    data.lastIndex = -1;
                return false;
            }
            return true;
        }

        public static bool CanDisplay(int index)
        {
            var pack = Packages.Find(x => x.packgIndex == index);
            if (pack == null) return false;
            if (Profile.IsUnlockedRacer(pack.racerId) || GetRemainedTime(index) < 1)
            {
                if (index == data.lastIndex)
                    data.lastIndex = -1;
                return false;
            }
            return true;
        }

        public static TimerManager.Type GetTimerType(int index)
        {
            switch (index)
            {
                case 0: return TimerManager.Type.ShopSpecialPackage0;
                case 1: return TimerManager.Type.ShopSpecialPackage1;
                case 2: return TimerManager.Type.ShopSpecialPackage2;
                case 3: return TimerManager.Type.ShopSpecialPackage3;
                case 4: return TimerManager.Type.ShopSpecialPackage4;
                case 5: return TimerManager.Type.ShopSpecialPackage5;
                case 6: return TimerManager.Type.ShopSpecialPackage6;
                case 7: return TimerManager.Type.ShopSpecialPackage7;
                case 8: return TimerManager.Type.ShopSpecialPackage8;
                case 9: return TimerManager.Type.ShopSpecialPackage9;
            }
            return TimerManager.Type.ShopSpecialPackage0;
        }

        private static void SetTimer(int index)
        {
            TimerManager.SetTimer(GetTimerType(index), GlobalConfig.Shop.leagueSpecialPackagesNextTime);
        }

        private static int GetRemainedTime(int index)
        {
            return TimerManager.GetRemainTime(GetTimerType(index));
        }
    }
}
