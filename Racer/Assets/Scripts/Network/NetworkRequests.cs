using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Network
{
    [System.Serializable]
    public class LoginBody
    {
        public string deviceId = string.Empty;
    }

    [System.Serializable]
    public class ProfileBody
    {
        public string data = string.Empty;
    }

    [System.Serializable]
    public class ScoreBody
    {
        public int score = 0;
    }

    [System.Serializable]
    public class NicknameBody
    {
        public string nickName = string.Empty;
    }

    [System.Serializable]
    public class TransferAccountBody
    {
        public string deviceId = string.Empty;
        public string userKey = string.Empty;
    }

    [System.Serializable]
    public class PrizeBody
    {
    }

    [System.Serializable]
    public class LikeBody
    {
        public int racerId = 0;
        public string likedFor = string.Empty;
    }
}