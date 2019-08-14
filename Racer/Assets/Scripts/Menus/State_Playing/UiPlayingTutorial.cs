﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPlayingTutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialObjects;

    private bool canDisplayNitros = true;

    private void Start()
    {
        if (!GetPlayingTutorialShowed(0))
            StartCoroutine(ShowThenHideTutorial(0, 5));

        if (!GetPlayingTutorialShowed(1))
            StartCoroutine(ShowThenHideTutorial(1, 13));

        if (GetPlayingTutorialShowed(0) && GetPlayingTutorialShowed(1) && GetPlayingTutorialShowed(2))
            Destroy(gameObject);
    }

    private void Update()
    {
        if (PlayerPresenter.local.NitrosReady)
        {
            if (canDisplayNitros)
            {
                canDisplayNitros = false;
                StartCoroutine(ShowThenHideTutorial(2));
            }
        }
        else tutorialObjects[2].gameObject.SetActive(false);
    }

    private IEnumerator ShowThenHideTutorial(int index, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        tutorialObjects[index].SetActive(true);
        SetPlayingTutorialShowed(index);
        yield return new WaitForSeconds(6);
        tutorialObjects[index].SetActive(false);

        if (index == 2) Destroy(gameObject);
    }

    private bool GetPlayingTutorialShowed(int index)
    {
        return PlayerPrefs.GetInt("PlayingTutorial_" + index, 0) > 0;
    }

    private void SetPlayingTutorialShowed(int index)
    {
        PlayerPrefs.SetInt("PlayingTutorial_" + index, 1);
    }
}
