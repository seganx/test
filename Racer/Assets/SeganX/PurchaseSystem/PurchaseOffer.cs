using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static class PurchaseOffer
    {
        public const int currentTimerId = -200001;

        private const int coolTimerId = -100001;
        private const int resourceTimerId = -100002;
        private const int purchaseTimerId = -100003;

        private static class Data
        {
            public static int maxIndex = 2;
            public static int startIndex = 4;
            public static int coolTime = 24 * 60 * 60;
            public static int minResource = 2400;
            public static int resourceTime = 3 * 60 * 60;
            public static int lastPurchaseTime = 5 * 24 * 60 * 60;

            public static int Index
            {
                get { return PlayerPrefsEx.GetInt("PurchaseOffer.Data.Index", startIndex); }
                set { PlayerPrefsEx.SetInt("PurchaseOffer.Data.Index", value); }
            }
        }

        public static void Setup(int startIndex, int maxIndex, int cooltimeHours, int minResource, int minResourceHours, int lastPurchaseDays)
        {
            Data.maxIndex = maxIndex;
            Data.startIndex = startIndex;
            Data.coolTime = cooltimeHours * 60 * 60;
            Data.minResource = minResource;
            Data.resourceTime = minResourceHours * 60 * 60;
            Data.lastPurchaseTime = lastPurchaseDays * 24 * 60 * 60;
        }

        public static int GetOfferIndex(int resource)
        {
            if (IsTimeToShow(resource) == false)
                return -1;

            // check if offer exist
            if (Online.Timer.Exist(currentTimerId))
            {
                // current offer is still exist ?
                if (Online.Timer.GetRemainSeconds(currentTimerId, 48 * 60 * 60) > 0)
                    return Data.Index;

                // it seems that the player just did not purchased
                Online.Timer.Remove(currentTimerId);
                if (Data.Index == 0)
                {
                    Online.Timer.Remove(coolTimerId);
                    Online.Timer.Remove(resourceTimerId);
                    Online.Timer.Remove(purchaseTimerId);
                    return -1;
                }
                else
                {
                    Data.Index--;
                    return Data.Index;
                }
            }
            else return Data.Index;
        }

        public static void SetPurchaseResult(bool success)
        {
            if (success)
            {
                Online.Timer.Remove(coolTimerId);
                Online.Timer.Remove(resourceTimerId);
                Online.Timer.Set(purchaseTimerId, Data.lastPurchaseTime);

                if (Data.Index < Data.maxIndex)
                    Data.Index++;
            }
        }

        private static bool IsTimeToShow(int resource)
        {
            // check cool time
            if (Online.Timer.GetRemainSeconds(coolTimerId, Data.coolTime) > 0) return false;

            // check resource leaks
            if (Online.Timer.Exist(resourceTimerId))
            {
                if (Online.Timer.GetRemainSeconds(resourceTimerId, Data.resourceTime) <= 0)
                    return true;
            }
            else if (resource < Data.minResource)
            {
                Online.Timer.Set(resourceTimerId, Data.resourceTime);
            }

            // check last purchase
            if (Online.Timer.GetRemainSeconds(-100003, Data.lastPurchaseTime) <= 0)
                return true;

            return false;
        }
    }
}
