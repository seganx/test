using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCameraFree : GarageCamera
{
    [Header("Options:")]
    public Vector3 target = Vector3.up;
    [Range(0, 0.2f)]
    public float rotationSpeed = 0.1f;
    public Vector2 clampRadius = new Vector2(5, 11);
    public Vector2 clampTheta = new Vector2(0.2f, 1.57f);

    private float speed = 60;
    private Vector3 initPos = Vector3.zero;
    private bool isOneTouch = false;
    private bool isMultiTouch = true;

    protected override void Update()
    {
        if (currentId != Id) return;
        base.Update();

        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject) return;

        if (Input.touchCount == 2)
        {
            if (isMultiTouch)
            {
                isOneTouch = true;
                isMultiTouch = false;
                initPos = Input.touches[0].position - Input.touches[1].position;
            }
            else
            {
                var delta = Input.touches[0].position - Input.touches[1].position;
                destSpherical.x += 2 * (initPos.magnitude - delta.magnitude) / Screen.dpi;
                initPos = delta;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (isOneTouch)
            {
                isOneTouch = false;
                isMultiTouch = true;
                initPos = Input.mousePosition;
            }
            else
            {
                var delta = (Input.mousePosition - initPos) / Screen.dpi;
                destSpherical.z -= delta.x * speed * Time.deltaTime;
                destSpherical.y += delta.y * speed * Time.deltaTime;
                initPos = Input.mousePosition;
            }
        }
        else
        {
            isMultiTouch = true;
            isOneTouch = true;
            if (rotationSpeed > 0)
            {
                destSpherical.z += Time.deltaTime * rotationSpeed;
                destSpherical.y = 1.5f + 0.2f * Mathf.Sin(Time.time * rotationSpeed * 0.35f);
                destSpherical.x = 7;
            }
        }

        destSpherical.x += Input.mouseScrollDelta.y;
    }

    protected override Vector3 UpdateCameraPosition()
    {
        destSpherical.x = Mathf.Clamp(destSpherical.x, clampRadius.x, clampRadius.y);
        destSpherical.y = Mathf.Clamp(destSpherical.y, clampTheta.x, clampTheta.y);

        return transform.position = base.UpdateCameraPosition() + target + Vector3.up * destSpherical.y * 0.5f;
    }

    protected override void UpdateCameraDirection()
    {
        DestTarget = target + Vector3.up * destSpherical.y * 0.5f;
        transform.up = Vector3.up;
        var dir = transform.position.normalized * Mathf.PI * 0.5f;
        lightDirection.x = -dir.x;
        lightDirection.y = -Mathf.Cos(dir.x);
        lightDirection.z = dir.z - Mathf.Sin(dir.x) * 0.5f;
        base.UpdateCameraDirection();
#if UNITY_EDITOR
        transform.LookAt(DestTarget);
#endif
    }
}
