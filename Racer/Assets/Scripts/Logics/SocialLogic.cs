using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialLogic : MonoBehaviour
{
    [System.Serializable]
    private class SerializableData
    {
        public List<LikeData> likesByMe = new List<LikeData>();
    }

    private void Awake()
    {
        LoadFromLocal();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveToLocal();
    }

    private void OnApplicationQuit()
    {
        SaveToLocal();
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    private static SocialData socialData = null;
    private static SerializableData data = new SerializableData();
    private static System.DateTime lastSocialUpdate = System.DateTime.Now;

    public static bool DownloadFromServer
    {
        get { return PlayerPrefs.GetInt("Likes.DownloadFromServer", 0) > 0; }
        set { PlayerPrefs.SetInt("Likes.DownloadFromServer", value ? 1 : 0); }
    }

    public static void GetSocialData(System.Action<SocialData> callback)
    {
        if (socialData == null || (System.DateTime.Now - lastSocialUpdate).TotalMinutes > 15)
        {
            lastSocialUpdate = System.DateTime.Now;
            Network.GetProfileSocialData(res =>
            {
                socialData = res ?? new SocialData();
                callback(socialData);
            });
        }
        else callback(socialData);
    }

    public static bool IsLiked(string profileId, int racerId)
    {
        return data.likesByMe.Exists(x => x.likedForProfile == profileId && x.racerId == racerId);
    }

    public static bool Action(string profileId, int racerId)
    {
        if (IsLiked(profileId, racerId))
        {
            data.likesByMe.RemoveAll(x => x.likedForProfile == profileId && x.racerId == racerId);
            return false;
        }
        else
        {
            data.likesByMe.Add(new LikeData(profileId, racerId));
            return true;
        }
    }

    private static void SaveToLocal()
    {
        PlayerPrefsEx.Serialize("SocialLogic.Data", data);
    }

    private static void LoadFromLocal()
    {
        data = PlayerPrefsEx.Deserialize("SocialLogic.Data", new SerializableData());
    }

    public static void SetData(List<LikeData> likeList)
    {
        if (likeList == null) return;
        DownloadFromServer = false;
        data.likesByMe = likeList;
        SaveToLocal();
    }
}
