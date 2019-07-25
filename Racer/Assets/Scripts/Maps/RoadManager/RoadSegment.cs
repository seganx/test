using BansheeGz.BGSpline.Curve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadSegment : MonoBehaviour
{
    private BGCurveMathI curveMath = null;

    public float Length { get; private set; }

    private void Start()
    {
        curveMath = new BGCurveBaseMath(GetComponent<BGCurve>(), new BGCurveBaseMath.Config() { Parts = 16, Fields = BGCurveBaseMath.Fields.PositionAndTangent });
        Length = curveMath.GetDistance();
    }

    public Vector3 GetPositionByDistance(float distance)
    {
        return curveMath.CalcPositionByDistance(distance);
    }

    public Vector3 GetForwardByDistance(float distance)
    {
        return curveMath.CalcTangentByDistance(distance);
    }
}
