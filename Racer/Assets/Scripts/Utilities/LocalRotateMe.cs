using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalRotateMe : MonoBehaviour
{
    public Vector3 speed = Vector3.zero;

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(speed.x, speed.y, speed.z, Space.Self);
    }
}
