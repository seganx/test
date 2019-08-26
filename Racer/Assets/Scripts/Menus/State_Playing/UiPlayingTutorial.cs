using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPlayingTutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialObjects;


    private IEnumerator Start()
    {
        HideAllTutorials();

        if (RaceModel.IsTutorial)
        {
            yield return new WaitForSecondsRealtime(6);
            tutorialObjects[0].SetActive(true);

            yield return HoldTime();

            yield return new WaitForSecondsRealtime(5);
            yield return new WaitUntil(() => PlayerPresenter.local.IsNitrosFull);
            tutorialObjects[1].SetActive(true);

            yield return HoldTimeForUseNitors();

            yield return new WaitForSecondsRealtime(3);
            yield return new WaitUntil(() => PlayerPresenter.local.Nitros < 0.01f);
            yield return new WaitForSecondsRealtime(1);
            tutorialObjects[2].SetActive(true);

            yield return HoldTime();

            yield return new WaitForSecondsRealtime(10);
            yield return new WaitUntil(() => PlayerPresenter.local.IsNitrosFull);
            yield return new WaitUntil(() => PlayerPresenter.local.IsNitrosUsing);
            yield return new WaitUntil(() => UiPlayingNitros.IsBoostInRange);
            tutorialObjects[3].SetActive(true);

            yield return HoldTimeForBoostNitros();
        }

        Destroy(gameObject);
    }

    private IEnumerator HoldTime()
    {
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(6);
        Time.timeScale = 1;
        HideAllTutorials();        
    }

    private IEnumerator HoldTimeForUseNitors()
    {
        Time.timeScale = 0.01f;
        yield return new WaitUntil(() => PlayerPresenter.local.IsNitrosUsing);
        Time.timeScale = 1;
        HideAllTutorials();
    }

    private IEnumerator HoldTimeForBoostNitros()
    {
        Time.timeScale = 0.01f;
        yield return new WaitWhile(() => UiPlayingNitros.IsBoostInRange);
        Time.timeScale = 1;
        HideAllTutorials();
    }

    private void HideAllTutorials()
    {
        for (int i = 0; i < tutorialObjects.Length; i++)
            tutorialObjects[i].SetActive(false);
    }
}
