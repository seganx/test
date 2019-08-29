using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiGarageRacerItem : Base
{
    [SerializeField] private Image racerImage = null;
    [SerializeField] private Image cardsImage = null;
    [SerializeField] private LocalText cardsLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private GameObject unlockButton = null;

    public UiGarageRacerItem Setup(RacerConfig config, System.Action<UiGarageRacerItem> callback)
    {
        nameLabel.SetText(config.Name);
        nameLabel.rectTransform.parent.SetAnchordWidth(nameLabel.preferredWidth + 4);


        var racerprofile = Profile.GetRacer(config.Id);
        if (racerprofile == null)
        {
            racerImage.sprite = GarageRacerImager.GetImageOpaque(config.Id, config.DefaultRacerCustom, racerImage.rectTransform.rect.width, racerImage.rectTransform.rect.height);

            racerImage.color = Color.gray;
            racerImage.SetColorAlpha(0);
            cardsImage.SetColorAlpha(0);
            cardsLabel.SetFormatedText(0, config.CardCount);
            rankLabel.SetFormatedText(config.ComputePower(0, 0, 0, 0), config.MaxPower);
        }
        else
        {
            racerImage.sprite = GarageRacerImager.GetImageOpaque(config.Id, racerprofile.custom, racerImage.rectTransform.rect.width, racerImage.rectTransform.rect.height);

            var unlocking = Profile.IsUnlockingRacer(config.Id);
            var unlocked = Profile.IsUnlockedRacer(config.Id);

            racerImage.color = Color.white;
            racerImage.SetColorAlpha(unlocked ? 1 : 0.05f);
            cardsImage.SetColorAlpha(1);
            cardsImage.gameObject.SetActive(unlocked == false);
            cardsLabel.SetFormatedText(racerprofile.cards, config.CardCount);
            rankLabel.SetFormatedText(config.ComputePower(racerprofile.level.SpeedLevel, racerprofile.level.NitroLevel, racerprofile.level.SteeringLevel, racerprofile.level.BodyLevel), config.MaxPower);
            unlockButton.SetActive(unlocking);
        }

        var button = transform.GetComponent<Button>(true, true);
        button.onClick.AddListener(() =>
        {
            button.SetInteractable(false);
            callback(this);
            DelayCall(1, () => button.SetInteractable(true));
        });

        return this;
    }
}
