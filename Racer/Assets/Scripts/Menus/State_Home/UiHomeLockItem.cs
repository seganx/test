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

    private string OpenedOnceString { get { return "HomeSectionOpenedOnce" + name; } }
    private bool IsOpenedOnce { get { return PlayerPrefs.GetInt(OpenedOnceString) == 1; } }

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
            button.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Confirm>().Setup(111142, false, true, null));
        }
        else
        {
            UpdateVisual();

            button.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt(OpenedOnceString, 1);
                UpdateVisual();
            });
        }
    }

    void UpdateVisual()
    {
        if (newLabelGameObject)
            newLabelGameObject.SetActive(!IsOpenedOnce);
    }

    private void Reset()
    {
        button = GetComponent<Button>();
        images = GetComponentsInChildren<MaskableGraphic>(true);
    }

}
