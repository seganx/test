using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RacerCustomeType : byte
{
    None = 0,
    Height = 1,
    Hood = 2,
    Horn = 3,
    Roof = 4,
    Spoiler = 5,
    Vinyl = 6,
    Wheel = 7,
    BodyColor = 8,
    WindowColor = 9,
    LightsColor = 10,
    HoodColor = 11,
    RoofColor = 12,
    SpoilerColor = 13,
    VinylColor = 14,
    RimColor = 15
}

[System.Serializable]
public class RacerCustomData
{
    public int[] di = new int[16];

    public int Height { set { di[0] = value; } get { return di[0]; } }
    public int Hood { set { di[1] = value; } get { return di[1]; } }
    public int Horn { set { di[2] = value; } get { return di[2]; } }
    public int Roof { set { di[3] = value; } get { return di[3]; } }
    public int Spoiler { set { di[4] = value; } get { return di[4]; } }
    public int Vinyl { set { di[5] = value; } get { return di[5]; } }
    public int Wheel { set { di[6] = value; } get { return di[6]; } }
    public int BodyColor { set { di[7] = value; } get { return di[7]; } }
    public int WindowColor { set { di[8] = value; } get { return di[8]; } }
    public int LightsColor { set { di[9] = value; } get { return di[9]; } }
    public int HoodColor { set { di[10] = value; } get { return di[10]; } }
    public int RoofColor { set { di[11] = value; } get { return di[11]; } }
    public int SpoilerColor { set { di[12] = value; } get { return di[12]; } }
    public int VinylColor { set { di[13] = value; } get { return di[13]; } }
    public int RimColor { set { di[14] = value; } get { return di[14]; } }
    public int ColorModel { set { di[15] = value; } get { return di[15]; } }
}

[System.Serializable]
public class RacerLevelData
{
    public int[] di = new int[4];

    public int Level { set { di[0] = value; } get { return di[0]; } }
    public int NitroLevel { set { di[1] = value; } get { return di[1]; } }
    public int SteeringLevel { set { di[2] = value; } get { return di[2]; } }
    public int BodyLevel { set { di[3] = value; } get { return di[3]; } }
}

[System.Serializable]
public class VersionRacerProfile
{
    public int ver = 1;
}

[System.Serializable]
public class RacerProfile : VersionRacerProfile
{
    public int id = 0;
    public int cards = 0;
    public RacerLevelData level = new RacerLevelData();
    public RacerCustomData custom = new RacerCustomData();

    public static RacerProfile FromJson(string json)
    {
        var version = JsonUtility.FromJson<VersionRacerProfile>(json);
        if (version.ver == 1)
            return JsonUtility.FromJson<RacerProfile>(json);
        else
            return new RacerProfile();
    }

    public static string GetCustomeSKU(RacerCustomeType type, int racerId, int customeId)
    {
        return (int)type + "_" + racerId + "_" + customeId;
    }
}
