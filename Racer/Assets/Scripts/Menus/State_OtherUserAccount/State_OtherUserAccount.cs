using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class State_OtherUserAccount : GameState
{
    [SerializeField] private RacerConfig[] racerConfigs;
    [SerializeField] private Button nextToturialButton;
    [SerializeField] private Button prevToturialButton;
    int currentToturialPageIndex;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
        UpdateRacer();

        nextToturialButton.onClick.AddListener(() =>
        {
            currentToturialPageIndex++;
            UpdateRacer();
        });

        prevToturialButton.onClick.AddListener(() =>
        {
            currentToturialPageIndex--;
            UpdateRacer();
        });
    }

    void UpdateRacer()
    {
        prevToturialButton.gameObject.SetActive(currentToturialPageIndex > 0);
        nextToturialButton.gameObject.SetActive(currentToturialPageIndex < racerConfigs.Length - 1);
    }
}
