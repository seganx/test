using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiGarageRacerItem : Base
{
    [SerializeField] private Image backgImage = null;
    [SerializeField] private Image racerImage = null;
    [SerializeField] private Image cardsImage = null;
    [SerializeField] private LocalText cardsLabel = null;
    [SerializeField] private LocalText rankLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Button unlockButton = null;

    public UiGarageRacerItem Setup(RacerConfig config, System.Action<UiGarageRacerItem> callback)
    {
        racerImage.sprite = config.icon;
        nameLabel.SetText(config.Name);
        nameLabel.rectTransform.parent.SetAnchordWidth(nameLabel.preferredWidth + 4);


        var racerprofile = Profile.GetRacer(config.Id);
        if (racerprofile == null)
        {
            backgImage.SetColorAlpha(0);
            racerImage.SetColorAlpha(0);
            cardsImage.SetColorAlpha(0);
            cardsLabel.SetFormatedText(0, config.CardCount);
            rankLabel.SetFormatedText(config.ComputePower(0, 0, 0), config.MaxPower);
        }
        else
        {
            var unlocked = config.IsUnlocked(racerprofile.cards);
            backgImage.SetColorAlpha(unlocked ? 1 : 0);
            racerImage.SetColorAlpha(1);
            cardsImage.SetColorAlpha(1);
            cardsImage.gameObject.SetActive(unlocked == false);
            cardsLabel.SetFormatedText(racerprofile.cards, config.CardCount);
            rankLabel.SetFormatedText(config.ComputePower(racerprofile.level.NitroLevel, racerprofile.level.SteeringLevel, racerprofile.level.BodyLevel), config.MaxPower);
        }

        transform.GetComponent<Button>(true, true).onClick.AddListener(() => callback(this));

        return this;
    }
}
