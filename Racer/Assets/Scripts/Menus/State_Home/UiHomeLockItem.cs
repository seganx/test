using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class UiHomeLockItem : MonoBehaviour
{
    [SerializeField] private int totalRacesToUnlock = 0;
    [SerializeField] private float disbaledScale = 0.8f;
    [SerializeField] private MaskableGraphic[] images = null;
    [SerializeField] private Button button = null;
    [SerializeField] private GameObject newLabelGameObject;

    private void Start()
    {
        if (newLabelGameObject)
            newLabelGameObject.SetActive(false);

        if (Profile.TotalRaces < totalRacesToUnlock)
        {
            transform.localScale = Vector3.one * disbaledScale;

            foreach (var image in images)
                image.color = button.colors.disabledColor;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Confirm>().Setup(111142, false, null));
        }
        else
        {
            SetNewLabelGameObjectActive();
        }
    }

    private void Reset()
    {
        button = GetComponent<Button>();
        images = GetComponentsInChildren<MaskableGraphic>(true);
    }

    string openedOnceString() { return "HomeSectionOpenedOnce" + name.ToString(); }
    public void SetOpenedOnce()
    {
        PlayerPrefs.SetInt(openedOnceString(), 1);
        SetNewLabelGameObjectActive();
    }
    bool GetOpenedOnce() { return PlayerPrefs.GetInt(openedOnceString()) == 1; }

    void SetNewLabelGameObjectActive()
    {
        if (newLabelGameObject)
            newLabelGameObject.SetActive(!GetOpenedOnce());
    }
}
