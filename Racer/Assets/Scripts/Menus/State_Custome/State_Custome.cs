using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Custome : GameState
{
    [SerializeField] private RacerCustomPresenter noneCustome = null;
    [SerializeField] private UiCustomeItem colorPrefab = null;
    [SerializeField] private UiCustomeItem itemPrefab = null;
    [SerializeField] private UiCustomePackage package = null;
    [SerializeField] private Button assignButton = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private RectTransform colorSelected = null;
    [SerializeField] private UiShowHide colorsShowHide = null;
    [SerializeField] private UiShowHide custumsShowHide = null;
    [SerializeField] private UiShowHide itemsShowHide = null;
    [SerializeField] private UiShowHide leftPanelShowHide = null;

    private RacerCustomeType selectedCustome = RacerCustomeType.None;
    private RacerPresenter racer = null;
    private RacerConfig config = null;
    private int defaultCustomId = 0;
    private int defaultColorId = 0;
    private bool isCustomeLocked = false;

    private void Start()
    {
        GarageCamera.SetCameraId(0);
        custumsShowHide.Show();
        leftPanelShowHide.Hide();
        racer = GarageRacer.racer;
        config = RacerFactory.Racer.GetConfig(racer.Id);

        colorPrefab.parentRectTransform.RemoveChildrenBut(colorPrefab.transform, colorSelected);
        colorPrefab.gameObject.SetActive(true);
        foreach (var item in RacerFactory.Colors.AllColors)
            colorPrefab.Clone<UiCustomeItem>().Setup(item.color, item.id, OnColorSelected);
        colorPrefab.gameObject.SetActive(false);
        colorSelected.SetAsLastSibling();

        assignButton.onClick.AddListener(OnAssignButtonClick);

        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(92));
    }

    public override float PreClose()
    {
        GarageRacerImager.RemoveImageOpaque(racer.Id);
        ValidateChanges();
        return base.PreClose();
    }

    public void OnCostumeSelected(int index)
    {
        if (selectedCustome != RacerCustomeType.None) Back();
        selectedCustome = (RacerCustomeType)index;
        switch (selectedCustome)
        {
            case RacerCustomeType.BodyColor: DisplayCustomes(RacerFactory.Colors.GetModels(racer.Id), racer.ColorModel, racer.BodyColor, false); break;
            case RacerCustomeType.Vinyl: DisplayCustomes(RacerFactory.Vinyl.GetPrefabs(racer.Id), racer.Vinyl, racer.VinylColor, true); break;
            case RacerCustomeType.Wheel: DisplayCustomes(RacerFactory.Wheel.GetPrefabs(racer.Id), racer.Wheel, racer.RimColor, false); break;
            case RacerCustomeType.Spoiler: DisplayCustomes(RacerFactory.Spoiler.GetPrefabs(racer.Id), racer.Spoiler, racer.SpoilerColor, true); break;
            case RacerCustomeType.Roof: DisplayCustomes(RacerFactory.Roof.GetPrefabs(racer.Id), racer.Roof, racer.RoofColor, true); break;
            case RacerCustomeType.Hood: DisplayCustomes(RacerFactory.Hood.GetPrefabs(racer.Id), racer.Hood, racer.HoodColor, true); break;
            case RacerCustomeType.Height: DisplayCustomes(RacerFactory.Height.GetPrefabs(racer.Id), racer.Height, 0, false); break;
            case RacerCustomeType.WindowColor: DisplayColor(defaultColorId = racer.WindowColor); break;
            case RacerCustomeType.LightsColor: DisplayColor(defaultColorId = racer.LightsColor); break;
        }
        package.Setup(selectedCustome, racer.Id, () => OnCostumeSelected(index));
        leftPanelShowHide.Show();
    }

    public void DisplayCustomes(List<RacerCustomPresenter> list, int currCustom, int currColor, bool displayDefault)
    {
        defaultCustomId = currCustom;
        defaultColorId = currColor;

        itemPrefab.parentRectTransform.RemoveChildrenBut(0);
        itemPrefab.gameObject.SetActive(true);

        if (selectedCustome == RacerCustomeType.BodyColor)
        {
            foreach (var item in list)
                itemPrefab.Clone<UiCustomeItem>()
                    .Setup(item, LocalizationService.Get(111040 + item.Id), currCustom == item.Id, OnCostumeItemSelected)
                    .SetIntractable(Profile.IsUnlockedCustom(selectedCustome, racer.Id, item.Id));
        }
        else
        {
            int index = 1;
            foreach (var item in list)
                itemPrefab.Clone<UiCustomeItem>()
                    .Setup(item, index++.ToString(), currCustom == item.Id, OnCostumeItemSelected)
                    .SetIntractable(Profile.IsUnlockedCustom(selectedCustome, racer.Id, item.Id));
        }

        if (displayDefault)
            itemPrefab.Setup(noneCustome, "00", currCustom == 0, OnCostumeItemSelected);
        else
            itemPrefab.gameObject.SetActive(false);

        custumsShowHide.Hide();
        itemsShowHide.Show();
    }


    public void OnCostumeItemSelected(RacerCustomPresenter custome)
    {
        isCustomeLocked = Profile.IsUnlockedCustom(selectedCustome, racer.Id, custome.Id) == false;

        switch (selectedCustome)
        {
            case RacerCustomeType.BodyColor:
                racer.ColorModel = custome.Id;
                DisplayColor(racer.BodyColor);
                break;
            case RacerCustomeType.Vinyl:
                racer.Vinyl = custome.Id;
                if (custome.colorable)
                    DisplayColor(racer.VinylColor);
                else
                    racer.VinylColor = RacerFactory.Colors.Default;
                break;
            case RacerCustomeType.Wheel:
                racer.Wheel = custome.Id;
                if (custome.colorable)
                    DisplayColor(racer.RimColor);
                else
                    racer.RimColor = RacerFactory.Colors.Default;
                break;
            case RacerCustomeType.Spoiler:
                racer.Spoiler = custome.Id;
                if (custome.colorable)
                    DisplayColor(racer.SpoilerColor);
                else
                    racer.SpoilerColor = RacerFactory.Colors.Default;
                break;
            case RacerCustomeType.Roof:
                racer.Roof = custome.Id;
                if (custome.colorable)
                    DisplayColor(racer.RoofColor);
                else
                    racer.RoofColor = RacerFactory.Colors.Default;
                break;
            case RacerCustomeType.Hood:
                racer.Hood = custome.Id;
                if (custome.colorable)
                    DisplayColor(racer.HoodColor);
                else
                    racer.HoodColor = RacerFactory.Colors.Default;
                break;
            case RacerCustomeType.Height: racer.Height = custome.Id; break;
        }

        if (custome.colorable == false)
            colorsShowHide.Hide();

        UpdateAssignButton();
    }

    private void DisplayColor(int currColor)
    {
        UiCustomeItem.SetDefaultColorSelected(colorPrefab.parentRectTransform, currColor);
        colorsShowHide.Show();
    }

    private void OnColorSelected(int id, Vector2 pos)
    {
        colorSelected.anchoredPosition = pos;
        switch (selectedCustome)
        {
            case RacerCustomeType.BodyColor: racer.BodyColor = id; break;
            case RacerCustomeType.Vinyl: racer.VinylColor = id; break;
            case RacerCustomeType.Wheel: racer.RimColor = id; break;
            case RacerCustomeType.Spoiler: racer.SpoilerColor = id; break;
            case RacerCustomeType.Roof: racer.RoofColor = id; break;
            case RacerCustomeType.Hood: racer.HoodColor = id; break;
            case RacerCustomeType.WindowColor: racer.WindowColor = id; break;
            case RacerCustomeType.LightsColor: racer.LightsColor = id; break;
        }
        UpdateAssignButton();
    }

    public void UpdateAssignButton()
    {
        switch (selectedCustome)
        {
            case RacerCustomeType.BodyColor:
                priceLabel.SetFormatedText(config.BodyColorCost);
                assignButton.transform.SetActiveChild((defaultCustomId == racer.ColorModel && defaultColorId == racer.BodyColor) ? 2 : 3);
                assignButton.SetInteractable(assignButton.transform.GetActiveChild() != 2);
                return;
            case RacerCustomeType.WindowColor:
                priceLabel.SetFormatedText(config.WindowColorCost);
                assignButton.transform.SetActiveChild(defaultColorId == racer.WindowColor ? 2 : 3);
                assignButton.SetInteractable(assignButton.transform.GetActiveChild() != 2);
                return;
            case RacerCustomeType.LightsColor:
                priceLabel.SetFormatedText(config.LightsColorCost);
                assignButton.transform.SetActiveChild(defaultColorId == racer.LightsColor ? 2 : 3);
                assignButton.SetInteractable(assignButton.transform.GetActiveChild() != 2);
                return;
        }

        if (isCustomeLocked)
        {
            assignButton.transform.SetActiveChild(0);
            assignButton.SetInteractable(false);
            return;
        }

        switch (selectedCustome)
        {
            case RacerCustomeType.Height: assignButton.transform.SetActiveChild(defaultCustomId == racer.Height ? 2 : 1); break;
            case RacerCustomeType.Hood: assignButton.transform.SetActiveChild(defaultCustomId == racer.Hood && defaultColorId == racer.HoodColor ? 2 : 1); break;
            case RacerCustomeType.Horn: assignButton.transform.SetActiveChild(defaultCustomId == racer.Horn ? 2 : 1); break;
            case RacerCustomeType.Roof: assignButton.transform.SetActiveChild(defaultCustomId == racer.Roof && defaultColorId == racer.RoofColor ? 2 : 1); break;
            case RacerCustomeType.Spoiler: assignButton.transform.SetActiveChild(defaultCustomId == racer.Spoiler && defaultColorId == racer.SpoilerColor ? 2 : 1); break;
            case RacerCustomeType.Vinyl: assignButton.transform.SetActiveChild(defaultCustomId == racer.Vinyl && defaultColorId == racer.VinylColor ? 2 : 1); break;
            case RacerCustomeType.Wheel: assignButton.transform.SetActiveChild(defaultCustomId == racer.Wheel && defaultColorId == racer.RimColor ? 2 : 1); break;
        }

        assignButton.SetInteractable(assignButton.transform.GetActiveChild() != 2);
    }

    private void OnAssignButtonClick()
    {
        var rp = Profile.GetRacer(racer.Id);
        switch (selectedCustome)
        {
            case RacerCustomeType.BodyColor:
                Game.SpendGem(config.BodyColorCost, () =>
                {
                    rp.custom.ColorModel = defaultCustomId = racer.ColorModel;
                    rp.custom.BodyColor = defaultColorId = racer.BodyColor;
                });
                break;

            case RacerCustomeType.WindowColor: Game.SpendGem(config.WindowColorCost, () => rp.custom.WindowColor = defaultColorId = racer.WindowColor); break;

            case RacerCustomeType.LightsColor: Game.SpendGem(config.LightsColorCost, () => rp.custom.LightsColor = defaultColorId = racer.LightsColor); break;

            case RacerCustomeType.Vinyl:
                rp.custom.Vinyl = defaultCustomId = racer.Vinyl;
                rp.custom.VinylColor = defaultColorId = racer.VinylColor;
                break;

            case RacerCustomeType.Wheel:
                rp.custom.Wheel = defaultCustomId = racer.Wheel;
                rp.custom.RimColor = defaultColorId = racer.RimColor;
                break;

            case RacerCustomeType.Spoiler:
                rp.custom.Spoiler = defaultCustomId = racer.Spoiler;
                rp.custom.SpoilerColor = defaultColorId = racer.SpoilerColor;
                break;

            case RacerCustomeType.Roof:
                rp.custom.Roof = defaultCustomId = racer.Roof;
                rp.custom.RoofColor = defaultColorId = racer.RoofColor;
                break;

            case RacerCustomeType.Hood:
                rp.custom.Hood = defaultCustomId = racer.Hood;
                rp.custom.HoodColor = defaultColorId = racer.HoodColor;
                break;

            case RacerCustomeType.Height:
                rp.custom.Height = defaultCustomId = racer.Height;
                break;
        }

        UpdateAssignButton();
    }

    public override void Back()
    {
        if (selectedCustome != RacerCustomeType.None)
        {
            ValidateChanges();
            colorsShowHide.Hide();
            itemsShowHide.Hide();
            custumsShowHide.Show();
            selectedCustome = RacerCustomeType.None;
            leftPanelShowHide.Hide();
        }
        else base.Back();
    }

    private void ValidateChanges()
    {
        switch (selectedCustome)
        {
            case RacerCustomeType.BodyColor:
                racer.ColorModel = defaultCustomId;
                racer.BodyColor = defaultColorId;
                break;
            case RacerCustomeType.Vinyl:
                racer.Vinyl = defaultCustomId;
                racer.VinylColor = defaultColorId;
                break;
            case RacerCustomeType.Wheel:
                racer.Wheel = defaultCustomId;
                racer.RimColor = defaultColorId;
                break;
            case RacerCustomeType.Spoiler:
                racer.Spoiler = defaultCustomId;
                racer.SpoilerColor = defaultColorId;
                break;
            case RacerCustomeType.Roof:
                racer.Roof = defaultCustomId;
                racer.RoofColor = defaultColorId;
                break;
            case RacerCustomeType.Hood:
                racer.Hood = defaultCustomId;
                racer.HoodColor = defaultColorId;
                break;
            case RacerCustomeType.Height: racer.Height = defaultCustomId; break;
            case RacerCustomeType.WindowColor: racer.WindowColor = defaultColorId; break;
            case RacerCustomeType.LightsColor: racer.LightsColor = defaultColorId; break;
        }
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
}
