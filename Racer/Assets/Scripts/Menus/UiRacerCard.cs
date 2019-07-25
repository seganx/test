using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class UiRacerCard : MonoBehaviour
{
    [SerializeField] private LocalText groupLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Image image = null;

    public UiRacerCard Setup(RacerConfig config)
    {
        image.sprite = config.halfIcon;
        groupLabel.SetText(config.GroupId.ToString());
        nameLabel.text = config.Name;
        return this;
    }

    public UiRacerCard Setup(int racerId)
    {
        var config = RacerFactory.Racer.GetConfig(racerId);
        return Setup(config);
    }
}
