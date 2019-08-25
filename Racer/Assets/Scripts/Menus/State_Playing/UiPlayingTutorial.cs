using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPlayingTutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialObjects;


    private IEnumerator Start()
    {
        for (int i = 0; i < tutorialObjects.Length; i++)
            tutorialObjects[i].SetActive(false);

        if (RaceModel.IsTutorial)
        {
            yield return new WaitForSecondsRealtime(4.5f);
            tutorialObjects[0].SetActive(true);

            yield return HoldTime();

            yield return new WaitForSecondsRealtime(6);
            tutorialObjects[1].SetActive(true);

            yield return HoldTime();

            yield return new WaitForSecondsRealtime(8);
            tutorialObjects[2].SetActive(true);

            yield return HoldTime();

            yield return new WaitForSecondsRealtime(12);
            tutorialObjects[3].SetActive(true);

            yield return HoldTime();
        }

        Destroy(gameObject);
    }

    private IEnumerator HoldTime()
    {
        Time.timeScale = 0.4f;
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1;

        for (int i = 0; i < tutorialObjects.Length; i++)
            tutorialObjects[i].SetActive(false);
    }
}
