using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(9000)]
public class RacerCamera : MonoBehaviour
{
    public enum Mode { Off, StickingFollower, QuadCopter, Cinematic, Driver, Front, SideLeft, SideRight }

    public static float fovScale = 1;
    public static Vector3 offset = Vector3.zero;
    public static float steeringValue = 0;

    public Mode mode = Mode.QuadCopter;

    private RacerCameraConfig config = null;
    private PlayerPresenter presenter = null;
    private Vector3 cameraOrigin = Vector3.zero;
    private Vector3 cameraTarget = Vector3.zero;
    private Vector3 cameraBounce = Vector3.zero;
    private Vector3 cameraPosition = Vector3.zero;
    private Vector3 cameraForward = Vector3.zero;
    private Vector3 cameraOffset = Vector3.zero;
    private float cameraBlend = 0;
    private static bool cameraBlendEnable = false;
    private float camHorizontalDelta = 0;


    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    private void Start()
    {
        config = RacerCameraConfig.Instance;
        presenter = GetComponentInParent<PlayerPresenter>();
        cameraOrigin = Camera.main.transform.position;
        //cameraOffset.z = -1.45f * presenter.racer.Size.z - transform.localPosition.z;
        //cameraOffset.y = presenter.racer.bluePrint.roof.position.y - 1.2f;
    }

    public void PlayingUpdate(float deltaTime)
    {
        switch (mode)
        {
            case Mode.StickingFollower: StickingFollower(deltaTime); break;
            case Mode.Cinematic: Cinematic(deltaTime); break;
            case Mode.QuadCopter: QuadCopter(deltaTime); break;
            case Mode.Front:
            case Mode.Driver:
            case Mode.SideLeft:
            case Mode.SideRight: StickToObject(deltaTime); break;
        }
    }

    private void ApplyToCamera(bool blendToThis, float fieldOfView, float deltaTime)
    {
        if (mode == config.currentMode)
        {
            cameraBlendEnable = blendToThis;
            if (blendToThis)
            {
                cameraBlend = Mathf.MoveTowards(cameraBlend, 1, deltaTime);
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPosition + cameraOffset, cameraBlend);
                Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, cameraForward, cameraBlend);
            }
            else
            {
                Camera.main.transform.position = cameraPosition + cameraOffset;
                Camera.main.transform.forward = cameraForward;
            }

            if (fieldOfView > 1)
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView * fovScale, deltaTime);
        }
        else if (cameraBlendEnable)
        {
            cameraBlend = Mathf.MoveTowards(cameraBlend, 0, deltaTime);
            if (cameraBlend > Mathf.Epsilon)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPosition + cameraOffset, cameraBlend);
                Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, cameraForward, cameraBlend);
            }
        }
    }

    private void StickingFollower(float deltaTime)
    {
        var pos = transform.position + offset;

        cameraOrigin.x = pos.x;
        cameraOrigin = cameraOrigin.LerpTo(pos, config.stickyFollower.originBlendSpeed * deltaTime);
        cameraTarget = cameraTarget.LerpTo(presenter.racer.cameraTargetTransform.position + config.stickyFollower.targetOffset, config.stickyFollower.targetBlendSpeed * deltaTime);

        //cameraBounce.x = Mathf.Lerp(cameraBounce.x, presenter.racer.transform.localPosition.x, config.stickyFollower.originBlendSpeed.x * deltaTime);
        float tempCamHorizontalPos = Mathf.Lerp(cameraBounce.x, presenter.racer.transform.localPosition.x, config.stickyFollower.originBlendSpeed.x * Time.deltaTime);
        if (steeringValue >= 0.5f)
        {
            if (camHorizontalDelta > tempCamHorizontalPos - presenter.racer.transform.localPosition.x)
                camHorizontalDelta = tempCamHorizontalPos - presenter.racer.transform.localPosition.x;
        }
        else if (steeringValue <= -0.5f)
        {
            if (camHorizontalDelta < tempCamHorizontalPos - presenter.racer.transform.localPosition.x)
                camHorizontalDelta = tempCamHorizontalPos - presenter.racer.transform.localPosition.x;
        }
        else camHorizontalDelta = tempCamHorizontalPos - presenter.racer.transform.localPosition.x;
        cameraBounce.x = presenter.racer.transform.localPosition.x + camHorizontalDelta;

        cameraForward = cameraTarget - cameraOrigin - cameraBounce;
        cameraBounce.z = Mathf.Lerp(cameraBounce.z, (pos.z - cameraOrigin.z) * config.stickyFollower.bounce, 5 * deltaTime);
        cameraPosition = cameraOrigin + cameraBounce;

        //cameraForward = transform.forward;
        //cameraPosition = pos;

        ApplyToCamera(config.stickyFollower.blendToThis, config.stickyFollower.fieldOfView, deltaTime);
    }

    private void QuadCopter(float deltaTime)
    {
        cameraOrigin = cameraOrigin.LerpTo(transform.position, config.quadCopter.originBlendSpeed * Time.deltaTime);
        cameraBounce.x = Mathf.Lerp(cameraBounce.x, presenter.racer.cameraTargetTransform.position.x - transform.position.x, config.quadCopter.originBlendSpeed.x * Time.deltaTime);
        cameraBounce.z = Mathf.Lerp(cameraBounce.z, transform.position.z - cameraOrigin.z, config.quadCopter.originBlendSpeed.z * Time.deltaTime);
        cameraPosition = cameraOrigin + cameraBounce * config.quadCopter.bounce;
        cameraTarget = cameraTarget.LerpTo(presenter.racer.cameraTargetTransform.position, config.quadCopter.targetBlendSpeed * Time.deltaTime);
        cameraForward = cameraTarget - cameraPosition;
        ApplyToCamera(config.quadCopter.blendToThis, config.quadCopter.fieldOfView, deltaTime);
    }

    private void Cinematic(float deltaTime)
    {
        cameraOrigin = cameraOrigin.LerpTo(transform.position, config.cinematic.originBlendSpeed * Time.deltaTime);
        cameraBounce.x = Mathf.Lerp(cameraBounce.x, transform.position.x - presenter.racer.cameraTargetTransform.position.x * 0.5f, config.cinematic.originBlendSpeed.x * Time.deltaTime);
        cameraBounce.z = Mathf.Lerp(cameraBounce.z, transform.position.z - cameraOrigin.z, config.cinematic.originBlendSpeed.z * Time.deltaTime);
        cameraPosition = cameraOrigin + cameraBounce * config.cinematic.bounce;
        cameraTarget = cameraTarget.LerpTo(presenter.racer.cameraTargetTransform.position, config.cinematic.targetBlendSpeed * Time.deltaTime);
        cameraForward = cameraTarget - cameraPosition;
        ApplyToCamera(config.cinematic.blendToThis, config.cinematic.fieldOfView, deltaTime);
    }

    private void StickToObject(float deltaTime)
    {
        cameraPosition = Vector3.Lerp(cameraPosition, transform.position, Time.deltaTime * 75f);
        cameraForward = Vector3.Lerp(cameraForward, transform.forward, Time.deltaTime * 75f);
        switch (mode)
        {
            case Mode.Driver: ApplyToCamera(config.driver.blendToThis, config.driver.fieldOfView, deltaTime); break;
            case Mode.Front: ApplyToCamera(config.front.blendToThis, config.front.fieldOfView, deltaTime); break;
            case Mode.SideLeft: ApplyToCamera(config.sideLeft.blendToThis, config.sideLeft.fieldOfView, deltaTime); break;
            case Mode.SideRight: ApplyToCamera(config.sideRight.blendToThis, config.sideRight.fieldOfView, deltaTime); break;
        }
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static List<RacerCamera> all = new List<RacerCamera>(10);

    public static void UpdateAll(float deltaTime)
    {
        for (int i = 0; i < all.Count; i++)
            all[i].PlayingUpdate(deltaTime);
    }
}
