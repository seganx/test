using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    public static class SpecialRacerPopup
    {
        private static bool PopupWasDisplayed = false;
        private static bool PopupWasUsed = false;
        private static GlobalConfig.Data.Shop.SpecialRacerCardPackage CurrentPackage = null;

        public static bool IsAvailable { get { return PopupWasUsed == false && CurrentPackage != null; } }
        public static bool AutoDisplay { get { return PopupWasUsed == false && PopupWasDisplayed == false; } }

        public static GlobalConfig.Data.Shop.SpecialRacerCardPackage Package
        {
            get { return PopupWasUsed ? null : CurrentPackage; }
        }

        public static void Display(System.Action onPurchase)
        {
            if (Package == null) return;
            PopupWasDisplayed = true;
            Game.Instance.OpenPopup<Popup_ShopSpecialRacer>().Setup(Package, onPurchase);
        }

        public static void TryToCreateNewPackage()
        {
            var pack = GlobalConfig.Shop.specialRacerCardPopup;
            if (pack.sku.IsNullOrEmpty() || pack.price < 1 || Profile.SelectedRacer < 1) return;

            var centerId = RacerFactory.Racer.AllConfigs[RewardLogic.FindSelectRacerCenter()].Id;
            var list = Profile.data.racers.FindAll(x => x.id >= centerId && Profile.IsUnlockedRacer(x.id) == false);
            if (list.Count < 1) return;

            var racerprofile = list.FindMax(x => x.cards);
            int racerCardsCount = RacerFactory.Racer.GetConfig(racerprofile.id).CardCount;
            int condition = racerCardsCount * pack.minCardFactor / 100;
            if (racerprofile.cards < condition) return;

            CurrentPackage = new GlobalConfig.Data.Shop.SpecialRacerCardPackage();
            CurrentPackage.cardCount = racerCardsCount;
            CurrentPackage.discount = pack.discount;
            CurrentPackage.price = pack.price;
            CurrentPackage.sku = pack.sku;
            CurrentPackage.racerId = racerprofile.id;

            TimerManager.SetTimer(TimerManager.Type.RacerSpecialOfferTimer, pack.durationTime);

            Save();
        }

        public static void Save()
        {
            PlayerPrefsEx.Serialize("Shop.SpecialRacerCardPopup", CurrentPackage);
        }

        public static void Load()
        {
            var res = PlayerPrefsEx.Deserialize<GlobalConfig.Data.Shop.SpecialRacerCardPackage>("Shop.SpecialRacerCardPopup", null);
            CurrentPackage = (res != null && res.price > 0) ? res : null;
        }

        public static void Clear()
        {
            //PopupWasUsed = true;
            CurrentPackage = new GlobalConfig.Data.Shop.SpecialRacerCardPackage();
            Save();
        }
    }
}
