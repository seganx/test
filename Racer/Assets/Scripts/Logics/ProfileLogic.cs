using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileLogic : MonoBehaviour
{
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
    private static ProfileData.NetData lastdata = new ProfileData.NetData();

    public static bool Synced { get { return Profile.data.data.IsEqualTo(lastdata); } }

    private static bool IsGlobalConfigUpdated { get; set; }
    private static bool IsLoggedIn { get; set; }


    public static void SyncWidthServer(bool sendProfile, System.Action<bool> nextTask)
    {
        if (IsGlobalConfigUpdated)
        {
            LoginToServer(sendProfile, nextTask);
            return;
        }

        var address = GlobalConfig.Instance.address + GlobalConfig.Instance.version + "/config.txt?" + System.DateTime.Now.Ticks;
        Http.DownloadText(address, null, null, json =>
        {
            IsGlobalConfigUpdated = (json != null && GlobalConfig.SetData(json));
            if (IsGlobalConfigUpdated)
                LoginToServer(sendProfile, nextTask);
            else
                nextTask(false);
        });
    }

    private static void LoginToServer(bool sendProfile, System.Action<bool> nextTask)
    {
        if (IsLoggedIn)
        {
            GetProfile(sendProfile, nextTask);
            return;
        }

        Network.Login(msg =>
        {
            IsLoggedIn = msg == Network.Message.ok;
            if (IsLoggedIn)
                GetProfile(sendProfile, nextTask);
            else
                nextTask(false);
        });
    }

    private static void GetProfile(bool sendProfile, System.Action<bool> nextTask)
    {
        Network.GetProfile((msg, data) =>
        {
            if (msg == Network.Message.ok)
                SyncProfile(sendProfile, data, nextTask);
            else
                nextTask(false);
        });
    }

    private static void SyncProfile(bool sendProfile, ProfileData newprofile, System.Action<bool> nextTask)
    {
        if (Profile.data.userId == newprofile.userId)
        {
            var selfdata = Profile.data.data;
            Profile.data = newprofile;
            Profile.data.data = selfdata;
            SendProfileData(sendProfile, nextTask);
        }
        else if (Profile.UserId.IsNullOrEmpty())
        {
            if (newprofile.data != null)
            {
                Popup_Loading.Hide();
                Game.Instance.OpenPopup<Popup_AccountSelection>().Setup(yes =>
                {
                    if (yes)
                    {
                        Profile.data = newprofile;
                        Game.Instance.OpenPopup<Popup_Confirm>().Setup(111068, false, ok =>
                        {
                            PlayerPrefs.DeleteAll();
                            PlayerPrefsEx.ClearData();
                            SaveToLocal();
                            Application.Quit();
                        });
                    }
                    else
                    {
                        var selfdata = Profile.data.data;
                        Profile.data = newprofile;
                        Profile.data.data = selfdata;
                        SendProfileData(sendProfile, nextTask);
                    }
                });
            }
            else
            {
                var selfdata = Profile.data.data;
                Profile.data = newprofile;
                Profile.data.data = selfdata;
                SendProfileData(sendProfile, nextTask);
            }
        }
        else if (Profile.UserId != newprofile.userId)
        {
            Popup_Loading.Hide();
            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111067, false, yes =>
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefsEx.ClearData();
                Profile.data = new ProfileData();
                Application.Quit();

                //  I don't know if it works. but let it be for sure :)
                PlayerPrefs.DeleteAll();
                PlayerPrefsEx.ClearData();
            });
        }
    }

    private static void SendProfileData(bool sendProfile, System.Action<bool> nextTask)
    {
        if (Synced || sendProfile == false)
        {
            nextTask(true);
            return;
        }

        Profile.data.modified++;
        Network.SendProfile(Profile.data.data, sendmsg =>
        {
            if (sendmsg == Network.Message.ok)
            {
                lastdata = Profile.data.data.CloneSerialized<ProfileData.NetData>();
                nextTask(true);
            }
            else nextTask(false);
        });
    }

    private static void SaveToLocal()
    {
        PlayerPrefsEx.Serialize("ProfileLogic.Data", Profile.data);
        PlayerPrefsEx.Serialize("ProfileLogic.LastData", lastdata);
    }

    private static void LoadFromLocal()
    {
        Profile.data = PlayerPrefsEx.Deserialize("ProfileLogic.Data", new ProfileData());
        lastdata = PlayerPrefsEx.Deserialize("ProfileLogic.LastData", new ProfileData.NetData());
    }

}
