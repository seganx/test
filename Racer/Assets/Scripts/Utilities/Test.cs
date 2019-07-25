using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Vector3 position = Vector3.zero;

    public Transform visual = null;

    Vector3 lastValue = Vector3.zero;

    // Update is called once per frame
    private void Update()
    {
        if (Time.deltaTime < Mathf.Epsilon) return;

        var delta = position - lastValue;
        lastValue = position;
        visual.localScale = delta / Time.deltaTime;
        //visual.localScale = Vector3.Lerp(visual.localScale, delta / Time.deltaTime, Time.deltaTime * 5);
    }
}
