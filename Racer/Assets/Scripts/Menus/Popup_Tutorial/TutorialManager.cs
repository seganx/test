using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Base
{
    #region fields
    public List<TutorialConfig> tutorialConfigs;
    #endregion

    #region properties
    #endregion

    #region methods
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.G))
        {
            CheckThenShowTutorial(1, 1f);
        }
    }

    public void CheckThenShowTutorial(int tutorialIndex, float delay)
    {
        TutorialConfig tutorialConfig = tutorialConfigs.Find(x => x.tutorialIndex == tutorialIndex);
        CheckThenShowTutorial(tutorialConfig, delay);
    }

    void CheckThenShowTutorial(TutorialConfig tutorialConfig, float delay)
    {
        //if (!IsTutorialShowed(tutorialConfig))
            DelayCall(delay, () => { ShowTutorial(tutorialConfig); });
    }

    private void ShowTutorial(TutorialConfig tutorialConfig)
    {
        SetTutorialShowed(tutorialConfig);
        gameManager.OpenPopup<Popup_Tutorial>().Setup(tutorialConfig, () =>
        {
            if (tutorialConfig.nextTutorialConfig)
                CheckThenShowTutorial(tutorialConfig.nextTutorialConfig, 0);
        });
    }

    public string TutorialshowedString(TutorialConfig tutorialConfig) { return "Tutorial_" + tutorialConfig.tutorialIndex; }
    public bool IsTutorialShowed(TutorialConfig tutorialConfig)
    {
        return PlayerPrefs.GetInt(TutorialshowedString(tutorialConfig), 0) > 0;
    }
    void SetTutorialShowed(TutorialConfig tutorialConfig)
    {
        PlayerPrefs.SetInt(TutorialshowedString(tutorialConfig), 1);
    }
    #endregion
}