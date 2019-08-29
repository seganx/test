using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-90)]
public class UiPlayingGesture : Base, IEndDragHandler, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 beginDragPos = Vector2.zero;

    public static bool GoToLeft { get; private set; }
    public static bool GoToRight { get; private set; }
    public static bool UseNitors { get; private set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out beginDragPos);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out localPoint);

        var delta = localPoint - beginDragPos;
        if (delta.y > 60)
            UseNitors = true;
        //if (delta.magnitude > 30)
        //{
        //    delta.Normalize();
        //    if (Mathf.Abs(delta.x) < 0.5f && delta.y > 0.5f)
        //        UseNitors = true;
        //}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out localPoint);
        if (localPoint.x < -100)
            GoToLeft = true;
        else if (localPoint.x > 100)
            GoToRight = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out localPoint);
        if (localPoint.x < 0)
            GoToLeft = false;
        else if (localPoint.x > 0)
            GoToRight = false;
    }

    private void LateUpdate()
    {
        UseNitors = false;
    }
}
