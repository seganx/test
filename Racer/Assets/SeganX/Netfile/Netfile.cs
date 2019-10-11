using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static class Netfile
    {
        [System.Serializable]
        public class Response<T>
        {
            public string msg = string.Empty;
            public T data = default(T);
        }

        [System.Serializable]
        public class LoginPost
        {
            public int game_id = 0;
            public string device_id = string.Empty;
        }

        [System.Serializable]
        public class Profile
        {
            public string username = string.Empty;
            public string password = string.Empty;
            public string nickname = string.Empty;
            public string status = string.Empty;
            public string datahash = string.Empty;
        }

        [System.Serializable]
        public class Data
        {
            public string private_data = string.Empty;
            public string public_data = string.Empty;
        }


        [System.Serializable]
        public class NicknamePost
        {
            public string nickname = string.Empty;
        }

        [System.Serializable]
        public class StatusPost
        {
            public string status = string.Empty;
        }

        [System.Serializable]
        public class DataPost : Data
        {
            public string hash = string.Empty;
        }

        [System.Serializable]
        public class UsernamePost
        {
            public string username = string.Empty;
        }

        [System.Serializable]
        public class IdPost
        {
            public int id = 0;
        }

        [System.Serializable]
        public class LeagueScorePost : IdPost
        {
            public int score = 0;
            public int value = 0;
            public string hash = string.Empty;
        }

        [System.Serializable]
        public class LeagueLeaderboardPost : IdPost
        {
            public int from = 0;
            public int count = 0;
        }

        [System.Serializable]
        public class League
        {
            public long start_time = 0;
            public long duration = 0;
            public int score = 0;
            public int position = 0;
            public int reward_score = 0;
            public int reward_position = 0;
        }

        [System.Serializable]
        public class LeagueProfile
        {
            public string username = string.Empty;
            public string nickname = string.Empty;
            public string status = string.Empty;
            public string score = string.Empty;
            public string position = string.Empty;
        }

        private static Dictionary<string, string> header = new Dictionary<string, string>();

        public static string error { get; private set; }

        private static void DownloadData<T>(string uri, object post, System.Action<bool, T> callback)
        {
            Http.DownloadText("http://seganx.com/games/api/" + uri, post == null ? null : JsonUtility.ToJson(post), header, resjson =>
            {
                if (resjson != null)
                {
                    var res = JsonUtility.FromJson<Response<T>>(resjson);
                    if (res.msg != Message.ok)
                    {
                        error = res.msg;
                        callback(false, res.data);
                    }
                    else callback(true, res.data);
                }
                else
                {
                    error = Message.TranslateHttp(Http.status);
                    callback(false, default(T));
                }
            });
        }


        public static void Time(System.Action<bool, long> callback)
        {
            DownloadData<long>("time.php", null, callback);
        }

        public static void Login(int gameId, string deviceId, System.Action<bool> callback)
        {
            var post = new LoginPost();
            post.game_id = gameId;
            post.device_id = deviceId;
            DownloadData<string>("login.php", post, (success, res) =>
            {
                if (success)
                    header["token"] = res;
                callback(success);
            });
        }

        public static void GetProfile(System.Action<bool, Profile> callback)
        {
            DownloadData("profile-get.php", null, callback);
        }

        public static void SetNickname(string nickname, System.Action<bool> callback)
        {
            var post = new NicknamePost();
            post.nickname = nickname;
            DownloadData<string>("profile-set-nickname.php", post, (success, res) => callback(success));
        }

        public static void SetStatus(string status, System.Action<bool> callback)
        {
            var post = new StatusPost();
            post.status = status;
            DownloadData<string>("profile-set-status.php", post, (success, res) => callback(success));
        }

        public static void SetData(string hash, string privateData, string publicData, System.Action<bool> callback)
        {
            var post = new DataPost();
            post.hash = hash;
            post.private_data = privateData;
            post.public_data = publicData;
            DownloadData<string>("data-set.php", post, (success, res) => callback(success));
        }

        public static void GetData(System.Action<bool, string, string> callback)
        {
            DownloadData<Data>("data-get.php", null, (success, res) => callback(success, res.private_data, res.public_data));
        }

        public static void GetDataPublic(string username, System.Action<bool, string> callback)
        {
            var post = new UsernamePost();
            post.username = username;
            DownloadData<string>("data-get-public.php", post, callback);
        }

        public static void SetLeagueScore(int id, int score, int value, string hash, System.Action<bool, int> callback)
        {
            var post = new LeagueScorePost();
            post.id = id;
            post.score = score;
            post.value = value;
            post.hash = hash;
            DownloadData<int>("league-set-score.php", post, callback);
        }

        public static void SetLeagueRewarded(int id, System.Action<bool> callback)
        {
            var post = new IdPost();
            post.id= id;
            DownloadData<string>("league-set-rewarded.php", post, (success, res) => callback(success));
        }

        public static void GetLeague(int id, System.Action<bool, League> callback)
        {
            var post = new IdPost();
            post.id = id;
            DownloadData("league-get.php", post, callback);
        }

        public static void GetLeagueLeaderboard(int id, int from, int count, System.Action<bool, List<LeagueProfile>> callback)
        {
            var post = new LeagueLeaderboardPost();
            post.id = id;
            post.from = from;
            post.count = count;
            DownloadData("league-get-leaderboard.php", post, callback);
        }

        public static class Message
        {
            public const string ok = "ok";
            public const string unknown = "unknown";
            public const string net_inaccessable = "net_inaccessable";
            public const string server_maintenance = "server_maintenance";
            public const string invalid_token = "invalid_token";
            public const string invalid_params = "invalid_params";
            public const string account_transfered = "account_transfered";

            internal static string TranslateHttp(Http.Status status)
            {
                switch (status)
                {
                    case Http.Status.NetworkError: return net_inaccessable;
                    case Http.Status.ServerError: return server_maintenance;
                }
                return unknown;
            }
        }
    }
}
