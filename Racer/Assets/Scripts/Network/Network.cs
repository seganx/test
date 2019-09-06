using SeganX;
using SeganX.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Network
{
    public static event System.Action<string> OnErrorOccurred = err => { };

    private static Dictionary<string, string> header = new Dictionary<string, string>();

    private static string address { get { return GlobalConfig.Server.address; } }

    private static void DownloadData<T>(string url, object post, System.Action<string, T> callback)
    {

        if (header.Count < 3)
        {
            header.Add("GameVersion", GlobalConfig.Instance.version.ToString());
            header.Add("GameMarket", ((int)GlobalConfig.Instance.market).ToString());
            header.Add("GameKey", GlobalConfig.Instance.gamekey.ToString());
        }

        Http.DownloadText(url, post == null ? null : JsonUtility.ToJson(post), header, resjson =>
        {
            if (resjson != null)
            {
                var res = JsonUtility.FromJson<ResponseBase<T>>(resjson);
                if (res.message != Message.ok)
                {
                    OnErrorOccurred(res.message);
                    callback(res.message, res.data);
                }
                else callback(res.message, res.data);
            }
            else callback(Message.TranslateHttp(Http.status), default(T));
        });
    }

    public static void GetConfig(System.Action<string, ConfigResponse> callback)
    {
        DownloadData(address + "Application/GetConfig", null, callback);
    }

    public static void Login(System.Action<string> callback)
    {
        var post = new LoginBody();
        post.deviceId = Core.DeviceId;
        DownloadData<string>(address + "Account/Login", post, (msg, res) =>
        {
            if (msg == Message.ok)
                header["Authorization"] = "Bearer " + res;
            callback(msg);
        });
    }

    public static void GetProfile(System.Action<string, ProfileData> callback)
    {
        DownloadData<ProfileResponse>(address + "Application/GetProfile", null, (msg, res) =>
        {
            if (msg == Message.ok)
            {
                var pr = new ProfileData();
                pr.key = res.key;
                pr.userId = res.profileId;
                pr.nickName = res.nickname;
                pr.position = res.league1Position;
                pr.score = res.league1Score;
                pr.rewardedPosition = res.prizeInfo == null ? 0 : res.prizeInfo.position;
                pr.rewardedScore = res.prizeInfo == null ? 0 : res.prizeInfo.score;

                var dejson = Utilities.DecompressString(res.data.Split('.')[0], string.Empty);
                pr.data = JsonUtilityEx.FromJson<ProfileData.NetData>(dejson);

                callback(msg, pr);
            }
            else callback(msg, null);
        });
    }

    public static void SendProfile(ProfileData.NetData data, System.Action<string> callback)
    {
        var post = new ProfileBody();
        post.data = Utilities.CompressString(JsonUtilityEx.ToJson(data), "{}") + "." + Http.userheader;
        DownloadData<string>(address + "Application/SendProfileData", post, (msg, res) => callback(msg));
    }

    public static void SendScore(int score, int trycount = 5, System.Action<string> callback = null)
    {
        var post = new ScoreBody();
        post.score = score;

        var salt = TimerManager.ServerTime.Day.ToString() + TimerManager.ServerTime.Hour.ToString();
        header["HashedRank"] = score.ToString().ComputeMD5(salt);
        DownloadData<string>(address + "Application/SendScore", post, (msg, res) =>
        {
            header.Remove("HashedRank");
            if (msg == Message.ok)
            {
                //Profile.Score = res.score;
                //Profile.Position = res.position;
            }
            else if (trycount > 0)
            {
                SendScore(score, --trycount, callback);
                return;
            }
            if (callback != null) callback(msg);
        });
    }

    public static void SendNickname(string nickname, System.Action<string> callback)
    {
        var post = new NicknameBody();
        post.nickName = nickname;
        DownloadData<string>(address + "Application/SendNickName", post, (msg, res) => callback(msg));
    }

    public static void SendPrizeResult(System.Action<string> callback)
    {
        DownloadData<string>(address + "Application/PrizeRecevied", new PrizeBody(), (msg, res) => callback(msg));
    }

    public static void GetLeaderboard(bool topPlayers, System.Action<string, List<LeaderboardProfileResponse>> callback)
    {
        var uri = address + (topPlayers ? "Application/GetTopPlayers/" + GlobalConfig.Server.getTopPlayersCount : "Application/GetLeagueData");
        DownloadData<List<LeaderboardProfileResponse>>(uri, null, (msg, res) =>
        {
            if (res != null)
            { 

                res.RemoveAll(x => x.position < 1);
                res.Sort((x, y) => x.position - y.position);
                foreach (var item in res)
                    if (item.nickname.IsNullOrEmpty())
                        item.nickname = item.profileId;
            }
            callback(msg, res);
        });
    }

    public static void TransferAccount(string profileKey, System.Action<string> callback)
    {
        var post = new TransferAccountBody();
        post.deviceId = Core.DeviceId;
        post.userKey = profileKey;
        DownloadData<string>(address + "Account/TransferAccount", post, (msg, res) =>
        {
            if (msg == Message.ok)
                header["Authorization"] = "Bearer " + res;
            callback(msg);
        });
    }

    public static void GetPlayerInfo(string profileId, System.Action<PlayerInfoResponse> callback)
    {
        DownloadData<PlayerInfoResponse>(address + "Players/GetPlayerInfo?profileId=" + profileId, null, (msg, res) =>
        {
            if (msg == Message.ok && res.profileData.HasContent())
            {
                res.profileId = profileId;
                var dejson = Utilities.DecompressString(res.profileData.Split('.')[0], string.Empty);
                res.netData = JsonUtilityEx.FromJson<ProfileData.NetData>(dejson);
                if (res.netData != null)
                {
                    res.netData.Validate();
                    callback(res);
                }
                else callback(null);
            }
            else callback(null);
        });
    }

    public static void Like(string profileId, int racerId, System.Action<bool> callback)
    {
        var post = new LikeBody();
        post.likedFor = profileId;
        post.racerId = racerId;
        DownloadData<string>(address + "Players/Like", post, (msg, res) => callback(msg == Message.ok));
    }

    public static void GetLikesByMe(System.Action<List<LikeData>> callback)
    {
        DownloadData<List<LikeData>>(address + "Players/GetLikesByMe", null, (msg, res) =>
        {
            if (msg == Message.ok)
            {
                callback(res);
            }
            else callback(null);
        });
    }

    public static void GetProfileSocialData(System.Action<SocialData> callback)
    {
        DownloadData<SocialData>(address + "Players/GetMyProfileLikesAndViews", null, (msg, res) =>
        {
            if (msg == Message.ok)
            {
                callback(res);
            }
            else callback(null);
        });
    }

    public static class Message
    {
        public const string ok = "ok";
        public const string unknown = "unknown";
        public const string networkNotReachable = "network not reachable";
        public const string serverMaintentance = "server maintenance";

        internal static string TranslateHttp(Http.Status status)
        {
            switch (status)
            {
                case Http.Status.NetworkError: return networkNotReachable;
                case Http.Status.ServerError: return serverMaintentance;
            }
            return unknown;
        }
    }

}
