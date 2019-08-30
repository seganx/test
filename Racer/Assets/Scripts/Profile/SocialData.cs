using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LikeData
{
    public int racerId = 0;
    public string likedForProfile = string.Empty;

    public LikeData(string profileId, int racerId)
    {
        likedForProfile = profileId;
        this.racerId = racerId;
    }
}

[System.Serializable]
public class RacerLike
{
    public int racerId = 0;
    public int count = 0;
}

[System.Serializable]
public class SocialData
{
    public List<RacerLike> racerLikes = new List<RacerLike>();
    public int dailyViews = 0;
}