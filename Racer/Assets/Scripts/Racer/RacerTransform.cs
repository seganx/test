using System.Collections;
using UnityEngine;


public class RacerTransform : MonoBehaviour
{
    private RacerPresenter racer = null;
    private Vector3 bodyPosition = Vector2.zero;

    private float wheelSteering = 0;

    private AcceleraReader accelera = new AcceleraReader(1);

    private void Start()
    {
        racer = GetComponent<RacerPresenter>();
        bodyPosition = racer.bodyTransform.localPosition;
    }

    private void Update()
    {
        var localPos = Vector3.one.Scale(transform.localPosition.x, transform.position.y, transform.position.z);
        accelera.Update(localPos);
        var velocity = accelera.velocity;

        transform.localRotation = transform.localRotation.LerpTo(0, velocity.x * 1.5f, 0, Time.deltaTime * 12);

        if (racer.IsShifting)
            racer.bodyTransform.localRotation = racer.bodyTransform.localRotation.LerpTo(0, 0, Mathf.Clamp(velocity.x, -5, 5), Time.deltaTime * 10);
        else
            racer.bodyTransform.localRotation = racer.bodyTransform.localRotation.LerpTo(Mathf.Clamp(-accelera.value.magnitude, -2, 2), 0, Mathf.Clamp(-velocity.x, -5, 5), Time.deltaTime * 6);

        racer.bodyTransform.localPosition = bodyPosition + Vector3.up * racer.Height * 0.03f + Vector3.down * Mathf.Clamp(accelera.y * 0.2f, -0.1f, 0.1f);

        if (racer.AutoWheelRotation)
        {
            foreach (var wheel in racer.rearWheels)
                wheel.RotationSpeed = velocity.z;

            foreach (var wheel in racer.frontWheels)
                wheel.RotationSpeed = velocity.z;
        }

        if (racer.AutoSteeringWheel)
        {
            wheelSteering = Mathf.Lerp(wheelSteering, velocity.x * 2, Time.deltaTime * 20);
            foreach (var wheel in racer.frontWheels)
                wheel.transform.localEulerAngles = Vector3.up * wheelSteering;
        }

    }
}