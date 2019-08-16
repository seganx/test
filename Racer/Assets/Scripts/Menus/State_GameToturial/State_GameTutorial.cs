using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_GameTutorial : GameState
{
    [SerializeField] private GameObject[] tutorialObjects;
    [SerializeField] private Button nextTutorialButton;
    [SerializeField] private Button prevTutorialButton;
    [SerializeField] private Button tutorialRaceButton;

    int currentTutorialPageIndex = 0;

    private void Start()
    {
        UiShowHide.ShowAll(transform);
        UpdateCurrenctTutorialPage();

        nextTutorialButton.onClick.AddListener(() =>
        {
            currentTutorialPageIndex++;
            UpdateCurrenctTutorialPage();
        });

        prevTutorialButton.onClick.AddListener(() =>
        {
            currentTutorialPageIndex--;
            UpdateCurrenctTutorialPage();
        });

        tutorialRaceButton.onClick.AddListener(() =>
        {
        });
    }

    void UpdateCurrenctTutorialPage()
    {
        foreach (var item in tutorialObjects)
            item.SetActive(false);
        tutorialObjects[currentTutorialPageIndex].SetActive(true);

        prevTutorialButton.gameObject.SetActive(currentTutorialPageIndex > 0);
        nextTutorialButton.gameObject.SetActive(currentTutorialPageIndex < tutorialObjects.Length - 1);
    }
}
