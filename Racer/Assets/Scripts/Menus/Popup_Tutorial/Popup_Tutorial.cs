using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Tutorial : GameState
{
    [SerializeField] LocalText titleLabel = null;
    [SerializeField] LocalText descLabel = null;
    [SerializeField] Transform characters = null;
    [SerializeField] Button nextButton = null;

    public Popup_Tutorial Setup(TutorialConfig config, System.Action onFinished)
    {
        titleLabel.gameObject.SetActive(config.title.HasContent());
        titleLabel.SetText(config.title);
        descLabel.SetText(config.description);
        characters.SetActiveChild((int)config.character);

        nextButton.onClick.AddListener(() =>
        {
            nextButton.onClick.RemoveAllListeners();

            if (config.next == null)
            {
                base.Back();
                if (onFinished != null)
                    onFinished();
            }
            else Setup(config.next, onFinished);
        });

        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public override void Back()
    {

    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static Popup_Tutorial Display(int id, bool showOnce = true, System.Action onFinished = null)
    {
        if (showOnce)
        {
            if (PlayerPrefs.GetInt("Popup_Tutorial.Displayed." + id, 0) > 0)
                return null;
            PlayerPrefs.SetInt("Popup_Tutorial.Displayed." + id, 1);
        }

        var config = ResourceEx.Load<TutorialConfig>("Tutorials", id);
        if (config == null) return null;
        return gameManager.OpenPopup<Popup_Tutorial>().Setup(config, onFinished);
    }
}