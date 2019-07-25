using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCamera : MonoBehaviour
{
    public float blendSpeed = 5;
    public float fieldOfView = 20;
    [Header("Light Properties:")]
    public Vector3 lightDirection = Vector3.down;
    public float lightIntensity = 1;

    protected Vector3 destSpherical = Vector3.one;

    public int Id { set; get; }
    public Vector3 DestTarget { set; get; }
    public bool IsActive { get { return currentId == Id; } }

    private void Awake()
    {
        var str = name.Split('_');
        Id = str[0].ToInt(-1);
    }

    private void OnEnable()
    {
        if (Id < 0) return;
        all.Add(this);
        ComputeSpherical();
        DestTarget = transform.position + transform.forward * transform.position.magnitude;
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    protected virtual void Update()
    {
        if (currentId != Id) return;

#if UNITY_EDITOR
        if (Id > 0)
        {
            ComputeSpherical();
            DestTarget = transform.position + transform.forward * transform.position.magnitude;
        }
#endif

        Camera.main.transform.position = UpdateCameraPosition();
        UpdateCameraDirection();
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.deltaTime * 3);

        GarageLight.intensity = lightIntensity;
        GarageLight.direction = lightDirection;
    }

    protected virtual Vector3 ComputeSpherical()
    {
        destSpherical.x = transform.position.magnitude;
        destSpherical.y = Vector3.Angle(Vector3.up, transform.position.normalized) * Mathf.Deg2Rad;
        destSpherical.z = Vector3.SignedAngle(Vector3.right, transform.position.normalized, Vector3.down) * Mathf.Deg2Rad;
        return destSpherical;
    }

    protected virtual Vector3 UpdateCameraPosition()
    {
        var pos = Vector3.zero;
        var dest = destSpherical;
        dest.x *= radiusFactor;

        currSpherical = Vector3.Lerp(currSpherical, dest, blendSpeed * Time.deltaTime);
        pos.x = currSpherical.x * Mathf.Sin(currSpherical.y) * Mathf.Cos(currSpherical.z) * 0.75f;
        pos.y = currSpherical.x * Mathf.Cos(currSpherical.y);
        pos.z = currSpherical.x * Mathf.Sin(currSpherical.y) * Mathf.Sin(currSpherical.z);
        return pos;
    }

    protected virtual void UpdateCameraDirection()
    {
        currTarget = Vector3.Lerp(currTarget, DestTarget * radiusFactor, blendSpeed * Time.deltaTime);
        upvector = Vector3.Slerp(upvector, transform.up, blendSpeed * Time.deltaTime);
        Camera.main.transform.LookAt(currTarget, upvector);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position, this.name);

        if (UnityEditor.Selection.Contains(gameObject))
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane, Camera.main.aspect);
        }
    }
#endif


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static int currentId = 0;
    public static float radiusFactor = 1;
    public static List<GarageCamera> all = new List<GarageCamera>();

    private static Vector3 currSpherical = Vector3.one;
    private static Vector3 currTarget = Vector3.zero;
    private static Vector3 upvector = Vector3.up;

    public static GarageCamera SetCameraId(int id)
    {
        if (all.Count < 1) return null;
        var res = all.Find(x => x.Id == id);
        if (res == null) return null;

        if (currentId == id) return res;

        if (id == 0)
            res.ComputeSpherical();

        if (currentId == 0)
        {
            var free = all.Find(x => x.Id == 0);
            if (free) currSpherical = free.ComputeSpherical();
        }

        currentId = id;
        return res;
    }
}

