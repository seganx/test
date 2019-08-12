using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerTrafficCounter : MonoBehaviour
{
    private RacerPresenter racer = null;
    private TrafficCar trafficCar = null;

    public int NosChance { get; set; }
    public float NosMaxDistance { get; set; }
    public int TotalTrafficPassed { get; set; }
    public int TotalTrafficFailed { get; set; }
    public int TotalTrafficSuccess { get { return TotalTrafficPassed - TotalTrafficFailed; } }


    private void Awake()
    {
        NosChance = 100;
        NosMaxDistance = GlobalConfig.Race.nosMaxDistance;
    }

    private void Start()
    {
        racer = GetComponent<RacerPresenter>();
        if (racer == null) GetComponentInParent<RacerPresenter>();
        if (racer == null) Destroy(this);
    }

    private void FixedUpdate()
    {
        TrafficCar candid = null;
        for (int i = 0; i < TrafficCar.all.Count; i++)
        {
            var item = TrafficCar.all[i];
            if (transform.position.z.Between(item.transform.position.z - 1, item.transform.position.z + 1))
                candid = item;
        }

        if (candid != null && trafficCar == null)
        {
            trafficCar = candid;
        }
        else if (candid == null && trafficCar != null)
        {
            TotalTrafficPassed++;

            var disx = Mathf.Abs(racer.transform.position.x - trafficCar.transform.position.x) - (racer.Size.x + trafficCar.Size.x) * 0.5f;
            if (disx < NosMaxDistance && Random.Range(0, 100) < NosChance)
                SendMessageUpwards("AddNitors", SendMessageOptions.DontRequireReceiver);
            else
                TotalTrafficFailed++;

            trafficCar = null;
        }
    }
}
