using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Garage : GameState
{
    [SerializeField] private Button inventoryButton = null;
    [SerializeField] private RectTransform separatorPrefab = null;
    [SerializeField] private UiGarageRacerItem itemPrefab = null;
    [SerializeField] private float itemSpace = 20;

    private RectTransform container = null;

    private float LastPosition
    {
        get { return PlayerPrefs.GetFloat("StateGarage.LastPosition", 0); }
        set { PlayerPrefs.SetFloat("StateGarage.LastPosition", value); }
    }

    public State_Garage Setup(System.Action onNextTask)
    {
        OnNextTask = onNextTask;
        return this;
    }

    private void Start()
    {
        GarageCamera.SetCameraId(1);
        UiHeader.Show();
        UiShowHide.ShowAll(transform);

        inventoryButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Confirm>().Setup(111103, false, null));

        var cars = RacerFactory.Racer.AllConfigs;
        cars.Sort((x, y) => x.GroupId == y.GroupId ? x.Id - y.Id : x.GroupId - y.GroupId);

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
                }
                else
                {
                    Profile.SelectedRacer = car.Id;
                    OnNextTask();
                }
            }
            ).rectTransform.anchoredPosition = itempos;

            itempos.x += itempos.y < -1 ? (itemPrefab.rectTransform.rect.width + itemSpace) : 0;
            itempos.y = itempos.y < -1 ? 0 : -(itemPrefab.parentRectTransform.rect.height - itemPrefab.rectTransform.rect.height);
        }
        itemPrefab.parentRectTransform.SetAnchordWidth(itempos.x + itemPrefab.rectTransform.rect.width + itemSpace);

        container = itemPrefab.parentRectTransform;
        container.SetAnchordPositionX(LastPosition);

        Destroy(separatorPrefab.gameObject);
        Destroy(itemPrefab.gameObject);
    }

    public override float PreClose()
    {
        if (container != null)
            LastPosition = container.anchoredPosition.x;
        return base.PreClose();
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static System.Action OnNextTask = () => gameManager.OpenState<State_Upgrade>();
}
