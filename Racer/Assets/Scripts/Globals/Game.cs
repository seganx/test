using SeganX;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace SeganX
{
    public class Game : GameManager<Game>
    {
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //Application.targetFrameRate = 25;

            SeganX.Console.Info.SetOnDisplayInfo(info =>
            {

                string str = "Ver: " + Application.version;
                str += PhotonNetwork.connected ? (" Onlines: " + PhotonNetwork.countOfPlayers + " Rooms: " + PhotonNetwork.countOfRooms) : "Not in Lobby";
                str += "\nId: " + SeganX.Console.Info.DisplayDeviceID;
                return str;
            });
        }

        private IEnumerator Start()
        {
            Loaded = false;
            GameMap.Load(0, 0);
            Popup_Loading.Display();
            yield return new WaitForSeconds(1);
            PurchaseSystem.Initialize(GlobalConfig.Instance.cafeBazaarKey, GlobalConfig.Socials.storeUrl, (success, msg) => Debug.Log("Purchase system initialized: " + success + " " + msg));

            //  first try to connect to internet
            Http.requestTimeout = GlobalConfig.Server.requestTimeout / 2;
            ProfileLogic.SyncWidthServer(false, success =>
            {
                Http.requestTimeout = GlobalConfig.Server.requestTimeout;
                Popup_Loading.Hide();
                OpenState<State_Home>();
                Loaded = true;
            });

            GarageRacerImager.LoadCache();
        }


        ////////////////////////////////////////////////////////
        /// STATIC MEMBER
        ////////////////////////////////////////////////////////
        public static bool Loaded { get; private set; }
        public static void LoadMap(int id, int skyId)
        {
            GameMap.Load(id, skyId);

            if (id == 0)
                UiHeader.Show();
            else
                UiHeader.Destroy();
        }


        public static void SpendGem(int value, System.Action onSuccess)
        {
            if (Profile.SpendGem(value))
                onSuccess();
            else
                Instance.OpenPopup<Popup_Shop>().SetupAsGems(() =>
                {
                //if (Profile.SpendGem(value))
                //    onSuccess();
            });
        }

        public static void SpendCoin(int value, System.Action onSuccess)
        {
            if (Profile.SpendCoin(value))
                onSuccess();
            else
                Instance.OpenPopup<Popup_Confirm>().Setup(111144, true, true, (confirm) =>
                {
                    if (confirm)
                        Instance.OpenState<State_Shop>();
                });
        }


#if UNITY_EDITOR
        [Console("unlock", "racers")]
        public static void UnlockRacers()
        {
            foreach (var racer in RacerFactory.Racer.AllConfigs)
            {
                if (Profile.IsUnlockedRacer(racer.Id)) continue;
                Profile.AddRacerCard(racer.Id, racer.CardCount);
            }
        }
#endif
    }
}
