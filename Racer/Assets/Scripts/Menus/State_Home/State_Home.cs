using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Home : GameState
{
    [SerializeField] private Button onlineButton = null;
    [SerializeField] private Button offlineButton = null;
    [SerializeField] private Button photoButton = null;
    [SerializeField] private Button tutorialButton = null;


#if OFF
    private bool SeenGameplayTutorial
    {
        get { return PlayerPrefs.GetInt("State_Home.Tutorial", 0) == 1; }
        set { PlayerPrefs.SetInt("State_Home.Tutorial", value ? 1 : 0); }
    }
#endif

    private void Start()
    {
#if DATABEEN && OFF
        DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] {
            new DataBeenConnection.CustomEventInfo() { key = "gem", value = Profile.Gem.ToString() },
            new DataBeenConnection.CustomEventInfo() { key = "coin", value = Profile.Coin.ToString() },
            new DataBeenConnection.CustomEventInfo() { key = "score", value = Profile.Score.ToString() },
            new DataBeenConnection.CustomEventInfo() { key = "power", value = Profile.CurrentRacerPower.ToString() },
        });
#endif        

#if OFF
        offlineButton.SetInteractable(SeenGameplayTutorial);
        onlineButton.SetInteractable(SeenGameplayTutorial);
        if (SeenGameplayTutorial == false)
            DelayCall(1, () => tutorialButton.transform.parent.GoToAnchordPosition(-230, 0, 0, 3));
#endif

        GarageCamera.SetCameraId(1);

        offlineButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "offline" } });
#endif
            StartOffline();
        });

        onlineButton.onClick.AddListener(() =>
        {
#if DATABEEN
            DataBeen.SendCustomEventData("home", new DataBeenConnection.CustomEventInfo[] { new DataBeenConnection.CustomEventInfo() { key = "select", value = "online" } });
#endif
            gameManager.OpenState<State_LeagueStart>();
        });

        photoButton.onClick.AddListener(() => gameManager.OpenState<State_Upgrade>());

        /*tutorialButton.onClick.AddListener(() =>
        {
            //SeenGameplayTutorial = true;
            gameManager.OpenPopup<Popup_GamePlayTutorial>().SetOnClose(() =>
                {
                    //offlineButton.SetInteractable(true);
                    //onlineButton.SetInteractable(true);
                    DelayCall(1, () => tutorialButton.transform.parent.GoToAnchordPosition(0, 0, 0, 3));
                });
        });*/

        CheckPreset(() =>
        {
            UiHeader.Show();
            UiShowHide.ShowAll(transform);
            GarageRacer.LoadRacer(0);

            Popup_RateUs.CheckAndDisplay();
        });
    }

    public override void Back()
    {
        gameManager.OpenPopup<Popup_Confirm>().Setup(111063, true, yes =>
        {
            if (yes) Application.Quit();
        });
    }

    private void StartOffline()
    {
        PlayModel.OfflineMode = true;
        PlayModel.eloScore = 0;
        PlayModel.eloPower = 0;
        PlayModel.selectedMapId = PlayModel.SelectRandomMap();
        PlayModel.maxPlayerCount = 4;

        gameManager.OpenState<State_FindOpponents>();
    }

    private void CheckPreset(System.Action nextTask)
    {
        if (Profile.SelectedRacer > 0)
            nextTask();
        else
            gameManager.OpenPopup<Popup_Welcome>().SetCallback(nextTask);
    }
}
