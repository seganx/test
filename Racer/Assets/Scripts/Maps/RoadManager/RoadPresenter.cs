using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPresenter : MonoBehaviour
{
    public RoadSegment[] segments = null;
    public float segmentLenght = 600;
    public float segmentWidth = 6;

    private int displayIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateSegments();
    }

    private void LateUpdate()
    {
        int distance = (int)Camera.main.transform.position.z - 50;
        int index = distance / (int)segmentLenght;
        if (index == displayIndex) return;
        displayIndex = index;
        UpdateSegments();
    }

    private void OnValidate()
    {
        UpdateSegments();
    }

    private void UpdateSegments()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            var index = (displayIndex + i);
            segments[index % segments.Length].transform.SetPosition(0, 0, index * segmentLenght);
        }
    }

    private int GetSegmentId(ref float distance)
    {
        var res = (int)(distance / segments[0].Length);
        distance = distance % segments[0].Length;
        return res % segments.Length;
    }

    private Vector3 CalcPositionByDistance(float distance)
    {
        var index = GetSegmentId(ref distance);
        return segments[index].GetPositionByDistance(distance);
    }

    private Vector3 CalcForwardByDistance(float distance)
    {
        var index = GetSegmentId(ref distance);
        return segments[index].GetForwardByDistance(distance);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    /////////////////////////////////////////////////////////////////////////////////
    private static RoadPresenter instance = null;

    public static Vector3 GetPositionByDistance(float distance)
    {
        return instance.CalcPositionByDistance(distance);
    }

    public static Vector3 GetForwardByDistance(float distance)
    {
        return instance.CalcForwardByDistance(distance);
    }

    public static float RoadWidth { get { return instance.segmentWidth; } }
}
