using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    [SerializeField] private Transform body = null;
    [SerializeField] private GameObject shadow = null;
    [SerializeField] private GameObject nights = null;

    private float pos;
    private float line;
    private bool canMove;
    private static List<MeshRenderer> meshes = new List<MeshRenderer>(5);
    private BoxCollider boxCollider = null;

    public Vector3 Size { get { return boxCollider != null ? boxCollider.size : Vector3.one; } }

    public TrafficCar Setup(int color, float line, float distanceVariance)
    {
        canMove = true;
        this.line = line;
        pos = PlayerPresenter.local.ForwardValue + GlobalConfig.Race.traffics.startDistance + distanceVariance;
        nights.SetActive(false);

        var carcolor = Color.HSVToRGB(color / 1000.0f, 1, 0.5f);
        meshes.Clear();
        transform.GetComponentsInChildren(true, meshes);
        foreach (var item in meshes)
        {
            if (item.material.HasProperty("_CarColor"))
            {
                item.material.SetColor("_CarColor", carcolor);
                break;
            }
        }
        return this;
    }

    public TrafficCar Shoot()
    {
        canMove = false;
        shadow.SetActive(false);
        return this;
    }

    private void Awake()
    {
        var rigid = body.GetComponent<Rigidbody>();
        rigid.interpolation = RigidbodyInterpolation.Interpolate;
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        boxCollider = body.GetComponent<BoxCollider>(true, true);
    }

    private void Update()
    {
        if (canMove)
        {
            pos += GlobalConfig.Race.traffics.carsSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, RoadPresenter.GetForwardByDistance(pos), Time.deltaTime * 10);
            transform.position = RoadPresenter.GetPositionByDistance(pos) + transform.right * line;
        }

        if (PlayerPresenter.local != null)
        {
            var distance = PlayerPresenter.local.transform.position.z - body.position.z;
            if (distance > 50) Destroy(gameObject);
        }
    }

    private void Reset()
    {
        body = transform.GetChild(0);
        shadow = body.FindRecursive("Shadow").gameObject;
        nights = body.FindRecursive("NightObjs").gameObject;
    }
}
