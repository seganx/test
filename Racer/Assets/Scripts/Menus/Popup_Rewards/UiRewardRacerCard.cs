using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRewardRacerCard : Base
{
    [SerializeField] private LocalText countLabel = null;
    [SerializeField] private LocalText groupLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Button openButton = null;
    [SerializeField] private SimpleAnimation animator = null;

    public bool IsOpened { get; private set; }

    private void Awake()
    {
        IsOpened = false;
        openButton.gameObject.SetActive(true);
    }

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

    public UiRewardRacerCard SetOnOpen(System.Action callback)
    {
        openButton.gameObject.SetActive(callback != null);
        openButton.onClick.AddListener(() =>
        {
            openButton.interactable = false;
            DelayCall(animator.PlayById(1).length, callback);
        });
        return this;
    }

}
