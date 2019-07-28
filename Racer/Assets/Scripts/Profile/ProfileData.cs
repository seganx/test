using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfileData
{
    public string key = string.Empty;
    public string userId = string.Empty;
    public string nickName = string.Empty;
    public CryptoInt position = 0;
    public CryptoInt score = 0;
    public CryptoInt rewardedPosition = 0;
    public CryptoInt rewardedScore = 0;
    public NetData data = new NetData();

    public int version { get { return data.ver; } set { data.ver = value; } }
    public int modified { get { return data.mod; } set { data.mod = value; } }
    public int gem { get { return data.i1.Value; } set { data.i1 = value; } }
    public int gold { get { return data.i2.Value; } set { data.i2 = value; } }
    public int selectedRacer { get { return data.i3.Value; } set { data.i3 = value; } }
    public int socials { get { return data.i4.Value; } set { data.i4 = value; } }
    public int skill { get { return data.i5.Value; } set { data.i5 = value; } }
    public List<string> purchasedItems { get { return data.a1; } set { data.a1 = value; } }
    public List<RacerProfile> racers { get { return data.a2; } set { data.a2 = value; } }

    public ProfileData AddRacer(RacerProfile racer)
    {
        if (racers.Exists(x => x.id == racer.id)) return this;
        racers.Add(racer);
        AddPurchasedItem(RacerCustomeType.Height, racer.id, racer.custom.Height);
        AddPurchasedItem(RacerCustomeType.Hood, racer.id, racer.custom.Hood);
        AddPurchasedItem(RacerCustomeType.Horn, racer.id, racer.custom.Horn);
        AddPurchasedItem(RacerCustomeType.Roof, racer.id, racer.custom.Roof);
        AddPurchasedItem(RacerCustomeType.Spoiler, racer.id, racer.custom.Spoiler);
        AddPurchasedItem(RacerCustomeType.Vinyl, racer.id, racer.custom.Vinyl);
        AddPurchasedItem(RacerCustomeType.Wheel, racer.id, racer.custom.Wheel);
        return this;
    }

    public ProfileData AddPurchasedItem(RacerCustomeType type, int racerId, int customeId)
    {
        if (customeId == 0) return this;
        return AddPurchasedItem(RacerProfile.GetCustomeSKU(type, racerId, customeId));
    }

    public ProfileData AddPurchasedItem(string sku)
    {
        if (purchasedItems.Exists(x => x == sku)) return this;
        purchasedItems.Add(sku);
        return this;
    }


    /////////////////////////////////////////////////////////////////////
    /// HELPER CLASSES
    /////////////////////////////////////////////////////////////////////
    [System.Serializable]
    public class NetDataBase
    {
        public int ver = 2; 
    }

    [System.Serializable]
    public class NetDataArrays : NetDataBase
    {
        public List<string> a1 = new List<string>();
        public List<RacerProfile> a2 = new List<RacerProfile>();
    }

    [System.Serializable]
    public class NetData : NetDataArrays
    {
        public int mod = 0;
        public CryptoInt i1 = 0;
        public CryptoInt i2 = 0;
        public CryptoInt i3 = 0;
        public CryptoInt i4 = 0;
        public CryptoInt i5 = 0;

        public bool IsEqualTo(NetData other)
        {
            if (other == null) return false;
            if (ver != other.ver) return false;
            if (i1.Value != other.i1.Value) return false;
            if (i2.Value != other.i2.Value) return false;
            if (i3.Value != other.i3.Value) return false;
            if (i4.Value != other.i4.Value) return false;
            if (a1.Count != other.a1.Count) return false;
            if (a2.Count != other.a2.Count) return false;

            var thistmp = new NetDataArrays();
            thistmp.a1 = a1;
            thistmp.a2 = a2;

            var othertmp = new NetDataArrays();
            othertmp.a1 = other.a1;
            othertmp.a2 = other.a2;

            var thisjson = JsonUtility.ToJson(thistmp);
            var othrjson = JsonUtility.ToJson(othertmp);
            return thisjson == othrjson;
        }
    }
}