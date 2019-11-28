using LocalPush;
using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class State_Settings : GameState
{
    [SerializeField] private Toggle[] tabToggles = null;
    [SerializeField] private GameObject[] tabSections = null;

    [SerializeField] private Toggle notifFullFuel = null;
    [SerializeField] private Toggle notifFreePackage = null;
    [SerializeField] private Toggle notifNewLeague = null;
    [SerializeField] private Toggle notifLegendStore = null;
    [SerializeField] private Toggle notifCommons = null;
    [SerializeField] private Toggle playMusic = null;
    [SerializeField] private Toggle playSfx = null;
    [SerializeField] private Toggle matchMakingWaitMoreToggle = null;
    [SerializeField] private Toggle displayHudToggle = null;
    [SerializeField] private Image steeringImage = null;
    [SerializeField] private Button steeringButton = null;
    [SerializeField] private Button telegramButton = null;
    [SerializeField] private Button emailButton = null;
    [SerializeField] private Button testGraphicButton = null;
    [SerializeField] private Button surveyButton = null;
    [SerializeField] private Button creditsButton = null;

    private void Start()
    {
        for (int i = 0; i < tabToggles.Length; i++)
        {
            tabToggles[i].isOn = false;
            int index = i;
            tabToggles[i].onValueChanged.AddListener(isOn => { ActiveSection(index); });
        }
        tabToggles[LastSelectedSecionIndex].isOn = true;

        notifFullFuel.isOn = IsFullFuelActiveNotificationActive;
        notifFreePackage.isOn = IsFreePackageNotificationActive;
        notifNewLeague.isOn = IsNewLeagueNotificationActive;
        notifLegendStore.isOn = IsLegendStoreActive;
        notifCommons.isOn = IsCommonsNotificationActive;
        playMusic.isOn = AudioManager.Instance.IsMusicOn;
        playSfx.isOn = AudioManager.Instance.IsFxOn;
        matchMakingWaitMoreToggle.isOn = State_GoToRace.WaitMore;
        displayHudToggle.isOn = PlayerHud.DisplayBox;

        steeringImage.sprite = GlobalFactory.GetSteeringIcon(GameSettings.SteeringMode);
        steeringButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_SteeringMode>().Setup(() => steeringImage.sprite = GlobalFactory.GetSteeringIcon(GameSettings.SteeringMode)));

        notifFullFuel.onValueChanged.AddListener((active) => IsFullFuelActiveNotificationActive = active);
        notifFreePackage.onValueChanged.AddListener((active) => IsFreePackageNotificationActive = active);
        notifNewLeague.onValueChanged.AddListener((active) => IsNewLeagueNotificationActive = active);
        notifLegendStore.onValueChanged.AddListener((active) => IsLegendStoreActive = active);
        notifCommons.onValueChanged.AddListener((active) => IsCommonsNotificationActive = active);
        playMusic.onValueChanged.AddListener((active) => AudioManager.Instance.IsMusicOn = active);
        playSfx.onValueChanged.AddListener((active) => AudioManager.Instance.IsFxOn = active);
        matchMakingWaitMoreToggle.onValueChanged.AddListener((active) => State_GoToRace.WaitMore = active);
        displayHudToggle.onValueChanged.AddListener((active) => PlayerHud.DisplayBox = active);

        telegramButton.onClick.AddListener(() => Application.OpenURL(GlobalConfig.Socials.contactTelegramUrl));
        emailButton.onClick.AddListener(() => SendEmail());
        testGraphicButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_AutoConfig>());
        surveyButton.onClick.AddListener(() => Application.OpenURL(GlobalConfig.Socials.contactSurveyUrl));
        creditsButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Confirm>().Setup(111101, false, true, null));

        GarageCamera.SetCameraId(1);
        UiShowHide.ShowAll(transform);
    }

    private void ActiveSection(int index)
    {
        for (int i = 0; i < tabSections.Length; i++)
            tabSections[i].SetActive(false);
        tabSections[index].SetActive(true);
        LastSelectedSecionIndex = index;
    }

    private void SendEmail()
    {
        string subject = MyEscapeURL("Support");
        string body = MyEscapeURL("\n\n\n\n\n\n" + SystemInfo.operatingSystem + "\n" + SystemInfo.deviceModel + "\n" + Profile.UserId + "\n" + Core.DeviceId);
        Application.OpenURL("mailto:" + GlobalConfig.Socials.contactEmailUrl + "?subject=" + subject + "&body=" + body);
    }

    private string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    private static string notifFullFuelString = "Notif_FullFuel";
    public static bool IsFullFuelActiveNotificationActive
    {
        get { return PlayerPrefs.GetInt(notifFullFuelString, 1) == 1; }
        set
        {
            if (!value)
                NotificationManager.Cancel(NotificationType.FullFuel);
            PlayerPrefs.SetInt(notifFullFuelString, value ? 1 : 0);
        }
    }

    private static string notifFreePackageString = "Notif_FreePackage";
    public static bool IsFreePackageNotificationActive
    {
        get { return PlayerPrefs.GetInt(notifFreePackageString, 1) == 1; }
        set
        {
            if (!value)
                NotificationManager.Cancel(NotificationType.FreePackage);
            PlayerPrefs.SetInt(notifFreePackageString, value ? 1 : 0);
        }
    }

    private static string notifNewLeagueString = "Notif_NewLeague";
    public static bool IsNewLeagueNotificationActive
    {
        get { return PlayerPrefs.GetInt(notifNewLeagueString, 1) == 1; }
        set
        {
            if (!value)
                NotificationManager.Cancel(NotificationType.LeagueStart);
            PlayerPrefs.SetInt(notifNewLeagueString, value ? 1 : 0);
        }
    }

    private static string notifLegendStoreString = "Notif_LegendStore";
    public static bool IsLegendStoreActive
    {
        get { return PlayerPrefs.GetInt(notifLegendStoreString, 1) == 1; }
        set { PlayerPrefs.SetInt(notifLegendStoreString, value ? 1 : 0); }
    }

    private static string notifCommonsString = "Notif_Commons";
    public static bool IsCommonsNotificationActive
    {
        get { return PlayerPrefs.GetInt(notifCommonsString, 1) == 1; }
        set { PlayerPrefs.SetInt(notifCommonsString, value ? 1 : 0); }
    }

    private string lastSelectedSectionIndex = "Setting_LastSelectionSelectedIndex";

    private int LastSelectedSecionIndex
    {
        get { return PlayerPrefs.GetInt(lastSelectedSectionIndex); }
        set { PlayerPrefs.SetInt(lastSelectedSectionIndex, value); }
    }
}

public static class GameSettings
{
    public static RaceModel.SteeringMode SteeringMode
    {
        get { return (RaceModel.SteeringMode)PlayerPrefs.GetInt("Settings.SteeringMode", (int)RaceModel.SteeringMode.Normal); }
        set { PlayerPrefs.SetInt("Settings.SteeringMode", (int)value); }
    }
}
