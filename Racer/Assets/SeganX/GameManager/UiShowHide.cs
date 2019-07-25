﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UiShowHide : Base
{
    [SerializeField] private float showDelay = 0;
    [SerializeField] private float showAlpha = 0;
    [SerializeField] private Vector2 showDirection = Vector3.up;
    [SerializeField] private float hideDelay = 0;
    [SerializeField] private float hideAlpha = 0;
    [SerializeField] private Vector2 hideDirection = Vector3.up;

    private Vector2 initPosition = Vector2.zero;
    private BlenderVector2 position = new BlenderVector2() { speed = 10, blendMode = BlenderVector2.BlendMode.Acceleration };
    private BlenderValue alpha = new BlenderValue() { speed = 5 };
    private CanvasGroup canvasGroup = null;

    public override bool Visible
    {
        get
        {
            return canvasGroup.interactable;
        }

        set
        {
            if (value)
                Show();
            else
                Hide();
        }
    }

    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    private void Awake()
    {
        initPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = initPosition - showDirection * 500;
        position.Setup(rectTransform.anchoredPosition);
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.alpha = showAlpha;
        alpha.Setup(showAlpha);
    }

    private void Start()
    {
        if (canvasGroup.interactable)
            Show();
    }

    // Update is called once per frame
    private void Update()
    {
        if (position.Update(Time.deltaTime))
            rectTransform.anchoredPosition = position.current;

        if (alpha.Update(Time.deltaTime))
            canvasGroup.alpha = alpha.current;
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        DelayCall(showDelay, () =>
        {
            position.destination = initPosition;
            alpha.destination = 1;
        });
    }

    public float Hide()
    {
        canvasGroup.interactable = false;
        DelayCall(hideDelay, () =>
        {
            position.destination = initPosition + hideDirection * 500;
            alpha.destination = hideAlpha;
        });
        return hideDelay + 0.5f;
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static List<UiShowHide> all = new List<UiShowHide>();

    public static void ShowAll(Transform parent)
    {
        foreach (var item in all)
            if (item.transform.IsChildOf(parent))
                item.Show();
    }

    public static float HideAll(Transform parent)
    {
        float res = 0;
        foreach (var item in all)
            if (item.transform.IsChildOf(parent))
            {
                var hidetime = item.Hide();
                if (hidetime > res)
                    res = hidetime;
            }
        return res;
    }
}
