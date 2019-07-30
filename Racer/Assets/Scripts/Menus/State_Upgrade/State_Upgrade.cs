using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Upgrade : GameState
{
    public enum UpgradeType : int { Nitors = 0, Steering = 1, Body = 2, Null = 3 }

    [System.Serializable]
    public class UpgradableMenu
    {
        public UpgradeType type = UpgradeType.Null;
        public LocalText level = null;
        public LocalText value = null;
        public LocalText plus = null;
        public GameObject arrows = null;
    }

    [System.Serializable]
    public class CarNavigation
    {
        public Button next = null;
        public Button prev = null;
        public UiShowHide showHide = null;
    }

    [System.Serializable]
    public class UpgradWindow
    {
        public LocalText level = null;
        public Transform pictures = null;
        public LocalText priceTitle = null;
        public LocalText priceLabel = null;
        public Button buyButton = null;
        public UiShowHide showhide = null;
    }

    [System.Serializable]
    public class LockedBar
    {
        public LocalText cardsCountLabel = null;
        public LocalText descLabel = null;
        public Button cardButton = null;
        public UiShowHide showhide = null;
    }


    [SerializeField] private Button photoButton = null;
    [SerializeField] private Image racerIcon = null;
    [SerializeField] private Text racerName = null;
    [SerializeField] private LocalText racerPower = null;
    [SerializeField] private LocalText racerPowerPlus = null;
    [SerializeField] private Button customeButton = null;
    [SerializeField] private UpgradableMenu[] upgradeMenus = null;
    [SerializeField] private CarNavigation navigation = null;
    [SerializeField] private UpgradWindow upgradeWindow = null;
    [SerializeField] private LockedBar lockedBar = null;

    private RacerConfig config = null;
    private RacerProfile racerprofile = null;
    private UpgradeType selectedType = UpgradeType.Null;
    private float powerDiffValue = 0;

    private float CurrPower
    {
        get { return config.ComputePower(LevelNitro, LevelSteering, LevelBody); }
    }

    private int LevelNitro
    {
        get { return racerprofile == null ? 0 : racerprofile.level.NitroLevel; }
        set { if (racerprofile != null) racerprofile.level.NitroLevel = value; }
    }

    private int LevelSteering
    {
        get { return racerprofile == null ? 0 : racerprofile.level.SteeringLevel; }
        set { if (racerprofile != null) racerprofile.level.SteeringLevel = value; }
    }

    private int LevelBody
    {
        get { return racerprofile == null ? 0 : racerprofile.level.BodyLevel; }
        set { if (racerprofile != null) racerprofile.level.BodyLevel = value; }
    }

    private int SelectedLevel
    {
        get
        {
            switch (selectedType)
            {
                case UpgradeType.Nitors: return LevelNitro;
                case UpgradeType.Steering: return LevelSteering;
                case UpgradeType.Body: return LevelBody;
                default: return 0;
            }
        }

        set
        {
            switch (selectedType)
            {
                case UpgradeType.Nitors: LevelNitro = value; break;
                case UpgradeType.Steering: LevelSteering = value; break;
                case UpgradeType.Body: LevelBody = value; break;
            }
        }
    }

    private int SelectedPrice
    {
        get
        {
            switch (selectedType)
            {
                case UpgradeType.Nitors: return config.UpgradeCostNitro(LevelNitro + 1);
                case UpgradeType.Steering: return config.UpgradeCostSteering(LevelSteering + 1);
                case UpgradeType.Body: return config.UpgradeCostBody(LevelBody + 1);
                default: return 0;
            }
        }
    }

    private bool UpgradeWindowVisible
    {
        get { return upgradeWindow.showhide.Visible; }
        set
        {
            if (value)
            {
                upgradeWindow.showhide.Show();
                navigation.showHide.Hide();
            }
            else
            {
                upgradeWindow.showhide.Hide();
                navigation.showHide.Show();
            }
        }
    }

    private void Start()
    {
        UiHeader.Show();
        GarageCamera.SetCameraId(2);

        customeButton.onClick.AddListener(() => gameManager.OpenState<State_Custome>());
        photoButton.onClick.AddListener(() => gameManager.OpenState<State_PhotoMode>());
        navigation.prev.onClick.AddListener(() => DisplayNextCar(-1));
        navigation.next.onClick.AddListener(() => DisplayNextCar(1));
        upgradeWindow.buyButton.onClick.AddListener(OnBuyButtonClick);
        lockedBar.cardButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_RacerCardInfo>());

        DisplayInfo();
    }

    private void DisplayNextCar(int step)
    {
        var currindex = RacerFactory.Racer.AllConfigs.FindIndex(x => x.Id == GarageRacer.racer.Id);
        var nextindex = currindex + step;
        if (nextindex < 0) return;
        if (nextindex >= RacerFactory.Racer.AllConfigs.Count) return;
        var nextId = RacerFactory.Racer.AllConfigs[nextindex].Id;
        GarageRacer.LoadRacer(nextId);
        Profile.SelectedRacer = nextId;
        selectedType = UpgradeType.Null;
        DisplayInfo();
    }

    private void DisplayInfo()
    {
        UiShowHide.ShowAll(transform);
        config = RacerFactory.Racer.GetConfig(GarageRacer.racer.Id);
        racerprofile = Profile.GetRacer(config.Id);
        racerIcon.sprite = config.icon;
        racerName.SetText(config.Name);

        if (Profile.IsUnlockedRacer(config.Id))
        {
            //customeButton.gameObject.SetActive(true);
            lockedBar.showhide.Hide();
        }
        else
        {
            lockedBar.descLabel.SetFormatedText(Mathf.Clamp(config.CardCount - (racerprofile != null ? racerprofile.cards : 0), 0, config.CardCount));
            lockedBar.cardsCountLabel.SetFormatedText(racerprofile != null ? racerprofile.cards : 0, config.CardCount);
#if UNITY_EDITOR
            //      costumButton.gameObject.SetActive(false);
#else
            //costumButton.gameObject.SetActive(false);
#endif

            lockedBar.showhide.Show();
        }

        foreach (var menu in upgradeMenus)
            DisplayMenu(menu, false);
        DisplayWindow();
    }

    public void OnSelectItem(int type)
    {
        if (racerprofile == null || Profile.IsUnlockedRacer(config.Id) == false) return;
        selectedType = (UpgradeType)type;

        foreach (var menu in upgradeMenus)
            DisplayMenu(menu, menu.type == selectedType);

        DisplayWindow();
    }

    private void DisplayWindow()
    {
        if (Profile.IsUnlockedRacer(config.Id) && selectedType != UpgradeType.Null)
        {
            UpgradeWindowVisible = true;
            upgradeWindow.pictures.SetActiveChild((int)selectedType);
            upgradeWindow.level.SetFormatedText(SelectedLevel);
            upgradeWindow.buyButton.gameObject.SetActive(SelectedLevel < config.MaxUpgradeLevel);
            upgradeWindow.priceTitle.SetFormatedText(SelectedLevel + 1);
            upgradeWindow.priceLabel.SetFormatedText(SelectedPrice);
        }
        else UpgradeWindowVisible = false;

        racerPower.SetFormatedText(CurrPower, config.MaxPower);
        racerPowerPlus.SetFormatedText(powerDiffValue);
        racerPowerPlus.gameObject.SetActive(powerDiffValue > Mathf.Epsilon);
    }

    private void DisplayMenu(UpgradableMenu menu, bool showPlus)
    {
        int level = 0;
        float value = 0;
        float nvalue = 0;

        switch (menu.type)
        {
            case UpgradeType.Nitors:
                level = LevelNitro;
                value = config.ComputeNitro(level);
                nvalue = config.ComputeNitro(level + 1);
                if (showPlus) powerDiffValue = config.ComputePower(LevelNitro + 1, LevelSteering, LevelBody) - CurrPower;
                break;
            case UpgradeType.Steering:
                level = LevelSteering;
                value = config.ComputeSteering(level);
                nvalue = config.ComputeSteering(level + 1);
                if (showPlus) powerDiffValue = config.ComputePower(LevelNitro, LevelSteering + 1, LevelBody) - CurrPower;
                break;
            case UpgradeType.Body:
                level = LevelBody;
                value = config.ComputeBody(level);
                nvalue = config.ComputeBody(level + 1);
                if (showPlus) powerDiffValue = config.ComputePower(LevelNitro, LevelSteering, LevelBody + 1) - CurrPower;
                break;
        }

        menu.level.SetFormatedText(level);
        menu.value.SetFormatedText(value);
        var diff = nvalue - value;
        menu.plus.SetFormatedText(diff);
        menu.plus.gameObject.SetActive(showPlus && diff > Mathf.Epsilon);
        menu.arrows.SetActive(Profile.IsUnlockedRacer(config.Id) && diff > Mathf.Epsilon);
    }

    private void OnBuyButtonClick()
    {
        Game.SpendCoin(SelectedPrice, () =>
        {
            SelectedLevel++;
            OnSelectItem((int)selectedType);
        });
    }

    public override void Back()
    {
        if (UpgradeWindowVisible)
            UpgradeWindowVisible = false;
        else
            base.Back();
    }
}
