using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Garage : GameState
{
    [SerializeField] private Button inventoryButton = null;
    [SerializeField] private Button steeringButton = null;
    [SerializeField] private Image steeringImage = null;
    [SerializeField] private GameObject steeringHelp = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private RectTransform separatorPrefab = null;
    [SerializeField] private UiGarageRacerItem itemPrefab = null;
    [SerializeField] private float itemSpace = 20;

    private RectTransform container = null;

    private float LastPosition
    {
        get { return PlayerPrefs.GetFloat("StateGarage.LastPosition", 0); }
        set { PlayerPrefs.SetFloat("StateGarage.LastPosition", value); }
    }

    public State_Garage Setup(int displayTargetGroup, bool displaySteeringMode, System.Action<RacerConfig> onNextTask)
    {
        OnNextTask = onNextTask;
        targetGroup = displayTargetGroup;
        selectSteeringMode = displaySteeringMode;
        return this;
    }

    private void Awake()
    {
        container = itemPrefab.parentRectTransform;
        descLabel.gameObject.SetActive(false);
    }

    private void Start()
    {
        GarageCamera.SetCameraId(1);
        UiHeader.Show();
        UiShowHide.ShowAll(transform);

        inventoryButton.SetInteractable(Popup_Inventory.ComputeNumberOfCards() > 0);
        inventoryButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Inventory>().Setup(() => inventoryButton.SetInteractable(Popup_Inventory.ComputeNumberOfCards() > 0)));

        if (selectSteeringMode)
        {
            steeringButton.gameObject.SetActive(true);
            steeringImage.sprite = GlobalFactory.GetSteeringIcon(GameSettings.SteeringMode);
            steeringHelp.gameObject.SetActive(PlayerPrefs.GetInt("State_Garage.Steering.Help", 1) > 0);
            PlayerPrefs.SetInt("State_Garage.Steering.Help", 0);

            steeringButton.onClick.AddListener(() =>
            {
                steeringHelp.gameObject.SetActive(false);
                gameManager.OpenPopup<Popup_SteeringMode>().Setup(() => steeringImage.sprite = GlobalFactory.GetSteeringIcon(GameSettings.SteeringMode));
            });
        }
        else steeringButton.gameObject.SetActive(false);


        DisplayItems();

        container.SetAnchordPositionX(LastPosition);
    }

    public override float PreClose()
    {
        if (container != null)
            LastPosition = container.anchoredPosition.x;
        return base.PreClose();
    }

    public void DisplayItems()
    {
        List<RacerConfig> cars = null;
        if (targetGroup > 0)
        {
            cars = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId == targetGroup);
            if (cars.Exists(x => Profile.IsUnlockedRacer(x.Id)) == false)
                cars.AddRange(RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId == targetGroup - 1));

            descLabel.gameObject.SetActive(true);
            descLabel.SetFormatedText(targetGroup);
        }
        else cars = RacerFactory.Racer.AllConfigs;
        cars.Sort((x, y) => x.GroupId == y.GroupId ? x.Id - y.Id : x.GroupId - y.GroupId);

        container.RemoveChildren(2);
        separatorPrefab.gameObject.SetActive(true);
        itemPrefab.gameObject.SetActive(true);
        int lastgroupid = -1;
        Vector3 itempos = Vector3.zero;
        foreach (var car in cars)
        {
            if (lastgroupid != car.GroupId)
            {
                itempos.x += itempos.y < -1 ? (itemPrefab.rectTransform.rect.width + itemSpace) : 0;
                itempos.y = 0;
                separatorPrefab.Clone<RectTransform>().SetAnchordPositionX(itempos.x).GetComponent<Text>(true, true).SetText(car.GroupId.ToString(), false, LocalizationService.IsPersian);
                itempos.x += separatorPrefab.rect.width + itemSpace;
                lastgroupid = car.GroupId;
            }

            itemPrefab.Clone<UiGarageRacerItem>().Setup(car, uitem =>
            {
                GarageRacer.LoadRacer(car.Id);
                if (Profile.IsUnlockingRacer(car.Id) && Profile.UnlockRacer(car.Id))
                {
                    gameManager.OpenState<State_UnlockRacer>().Setup(car.Id);
                    ProfileLogic.SyncWidthServer(false, success => { });
                }
                else
                {
                    Profile.SelectedRacer = car.Id;
                    OnNextTask(car);
                }
            }
            ).rectTransform.anchoredPosition = itempos;

            itempos.x += itempos.y < -1 ? (itemPrefab.rectTransform.rect.width + itemSpace) : 0;
            itempos.y = itempos.y < -1 ? 0 : -(itemPrefab.parentRectTransform.rect.height - itemPrefab.rectTransform.rect.height);
        }
        itemPrefab.parentRectTransform.SetAnchordWidth(itempos.x + itemPrefab.rectTransform.rect.width + itemSpace);

        separatorPrefab.gameObject.SetActive(false);
        itemPrefab.gameObject.SetActive(false);
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static System.Action<RacerConfig> OnNextTask = carid => gameManager.OpenState<State_Upgrade>();
    private static int targetGroup = 0;
    private static bool selectSteeringMode = false;
}
