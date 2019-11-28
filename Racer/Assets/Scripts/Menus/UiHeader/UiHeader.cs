using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHeader : Base
{
    [SerializeField] private Image leagueImage = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private Button backButton = null;
    [SerializeField] private Button statsButton = null;
    [SerializeField] private Button garageButton = null;
    [SerializeField] private GameObject garageNotif = null;
    [SerializeField] private LocalText gemLabel = null;
    [SerializeField] private LocalText coinLabel = null;
    [SerializeField] private Button shopButton = null;
    [SerializeField] private MaskableGraphic syncIcon = null;
    [SerializeField] private Button profileButton = null;
    [SerializeField] private Button homeButton = null;
    [SerializeField] private Button settingsButton = null;

    private BlenderValue profileWidth = new BlenderValue() { speed = 15 };
    private BlenderValue position = new BlenderValue() { speed = 5 };
    private float initProfileWidth = 0;
    private float backButtonWidth = 0;

    private void OnEnable()
    {
        gameManager.OnOpenState += OnGameStateChanged;
        gameManager.OnBackButton += OnGameStateChanged;
    }

    private void OnDisable()
    {
        if (gameManager == null) return;
        gameManager.OnOpenState -= OnGameStateChanged;
        gameManager.OnBackButton -= OnGameStateChanged;
    }

    private IEnumerator Start()
    {
        initProfileWidth = statsButton.transform.AsRectTransform().rect.width;
        backButtonWidth = backButton.transform.AsRectTransform().rect.width;
        backButton.onClick.AddListener(() => gameManager.CurrentState.Back());
        profileButton.onClick.AddListener(() => gameManager.OpenState<State_Profile>());
        homeButton.onClick.AddListener(() => gameManager.OpenState<State_Home>(true));
        settingsButton.onClick.AddListener(() => gameManager.OpenState<State_Settings>());
        shopButton.onClick.AddListener(() => GoToShop(0));
        gemLabel.GetComponentInParent<Button>().onClick.AddListener(() => GoToShop(1));
        coinLabel.GetComponentInParent<Button>().onClick.AddListener(() => GoToShop(2));
        OnGameStateChanged(gameManager.CurrentState);

        garageButton.onClick.AddListener(() =>
        {
            gameManager.OpenState<State_Garage>().Setup(0, false, rc => gameManager.OpenState<State_PhotoMode>());
            PopupQueue.Add(.5f, () => Popup_Tutorial.Display(61));
        });

        var syncColor = 0;
        var waitseconds = new WaitForSeconds(0.5f);
        while (true)
        {
            leagueImage.sprite = GlobalFactory.League.GetSmallIcon(Profile.League);
            scoreLabel.SetText(Profile.Score.ToString("#,0"));
            gemLabel.SetText(Profile.Gem.ToString("#,0"));
            coinLabel.SetText(Profile.Coin.ToString("#,0"));
            garageNotif.SetActive(Profile.IsUnlockingRacerExist);

            syncColor++;
            if (syncColor % 2 == 0) syncIcon.color = syncColor < 0 ? Color.yellow : (ProfileLogic.Synced ? Color.green : Color.red);

            if ((System.DateTime.Now - lastSyncTime).TotalSeconds > 300)
            {
                lastSyncTime = System.DateTime.Now;
                if (ProfileLogic.Synced == false)
                    ProfileLogic.SyncWidthServer(true, done => syncColor = 0);
            }

            yield return waitseconds;
        }
    }

    private void Update()
    {
        if (profileWidth.Update(Time.deltaTime))
            statsButton.transform.SetAnchordWidth(profileWidth.current);

        if (position.Update(Time.deltaTime))
            rectTransform.SetAnchordPositionY(position.current);
    }

    private void OnGameStateChanged(GameState gamestate)
    {
        if (gameManager.CurrentState == null) return;
        var curstate = gameManager.CurrentState;
        bool ishome = curstate is State_Home;
        bool isprof = curstate is State_Profile;
        bool isshop = curstate is State_Shop;
        bool isgarage = curstate is State_Garage || curstate is State_Upgrade || curstate is State_Custome;

        settingsButton.gameObject.SetActive(ishome);
        homeButton.gameObject.SetActive(!ishome);
        profileButton.SetInteractable(!isprof);
        garageButton.SetInteractable(!isgarage);
        shopButton.SetInteractable(!isshop);

        profileWidth.destination = initProfileWidth + (ishome ? backButtonWidth : 0);

        if (gameManager.CurrentPopup == null)
            transform.SetAsLastSibling();
    }

    private void GoToShop(int index)
    {
        if (gameManager.CurrentState is State_Shop) return;
        gameManager.OpenState<State_Shop>();
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static System.DateTime lastSyncTime = System.DateTime.Now;
    private static UiHeader instance = null;

    public static void Show()
    {
        if (instance == null)
            instance = Resources.Load<UiHeader>(gameManager.prefabPath + typeof(UiHeader).Name).Clone<UiHeader>(gameManager.canvas.transform);
        instance.position.destination = 0;
    }

    public static void Hide()
    {
        if (instance == null) return;
        instance.position.destination = 200;
    }


    public static void Destroy()
    {
        if (instance == null) return;
        Destroy(instance.gameObject);
    }
}
