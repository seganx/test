using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class DataBeen : MonoBehaviour
{
    [System.Serializable]
    public class Data<T>
    {
        public string secretKey = string.Empty;
        public string productName = string.Empty;
        public string deviceId = string.Empty;
        public string eventName = string.Empty;
        public string date = string.Empty;
        public string appVersion = string.Empty;
        public string versionCode = string.Empty;
        public string ip = string.Empty;
        //public string location = string.Empty;
        //public string carrier = string.Empty;
        //public string networkType = string.Empty;
        //public string sdkPlatform = string.Empty;
        public string sessionID = string.Empty;
        public List<T> parameter = new List<T>();
    }

    [System.Serializable]
    public class InstallParams
    {
        public string os = string.Empty;
        public string model = string.Empty;
        public string screen = string.Empty;
        public string market = string.Empty;
        //public string adid = string.Empty;
    }

    [System.Serializable]
    public class SessionParams
    {
        public string enter_date = string.Empty;
        public string exit_date = string.Empty;
        public string exit_code = string.Empty;
    }

    [System.Serializable]
    public class PurchaseParams
    {
        public string sku = string.Empty;
        public string token = string.Empty;
    }

    [System.Serializable]
    public class ContentViewParams
    {
        public string contenttype = string.Empty;
        public string result = string.Empty;
    }


    [SerializeField] private string secretKey = string.Empty;
    //[SerializeField] private string boundleVersionCode = string.Empty;
    private string sessionId = string.Empty;
    private string enterDate = string.Empty;
    private const string dateTimeFormat = "yyyy-MM-dd hh:mm:ss";

    private void Awake()
    {
        instance = this;
        sessionId = System.DateTime.Now.ToString("yyyyMMddhhmmss");
        enterDate = System.DateTime.Now.ToString(dateTimeFormat);
        header.Add("Accept", "text/html,application/json,application/xml");
        header.Add("Content-Type", "application/json");
        header.Add("Cache-Control", "no-cache, no-store, must-revalidate");
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("Databeen.IsFirst", 0) == 0)
        {
            SendData(GetInstallData());
            PlayerPrefs.SetInt("Databeen.IsFirst", 1);
        }
    }

    private void OnApplicationQuit()
    {
        SendData(GetSessionData());
    }

    private void SendData(object data)
    {
        StartCoroutine(DoSendData(data));
    }

    private IEnumerator DoSendData(object data)
    {
        var json = JsonUtility.ToJson(data);
        Debug.Log("DataBeen Sending: " + json);
        var res = new WWW("https://api.databeen.ir/v1/statistics", json.GetBytes(), header);
        yield return res;

        if (res.error.HasContent())
            Debug.Log("DataBeen Sent: " + res.text + " Error: " + res.error);
        //else
        //    Debug.Log("DataBeen Sent: " + res.text);
    }

    private Data<T> CreateData<T>()
    {
        var res = new Data<T>();
        res.appVersion = Application.version;
        res.date = System.DateTime.Now.ToString(dateTimeFormat);
        res.deviceId = SystemInfo.deviceUniqueIdentifier;
        res.ip = GetIP();
        res.productName = Application.identifier;
        res.secretKey = secretKey;
        res.sessionID = sessionId;
        //res.versionCode = boundleVersionCode;
        res.versionCode = GlobalConfig.Instance.version.ToString();
        return res;
    }


    private Data<InstallParams> GetInstallData()
    {
        var param = new InstallParams();
        param.market = "CafeBazaar";
        param.model = SystemInfo.deviceModel;
        param.os = SystemInfo.operatingSystem;
        param.screen = Screen.width + "x" + Screen.height;

        var res = CreateData<InstallParams>();
        res.eventName = "install";
        res.parameter.Add(param);
        return res;
    }


    private Data<SessionParams> GetSessionData()
    {
        var param = new SessionParams();
        param.enter_date = enterDate;
        param.exit_date = System.DateTime.Now.ToString(dateTimeFormat);
        param.exit_code = "920";

        var res = CreateData<SessionParams>();
        res.eventName = "session";
        res.parameter.Add(param);
        return res;
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static DataBeen instance = null;
    private static Dictionary<string, string> header = new Dictionary<string, string>();

    public static void SendContentView(string contenttype, string result)
    {
        var param = new ContentViewParams();
        param.contenttype = contenttype;
        param.result = result;

        var res = instance.CreateData<ContentViewParams>();
        res.eventName = "contentview";
        res.parameter.Add(param);
        instance.SendData(res);
    }

    public static void SendPurchase(string sku, string token)
    {
        var param = new PurchaseParams();
        param.sku = sku;
        param.token = token;

        var res = instance.CreateData<PurchaseParams>();
        res.eventName = "payment";
        res.parameter.Add(param);
        instance.SendData(res);
    }

    public static string GetIP()
    {
        string res = string.Empty;

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif 
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        res = ip.Address.ToString();
                    }
                }
            }
        }
        return res;
    }
}
