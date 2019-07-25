using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRewardCustomeCard : MonoBehaviour
{
    [SerializeField] private Transform icons = null;
    [SerializeField] private LocalText customeLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Image image = null;

    public UiRewardCustomeCard Setup(RacerCustomeType type, RacerConfig config, int customeId)
    {
        int index = 0;
        string customeName = string.Empty;
        switch (type)
        {
            case RacerCustomeType.Hood:
                icons.RemoveChildrenBut(0);
                index = RacerFactory.Hood.GetPrefabs(config.Id).FindIndex(x => x.Id == customeId);
                customeName = LocalizationService.Get(111070);
                break;
            case RacerCustomeType.Roof:
                icons.RemoveChildrenBut(1);
                index = RacerFactory.Roof.GetPrefabs(config.Id).FindIndex(x => x.Id == customeId);
                customeName = LocalizationService.Get(111071);
                break;
            case RacerCustomeType.Spoiler:
                icons.RemoveChildrenBut(2);
                index = RacerFactory.Spoiler.GetPrefabs(config.Id).FindIndex(x => x.Id == customeId);
                customeName = LocalizationService.Get(111072);
                break;
            case RacerCustomeType.Vinyl:
                icons.RemoveChildrenBut(3);
                index = RacerFactory.Vinyl.GetPrefabs(config.Id).FindIndex(x => x.Id == customeId);
                customeName = LocalizationService.Get(111073);
                break;
            case RacerCustomeType.Wheel:
                icons.RemoveChildrenBut(4);
                index = RacerFactory.Wheel.GetPrefabs(config.Id).FindIndex(x => x.Id == customeId);
                customeName = LocalizationService.Get(111074);
                break;
            default: icons.RemoveChildren(); break;
        }

        image.sprite = config.halfIcon;
        nameLabel.text = config.Name;
        customeLabel.SetFormatedText(customeName, index + 1);

        return this;
    }

    public UiRewardCustomeCard Setup(RacerCustomeType type, int racerId, int customeId)
    {
        var config = RacerFactory.Racer.GetConfig(racerId);
        return Setup(type, config, customeId);
    }
}
