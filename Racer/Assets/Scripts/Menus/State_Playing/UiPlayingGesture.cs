using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UiPlayingGesture : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [SerializeField] private Vector2 useNitrosDirection = new Vector2(5, 10);

    public static bool UseNitors { get; private set; }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) < useNitrosDirection.x && eventData.delta.y > useNitrosDirection.y)
            UseNitors = true;
    }

    private void LateUpdate()
    {
        UseNitors = false;
    }
}
