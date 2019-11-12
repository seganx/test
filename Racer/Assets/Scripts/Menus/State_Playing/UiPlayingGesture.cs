using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-90)]
public class UiPlayingGesture : Base, IEndDragHandler, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float easyRange = 0.1f;

    private Vector2 beginDragPos = Vector2.zero;
    private bool easyStarted = false;
    private float easyDest = 0;
    private float easyStep = 0;

    public static float Steering { get; private set; }
    public static bool UseNitors { get; private set; }

    private void Start()
    {
        easyStep = RoadPresenter.RoadWidth * 0.5f;
        Steering = 0;
        UseNitors = false;
    }

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
        else if (delta.x > 0)
        {

        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out localPoint);

        switch (RaceModel.specs.steering)
        {
            case RaceModel.SteeringMode.Normal:
                {
                    if (localPoint.x < -100)
                        Steering = -1;
                    else if (localPoint.x > 100)
                        Steering = 1;
                }
                break;

            case RaceModel.SteeringMode.Easy:
                {
                    if (easyStarted == false)
                    {
                        easyDest = PlayerPresenter.local.racer.transform.localPosition.x;
                        easyStarted = true;
                    }

                    if (localPoint.x < -100) // go to left
                        easyDest = easyDest < 1 ? -easyStep : 0;
                    else if (localPoint.x > 100)
                        easyDest = easyDest > -1 ? easyStep : 0;

                    easyDest += Random.Range(-0.4f, 0.4f);
                }
                break;

            case RaceModel.SteeringMode.Tilt:
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.enterEventCamera ?? eventData.pressEventCamera, out localPoint);

        switch (RaceModel.specs.steering)
        {
            case RaceModel.SteeringMode.Normal:
                if (localPoint.x < 0 && Steering < 0)
                    Steering = 0;
                else if (localPoint.x > 0 && Steering > 0)
                    Steering = 0;
                break;

            case RaceModel.SteeringMode.Easy:
                break;

            case RaceModel.SteeringMode.Tilt:
                break;
        }
    }

    private void Update()
    {
        if (PlayerPresenter.local == null) return;

        switch (RaceModel.specs.steering)
        {
            case RaceModel.SteeringMode.Normal:
                break;

            case RaceModel.SteeringMode.Easy:
                if (easyStarted)
                {
                    var delta = easyDest - PlayerPresenter.local.racer.transform.localPosition.x;
                    if (delta > easyRange)
                        Steering = Mathf.Clamp01(delta);
                    else if (delta < -easyRange)
                        Steering = Mathf.Clamp(delta, -1, 0);
                    else
                        Steering = 0;
                }
                break;

            case RaceModel.SteeringMode.Tilt:
                {
                    Steering = Mathf.Clamp(Input.acceleration.x / 0.25f, -1, 1);
                }
                break;
        }
    }

    private void LateUpdate()
    {
        UseNitors = false;
    }
}
