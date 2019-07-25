using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPresenter : RacerCustomPresenter
{
    [SerializeField] private Transform tireTransform = null;

    public float RotationSpeed { get; set; }

    private void Update()
    {
        tireTransform.Rotate(RotationSpeed * Time.deltaTime * 200, 0, 0, Space.Self);
    }
}
