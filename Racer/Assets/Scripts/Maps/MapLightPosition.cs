using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLightPosition : MonoBehaviour
{
    [SerializeField] private float range = 1;
    [SerializeField] private float speed = 1;

    private Vector3 currentRotation = Vector3.zero;

    // Use this for initialization
    private void Start()
    {
        currentRotation = transform.localEulerAngles;
    }

    // Update is called once per frame
    private void Update()
    {
        var rot = currentRotation;
        rot.y += Mathf.Sin(Time.time * speed) * range;
        transform.localEulerAngles = rot;
    }
}
