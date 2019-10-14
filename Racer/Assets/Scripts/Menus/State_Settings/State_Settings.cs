using SeganX;
using UnityEngine;
using UnityEngine.UI;
using LocalPush;

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
        creditsButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Confirm>().Setup(111101, false, null));

        GarageCamera.SetCameraId(1);
        UiShowHide.ShowAll(transform);
    }

    void ActiveSection(int index)
    {
        for (int i = 0; i < tabSections.Length; i++)
            tabSections[i].SetActive(false);
        tabSections[index].SetActive(true);
        LastSelectedSecionIndex = index;
    }

    void SendEmail()
    {
        string subject = MyEscapeURL("Support");
        string body = MyEscapeURL("\n\n\n\n\n\n" + SystemInfo.operatingSystem + "\n" + SystemInfo.deviceModel + "\n" + Profile.UserId + "\n" + Core.DeviceId);
        Application.OpenURL("mailto:" + GlobalConfig.Socials.contactEmailUrl + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    static string notifFullFuelString = "Notif_FullFuel";
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

    static string notifFreePackageString = "Notif_FreePackage";
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

    static string notifNewLeagueString = "Notif_NewLeague";
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

    static string notifLegendStoreString = "Notif_LegendStore";
    public static bool IsLegendStoreActive
    {
        get { return PlayerPrefs.GetInt(notifLegendStoreString, 1) == 1; }
        set { PlayerPrefs.SetInt(notifLegendStoreString, value ? 1 : 0); }
    }

    static string notifCommonsString = "Notif_Commons";
    public static bool IsCommonsNotificationActive
    {
        get { return PlayerPrefs.GetInt(notifCommonsString, 1) == 1; }
        set { PlayerPrefs.SetInt(notifCommonsString, value ? 1 : 0); }
    }

    string lastSelectedSectionIndex = "Setting_LastSelectionSelectedIndex";
    int LastSelectedSecionIndex
    {
        get { return PlayerPrefs.GetInt(lastSelectedSectionIndex); }
        set { PlayerPrefs.SetInt(lastSelectedSectionIndex, value); }
    }
}
