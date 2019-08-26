using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UiPlayingGesture : Base, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    private Vector2 beginDragPos = Vector2.zero;

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
        if (delta.magnitude > 30)
        {
            delta.Normalize();

            if (Mathf.Abs(delta.x) < 0.5f && delta.y > 0.5f)
                UseNitors = true;
        }
    }

    private void LateUpdate()
    {
        UseNitors = false;
    }
}
