using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class UiHomeLockItem : MonoBehaviour
{
    [SerializeField] private int totalRacesToUnlock = 0;
    [SerializeField] private float disbaledScale = 0.8f;
    [SerializeField] private MaskableGraphic[] images = null;
    [SerializeField] private Button button = null;

    private void Start()
    {
        if (Profile.TotalRaces < totalRacesToUnlock)
        {
            transform.localScale = Vector3.one * disbaledScale;

            foreach (var image in images)
                image.color = button.colors.disabledColor;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Confirm>().Setup(111142, false, null));
        }
    }

    private void Reset()
    {
        button = GetComponent<Button>();
        images = GetComponentsInChildren<MaskableGraphic>(true);
    }
}
