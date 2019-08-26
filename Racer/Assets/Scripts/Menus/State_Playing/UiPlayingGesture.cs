using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UiPlayingGesture : Base, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    [SerializeField] private Vector2 useNitrosDirection = new Vector2(5, 10);

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
        if (Mathf.Abs(delta.x) < useNitrosDirection.x && delta.y > useNitrosDirection.y)
            UseNitors = true;
    }

    private void LateUpdate()
    {
        UseNitors = false;
    }
}
