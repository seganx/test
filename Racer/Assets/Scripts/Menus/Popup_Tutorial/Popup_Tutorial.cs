using System;
using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Tutorial : GameState
{
    #region fields
    public RectTransform[] darkImages;
    public RectTransform pointer, image_left, image_right, image_up, image_down;
    [SerializeField] RectTransform pointerRectTransform = null, dialogueRectTransform = null, alignParentRectTransform = null;
    [SerializeField] LocalText dialogueText = null;
    #endregion

    #region properties
    public RectTransform DialogueRectTransform { get { return dialogueRectTransform; } }
    #endregion

    #region methods
    public void Setup(TutorialConfig tutorialConfig, Action onTouch)
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            base.Back();
            onTouch();
        });

        switch (tutorialConfig.align)
        {
            case Align.Left:
                alignParentRectTransform.anchorMin = new Vector2(0, .5f);
                alignParentRectTransform.anchorMax = new Vector2(0, .5f);
                break;
            case Align.Right:
                alignParentRectTransform.anchorMin = new Vector2(1, .5f);
                alignParentRectTransform.anchorMax = new Vector2(1, .5f);
                break;
            case Align.Up:
                alignParentRectTransform.anchorMin = new Vector2(.5f, 1);
                alignParentRectTransform.anchorMax = new Vector2(.5f, 1);
                break;
            case Align.Down:
                alignParentRectTransform.anchorMin = new Vector2(.5f, 0);
                alignParentRectTransform.anchorMax = new Vector2(.5f, 0);
                break;
        }



        if (string.IsNullOrEmpty(tutorialConfig.dialogueString))
            dialogueRectTransform.gameObject.SetActive(false);
        else
        {
            dialogueText.SetText(tutorialConfig.dialogueString.Replace("\\n", "\n"));
            dialogueRectTransform.anchoredPosition = tutorialConfig.dialoguePosition;
        }

        image_left.anchoredPosition = new Vector2(tutorialConfig.focusRect.x, 0);
        image_right.anchoredPosition = new Vector2(tutorialConfig.focusRect.xMax, 0);
        image_up.anchoredPosition = new Vector2(tutorialConfig.focusRect.x, tutorialConfig.focusRect.yMax);
        image_up.SetAnchordWidth(tutorialConfig.focusRect.width);
        image_down.anchoredPosition = new Vector2(tutorialConfig.focusRect.x, tutorialConfig.focusRect.y);
        image_down.SetAnchordWidth(tutorialConfig.focusRect.width);

        pointerRectTransform.anchoredPosition = tutorialConfig.tutorialPointer.position;
        switch (tutorialConfig.tutorialPointer.dir)
        {
            case TutorialDir.Left:
                pointer.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case TutorialDir.Right:
                pointer.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case TutorialDir.Up:
                pointer.rotation = Quaternion.Euler(0, 0, 180);
                break;
        }
    }

    public override void Back()
    {
    }
    #endregion
}