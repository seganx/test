using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerTrafficCounter : MonoBehaviour
{
    private TrafficCar trafficCar = null;
    private float racerWidth = 0;

    public int EarnNosChance { get; set; }
    public float NosMaxDistance { get; set; }
    public int TotalTrafficPassed { get; set; }
    public int TotalTrafficFailed { get; set; }
    public int TotalTrafficSuccess { get { return TotalTrafficPassed - TotalTrafficFailed; } }


    private void Awake()
    {
        EarnNosChance = 100;
        NosMaxDistance = GlobalConfig.Race.nosMaxDistance;
    }

    private void Start()
    {
        var racer = GetComponent<RacerPresenter>();
        if (racer == null) GetComponentInParent<RacerPresenter>();
        if (racer == null)
            Destroy(this);
        else
            racerWidth = racer.Size.x;
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
            var disx = Mathf.Abs(transform.position.x - trafficCar.transform.position.x) - (racerWidth + trafficCar.Width) * 0.5f;

            TotalTrafficPassed++;
            if (disx < NosMaxDistance && Random.Range(0, 100) < EarnNosChance)
                SendMessageUpwards("AddNitors", SendMessageOptions.DontRequireReceiver);
            else
                TotalTrafficFailed++;

            trafficCar = null;
        }
    }
}
