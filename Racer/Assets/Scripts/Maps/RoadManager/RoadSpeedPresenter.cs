using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(3000)]
public class RoadSpeedPresenter : MonoBehaviour
{
    public static float CurrentSpeed = 0;

    public Renderer roadRenderer = null;

    private void Reset()
    {
        if (roadRenderer == null)
            roadRenderer = GetComponent<MeshRenderer>();
    }

    private void Awake()
    {
        Reset();
    }

    private void Update()
    {
        var speed = Mathf.Clamp01(CurrentSpeed / 100);
        roadRenderer.material.SetFloat("_Speed", speed);
    }
}
