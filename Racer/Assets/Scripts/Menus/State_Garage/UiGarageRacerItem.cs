using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiGarageRacerItem : Base
{
    public static int racerImageWidth = 250;
    public static int racerImageHeight = 175;

    [SerializeField] private Image racerImage = null;
    [SerializeField] private Image cardsImage = null;
    [SerializeField] private LocalText cardsLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private GameObject unlockButton = null;
    [SerializeField] private GameObject discountButton = null;

    private RacerConfig config = null;
    private ShopLogic.SpecialOffer.Package offer = null;

    public UiGarageRacerItem Setup(RacerConfig config, System.Action<UiGarageRacerItem> callback)
    {
        this.config = config;

        // search and validate for special offer
        offer = ShopLogic.SpecialOffer.Packages.Find(x => x.racerId == config.Id);
        if (offer != null && ShopLogic.SpecialOffer.CanDisplay(offer) == false)
            offer = null;

        UpdateVisual();

        var button = transform.GetComponent<Button>(true, true);
        button.onClick.AddListener(() =>
        {
            button.SetInteractable(false);
            if (offer != null)
            {
                Game.Instance.OpenPopup<Popup_ShopSpecialPackage>().Setup(offer, pack =>
                {
                    if (pack == offer)
                    {
                        offer = null;
                        UpdateVisual();
                    }
                });
            }
            else callback(this);
            DelayCall(1, () => button.SetInteractable(true));
        });

        return this;
    }

    public void UpdateVisual()
    {
        if (config == null) return;

        nameLabel.SetText(config.Name);
        nameLabel.rectTransform.parent.SetAnchordWidth(nameLabel.preferredWidth + 4);

        if (offer != null)
        {
            discountButton.transform.GetComponent<LocalText>(true, true).SetFormatedText(offer.item.discount);
            discountButton.SetActive(true);
        }
        else discountButton.SetActive(false);

        var racerprofile = Profile.GetRacer(config.Id);
        if (racerprofile == null)
        {
            racerImage.sprite = GarageRacerImager.GetImageOpaque(config.Id, config.DefaultRacerCustom, racerImageWidth, racerImageHeight);

            racerImage.color = Color.gray;
            racerImage.SetColorAlpha(offer != null ? 1 : 0);
            cardsImage.SetColorAlpha(0);
            cardsLabel.SetFormatedText(0, config.CardCount);
            rankLabel.SetFormatedText(config.ComputePower(0, 0, 0, 0), config.MaxPower);
            unlockButton.SetActive(false);
        }
        else
        {
            racerImage.sprite = GarageRacerImager.GetImageOpaque(config.Id, racerprofile.custom, racerImageWidth, racerImageHeight);

            var unlocking = Profile.IsUnlockingRacer(config.Id);
            var unlocked = Profile.IsUnlockedRacer(config.Id);

            racerImage.color = Color.white;
            racerImage.SetColorAlpha(offer != null ? 1 : (unlocked ? 1 : 0.05f));
            cardsImage.SetColorAlpha(1);
            cardsImage.gameObject.SetActive(unlocked == false);
            cardsLabel.SetFormatedText(racerprofile.cards, config.CardCount);
            rankLabel.SetFormatedText(config.ComputePower(racerprofile.level.SpeedLevel, racerprofile.level.NitroLevel, racerprofile.level.SteeringLevel, racerprofile.level.BodyLevel), config.MaxPower);
            unlockButton.SetActive(unlocking);
        }

    }
}
