using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UiPlayingGesture : MonoBehaviour, IDragHandler
{
    [SerializeField] private Vector2 useNitrosDirection = new Vector2(5, 10);

    public static bool UseNitors { get; private set; }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) < useNitrosDirection.x && eventData.delta.y > useNitrosDirection.y)
        {
            Debug.Log(eventData.delta);
            UseNitors = true;
        }
    }

    private void LateUpdate()
    {
        UseNitors = false;
    }
}
