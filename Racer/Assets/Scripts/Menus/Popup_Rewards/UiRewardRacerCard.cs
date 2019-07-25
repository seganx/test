using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRewardRacerCard : MonoBehaviour
{
    [SerializeField] private LocalText countLabel = null;
    [SerializeField] private LocalText groupLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Image image = null;

    public UiRewardRacerCard Setup(RacerConfig config, int count)
    {
        countLabel.SetFormatedText(count);
        groupLabel.SetText(config.GroupId.ToString());
        image.sprite = config.halfIcon;
        nameLabel.text = config.Name;
        return this;
    }

    public UiRewardRacerCard Setup(int racerId, int count)
    {
        var config = RacerFactory.Racer.GetConfig(racerId);
        return Setup(config, count);
    }
}
