using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public class RacerCameraConfig : StaticConfig<RacerCameraConfig>
{
    public RacerCamera.Mode currentMode = RacerCamera.Mode.QuadCopter;

    [System.Serializable]
    public class BaseData
    {
        public float fieldOfView = 0;
        public bool blendToThis = true;
    }

    [System.Serializable]
    public class FollowerData : BaseData
    {
        public Vector3 originBlendSpeed = Vector3.one;
        public Vector3 targetBlendSpeed = Vector3.one;
        public float bounce = 0.95f;
    }

    [System.Serializable]
    public class StickyFollowerData : FollowerData
    {
        public Vector3 targetOffset = Vector3.zero;
    }

    public StickyFollowerData stickyFollower = new StickyFollowerData();
    public FollowerData quadCopter = new FollowerData();
    public FollowerData cinematic = new FollowerData();
    public BaseData driver = new BaseData();
    public BaseData front = new BaseData();
    public BaseData sideLeft = new BaseData();
    public BaseData sideRight = new BaseData();

    protected override void OnInitialize()
    {
        
    }
}
