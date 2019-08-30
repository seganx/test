using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Network
{
    [System.Serializable]
    public class ResponseBase<T>
    {
        public string message = string.Empty;
        public T data = default(T);
    }

    [System.Serializable]
    public class ConfigResponse
    {
        public long serverTime = 0;
        public long league1EndDate = 0;
        public long league1StartDate = 0;
    }

    [System.Serializable]
    public class PrizeInfo
    {
        public int position = 0;
        public int score = 5;
    }

    [System.Serializable]
    public class ProfileResponse
    {
        public string key = string.Empty;
        public string profileId = string.Empty;
        public string nickname = string.Empty;
        public int league1Position = 0;
        public int league1Score = 5;
        public string data = string.Empty;
        public PrizeInfo prizeInfo = null;
    }

    [System.Serializable]
    public class ScoreResponse
    {
        public int score = 0;
        public int position = 0;
    }

    [System.Serializable]
    public class LeaderboardProfileResponse
    {
        public string nickname = string.Empty;
        public string profileId = string.Empty;
        public int score = 0;
        public int position = 0;
    }

    [System.Serializable]
    public class PlayerInfoResponse
    {
        public string profileId = string.Empty;
        public int dailyProfileView = 0;
        public string profileData = string.Empty;
        public ProfileData.NetData netData = null;
        public List<RacerLike> racerLikes = new List<RacerLike>();
    }


}
