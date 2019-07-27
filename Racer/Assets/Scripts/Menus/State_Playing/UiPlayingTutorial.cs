using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPlayingTutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialObjects;

    void Start()
    {
        if (!GetPlayingTutorialShowed(0))
            StartCoroutine(ShowThenHideTutorial(0, 5));

        if (!GetPlayingTutorialShowed(1))
            StartCoroutine(ShowThenHideTutorial(1, 13));

        if (GetPlayingTutorialShowed(2))
            enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(ShowThenHideTutorial(2));
            enabled = false;
        }
    }

    IEnumerator ShowThenHideTutorial(int index, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        tutorialObjects[index].SetActive(true);
        SetPlayingTutorialShowed(index);
        yield return new WaitForSeconds(5);
        tutorialObjects[index].SetActive(false);
    }

    string GetPlayingTutorialShowedString(int index) { return "PlayingTutorial_" + index; }
    bool GetPlayingTutorialShowed(int index)
    {
        return PlayerPrefs.GetInt(GetPlayingTutorialShowedString(index), 0) > 0;
    }
    void SetPlayingTutorialShowed(int index)
    {
        PlayerPrefs.SetInt(GetPlayingTutorialShowedString(index), 1);
    }
}
