using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_GameToturial : GameState
{
    [SerializeField] private GameObject[] toturialObjects;
    [SerializeField] private Button nextToturialButton;
    [SerializeField] private Button prevToturialButton;
    [SerializeField] private Button toturialRaceButton;

    int currentToturialPageIndex = 0;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
        UpdateCurrenctToturialPage();

        nextToturialButton.onClick.AddListener(() =>
        {
            currentToturialPageIndex++;
            UpdateCurrenctToturialPage();
        });

        prevToturialButton.onClick.AddListener(() =>
        {
            currentToturialPageIndex--;
            UpdateCurrenctToturialPage();
        });

        toturialRaceButton.onClick.AddListener(() =>
        {
        });
    }

    void UpdateCurrenctToturialPage()
    {
        foreach (var item in toturialObjects)
            item.SetActive(false);
        toturialObjects[currentToturialPageIndex].SetActive(true);

        prevToturialButton.gameObject.SetActive(currentToturialPageIndex > 0);
        nextToturialButton.gameObject.SetActive(currentToturialPageIndex < toturialObjects.Length - 1);
    }
}
