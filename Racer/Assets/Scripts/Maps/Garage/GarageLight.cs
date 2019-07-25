using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageLight : MonoBehaviour
{
    public Light mainlight = null;

    private void Update()
    {
        mainlight.intensity = Mathf.Lerp(mainlight.intensity, intensity, Time.deltaTime * 5);
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 10);
    }

    private void Reset()
    {
        if (mainlight == null)
            mainlight = transform.GetComponent<Light>(true, true);
    }

    private void OnValidate()
    {
        Reset();
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static Vector3 direction = Vector3.down;
    public static float intensity = 1;
}
