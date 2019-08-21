using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    [SerializeField] private Transform body = null;
    [SerializeField] private GameObject shadow = null;
    [SerializeField] private GameObject nights = null;

    private float forwardPosition;
    private float line;
    private static List<MeshRenderer> meshes = new List<MeshRenderer>(5);

    public bool CanMove { get; private set; }
    public float Width { get; private set; }

    public TrafficCar Setup(int color, float line, float distanceVariance)
    {
        CanMove = true;
        this.line = line;
        forwardPosition = RaceModel.stats.playerPosition + GlobalConfig.Race.traffics.startDistance + distanceVariance;
        nights.SetActive(false);

        var carcolor = Color.HSVToRGB(color / 1000.0f, 0.65f, 0.75f);
        return SetColor(carcolor);
    }

    public TrafficCar SetColor(Color color)
    {
        meshes.Clear();
        transform.GetComponentsInChildren(true, meshes);
        for (int i = 0; i < meshes.Count; i++)
            meshes[i].material.color = color;
        return this;
    }

    public TrafficCar Shoot()
    {
        CanMove = false;
        shadow.SetActive(false);
        return this;
    }

    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    private void Awake()
    {
        var rigid = body.GetComponent<Rigidbody>();
        rigid.interpolation = RigidbodyInterpolation.Interpolate;
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var boxCollider = body.GetComponent<BoxCollider>(true, true);
        Width = (boxCollider != null) ? boxCollider.size.x : 0;
    }

    private void Update()
    {
        if (CanMove)
        {
            forwardPosition += GlobalConfig.Race.traffics.carsSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, RoadPresenter.GetForwardByDistance(forwardPosition), Time.deltaTime * 10);
            transform.position = RoadPresenter.GetPositionByDistance(forwardPosition) + transform.right * line;
        }

        var distance = RaceModel.stats.playerPosition - body.position.z;
        if (distance > 50) Destroy(gameObject);
    }

    private void Reset()
    {
        body = transform.GetChild(0);
        shadow = body.FindRecursive("Shadow").gameObject;
        nights = body.FindRecursive("NightObjs").gameObject;
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static List<TrafficCar> all = new List<TrafficCar>(30);
}
