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


public static class Likes
{
    [System.Serializable]
    public class Data
    {
        public List<LikeData> likes = new List<LikeData>();
    }

    public static Data data = new Data();

    public static bool DownloadFromServer
    {
        get { return PlayerPrefs.GetInt("Likes.DownloadFromServer", 0) > 0; }
        set { PlayerPrefs.SetInt("Likes.DownloadFromServer", value ? 1 : 0); }
    }

    public static bool IsLiked(string profileId, int racerId)
    {
        return data.likes.Exists(x => x.likedForProfile == profileId && x.racerId == racerId);
    }

    public static bool Action(string profileId, int racerId)
    {
        if (IsLiked(profileId, racerId))
        {
            data.likes.RemoveAll(x => x.likedForProfile == profileId && x.racerId == racerId);
            return false;
        }
        else
        {
            data.likes.Add(new LikeData(profileId, racerId));
            return true;
        }
    }

    public static void Save()
    {
        PlayerPrefsEx.Serialize("Likes.Data", data);
    }

    public static void Load()
    {
        data = PlayerPrefsEx.Deserialize("Likes.Data", new Data());
    }

    public static void SetData(List<LikeData> likeList)
    {
        if (likeList == null) return;
        DownloadFromServer = false;
        data.likes = likeList;
        Save();
    }
}