using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Game.Loaded);

    }

    public static int GetCustomPackagePrice(int unlockedCount, GlobalConfig.Data.Shop.RacerCosts.CustomCost cc, RacerConfig rc)
    {
        var count = unlockedCount / cc.count;
        var basePrice = rc.Price * cc.baseCostFactor;
        var priceRatio = rc.Price * cc.costRatio;
        return Mathf.RoundToInt(basePrice + priceRatio * count);
    }


    //////////////////////////////////////////////////////////
    /// POPUP SPECIAL OFFER
    //////////////////////////////////////////////////////////
    public static class SpecialOffer
    {
        public static GlobalConfig.Data.Shop.SpecialPackage Package = null;

        private static int LastIndex
        {
            get { return PlayerPrefsEx.GetInt("SpecialOffer.LastIndex", -1); }
            set { PlayerPrefsEx.SetInt("SpecialOffer.LastIndex", value); }
        }

        public static void SelectSpecialOffer()
        {
            var baseIndex = Mathf.Max(Profile.League, 4);
            var leagueStartScore = GlobalConfig.Leagues.GetByIndex(baseIndex).startScore;
            var leagueEndScore = GlobalConfig.Leagues.GetByIndex(baseIndex + 1).startScore;
            if (leagueEndScore - leagueStartScore < 100) leagueEndScore += 500;
            var leagueMidScore = (leagueEndScore - leagueStartScore) / 2;

            var midlIndex = (Profile.Score > leagueMidScore) ? 1 : 0;
            var index = baseIndex * 2 + midlIndex;

            if (index != LastIndex)
            {
                LastIndex = index;
                TimerManager.SetTimer(TimerManager.Type.CombinedShopItemTimer, GlobalConfig.Shop.combinedPackagesNextTime);
            }

            Package = GlobalConfig.Shop.combinedPackages[index];
            if (CanDisplay(0)) Package.PopupRacerId = Package.racerIds[0];
            else if (CanDisplay(1)) Package.PopupRacerId = Package.racerIds[1];
            else if (CanDisplay(2)) Package.PopupRacerId = Package.racerIds[2];
            else Package.PopupRacerId = 0;

        }

        public static bool CanDisplay(int index)
        {
            if (Package == null) return false;
            var racerId = Package.racerIds[index];
            return Profile.IsUnlockedRacer(racerId) == false;
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
            Game.Instance.OpenPopup<Popup_ShopSpecialRacer>().Setup(Package, onPurchase);
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
