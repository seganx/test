using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_GamePlayTutorial : GameState
{
    [SerializeField] private float waitDuration = 4;
    [SerializeField] private Button nosButton = null;
    [SerializeField] private Button nextTutorialButton = null;
    [SerializeField] private Button exitButton = null;
    [SerializeField] private LocalText tutorialText = null;
    [SerializeField] private Animation carsAnimation = null;
    private bool nosButtonPressed = false;
    private bool nextTutorialButtonPressed = false;
    private System.Action onCloseFunc = null;

    void Start()
    {
        nosButton.onClick.AddListener(() => nosButtonPressed = true);
        nosButton.SetInteractable(false);
        nextTutorialButton.onClick.AddListener(() => nextTutorialButtonPressed = true);
        nextTutorialButton.SetInteractable(false);
        exitButton.onClick.AddListener(() => Back());

        UiShowHide.ShowAll(transform);

        StartCoroutine(ShowTutorial());
    }

    public Popup_GamePlayTutorial SetOnClose(System.Action callback)
    {
        onCloseFunc = callback;
        return this;
    }

    public override void Back()
    {
        base.Back();
        if (onCloseFunc != null) onCloseFunc();
    }

    IEnumerator ShowTutorial()
    {
        carsAnimation.Play("arrow");
        tutorialText.SetText(LocalizationService.Get(111110));
        yield return new WaitForSeconds(waitDuration);
        yield return WaitForNextTutorialButton();
        tutorialText.SetText(LocalizationService.Get(111111));
        yield return new WaitForSeconds(waitDuration);
        yield return WaitForNextTutorialButton();
        tutorialText.SetText("");
        carsAnimation.Play("trafficCars");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText(LocalizationService.Get(111112));
        yield return WaitForNosButton();
        tutorialText.SetText("");
        carsAnimation.Play("player_0_1");
        yield return new WaitForSeconds(waitDuration);
        carsAnimation.Play("trafficCars");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText(LocalizationService.Get(111113));
        yield return WaitForNosButton();
        tutorialText.SetText("");
        carsAnimation.Play("player_1_2");
        yield return new WaitForSeconds(waitDuration);
        carsAnimation.Play("trafficCars");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText(LocalizationService.Get(111114));
        yield return WaitForNosButton();
        tutorialText.SetText("");
        carsAnimation.Play("player_2_3");
        yield return new WaitForSeconds(waitDuration);
        carsAnimation.Play("trafficCars");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText(LocalizationService.Get(111115));
        yield return WaitForNosButton();
        tutorialText.SetText("");
        carsAnimation.Play("player_3_4");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText(LocalizationService.Get(111116));
        yield return WaitForNextTutorialButton();
        carsAnimation.Play("player_4_3");
        yield return new WaitForSeconds(waitDuration);
        carsAnimation.Play("player_3_2");
        yield return new WaitForSeconds(waitDuration);
        carsAnimation.Play("player_2_1");
        yield return new WaitForSeconds(waitDuration);
        carsAnimation.Play("player_1_0");
        tutorialText.SetText("");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText(LocalizationService.Get(111117));
        yield return new WaitForSeconds(waitDuration);
        yield return WaitForNextTutorialButton();
        carsAnimation.Play("player_0_0");
        yield return new WaitForSeconds(waitDuration);
        tutorialText.SetText("");
        nextTutorialButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);
    }

    IEnumerator WaitForNosButton()
    {
        nosButton.SetInteractable(true);
        yield return new WaitUntil(() => nosButtonPressed == true);
        nosButton.SetInteractable(false);
        nosButtonPressed = false;
    }

    IEnumerator WaitForNextTutorialButton()
    {
        nextTutorialButton.SetInteractable(true);
        yield return new WaitUntil(() => nextTutorialButtonPressed == true);
        nextTutorialButton.SetInteractable(false);
        nextTutorialButtonPressed = false;
    }

    IEnumerator WaitForExitButton()
    {
        nextTutorialButton.SetInteractable(true);
        yield return new WaitUntil(() => nextTutorialButtonPressed == true);
        nextTutorialButton.SetInteractable(false);
        nextTutorialButtonPressed = false;
    }
}
