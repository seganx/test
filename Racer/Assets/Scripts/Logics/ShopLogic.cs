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
            if (Profile.IsUnlockedRacer(pack.racerId)) pack.racerIndex = ++pack.racerIndex % 3;
            if (Profile.IsUnlockedRacer(pack.racerId)) pack.racerIndex = ++pack.racerIndex % 3;
            if (Profile.IsUnlockedRacer(pack.racerId)) return;

            Packages.Add(pack);
            SetTimer(index);
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

    public static class SpecialRacerPopup
    {
        private static bool PopupWasDisplayed = false;
        private static bool PopupWasUsed = false;
        private static GlobalConfig.Data.Shop.SpecialPackage CurrentPackage = null;

        public static bool IsAvailable { get { return PopupWasUsed == false && CurrentPackage != null && CurrentPackage.price > 0; } }
        public static bool AutoDisplay { get { return PopupWasUsed == false && PopupWasDisplayed == false; } }

        public static GlobalConfig.Data.Shop.SpecialPackage Package
        {
            get { return PopupWasUsed || CurrentPackage == null || CurrentPackage.price < 1 ? null : CurrentPackage; }
        }

        public static void Display(System.Action onPurchase)
        {
            if (Package == null) return;
            PopupWasDisplayed = true;
            Game.Instance.OpenPopup<Popup_ShopSpecialPackage>().Setup(Package, onPurchase);
        }

        public static void TryToCreateNewPackage()
        {
#if OFF
            var pack = GlobalConfig.Shop.specialRacerCardPopup;
            if (pack.skus.IsNullOrEmpty() || pack.prices.Length < 1 || Profile.SelectedRacer < 1) return;

            var centerId = RacerFactory.Racer.AllConfigs[RewardLogic.FindSelectRacerCenter()].Id;
            var list = Profile.Data.racers.FindAll(x => x.id >= centerId && Profile.IsUnlockedRacer(x.id) == false);
            if (list.Count < 1) return;

            var racerprofile = list.FindMax(x => x.cards);
            var config = RacerFactory.Racer.GetConfig(racerprofile.id);
            int racerCardsCount = config.CardCount;
            int condition = racerCardsCount * pack.minCardFactor / 100;
            if (racerprofile.cards < condition) return;

            CurrentPackage = new GlobalConfig.Data.Shop.SpecialRacerCardPackage();
            CurrentPackage.cardCount = racerCardsCount;
            CurrentPackage.discount = pack.discount;
            CurrentPackage.price = pack.prices[Mathf.Clamp(config.GroupId - 1, 0, pack.prices.LastIndex())];
            CurrentPackage.sku = pack.skus[Mathf.Clamp(config.GroupId - 1, 0, pack.skus.LastIndex())];
            CurrentPackage.racerId = racerprofile.id;

            TimerManager.SetTimer(TimerManager.Type.RacerSpecialOfferTimer, pack.durationTime);
#endif
            Save();
        }

        public static void Save()
        {
            PlayerPrefsEx.Serialize("Shop.SpecialRacerCardPopup", CurrentPackage);
        }

        public static void Load()
        {
            var res = PlayerPrefsEx.Deserialize<GlobalConfig.Data.Shop.SpecialPackage>("Shop.SpecialRacerCardPopup", null);
            CurrentPackage = (res != null && res.price > 0) ? res : null;
        }

        public static void Clear()
        {
            //PopupWasUsed = true;
            CurrentPackage = new GlobalConfig.Data.Shop.SpecialPackage();
            Save();
        }
    }
}
