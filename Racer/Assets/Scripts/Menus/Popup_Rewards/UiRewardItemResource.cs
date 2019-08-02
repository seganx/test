using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiRewardItemResource : MonoBehaviour
{
    [SerializeField] private LocalText resourceLabel = null;
    private int value = 0;
    private float tlerp = 0;

    public UiRewardItemResource Setup(int resouce)
    {
        value = resouce;
        resourceLabel.SetText("0");
        transform.SetAsLastSibling();
        return this;
    }

    private void Update()
    {
        tlerp += (value > 10) ? (Time.deltaTime * 0.5f) : Time.deltaTime;
        resourceLabel.SetText(Mathf.RoundToInt(Mathf.Lerp(0, value, tlerp)).ToString("#,0"));
    }
}
