using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class FreeShopItemLabelPresenter : TimerPresenter
{
    [SerializeField] private GameObject labelGameObject;

    public override void Start()
    {
        base.Start();
        labelGameObject.SetActive(false);
    }

    public override void UpdateTimerText(int remainTime) { }

    public override void SetActiveTimerObjects(bool active)
    {
        labelGameObject.SetActive(!active);
    }
}