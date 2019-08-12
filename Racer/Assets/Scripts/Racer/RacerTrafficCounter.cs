using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerTrafficCounter : MonoBehaviour
{
    private RacerPresenter racer = null;
    private TrafficCar trafficCar = null;

    public int NosChance { get; set; }
    public float NosMaxDistanceOffset { get; set; }

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
            var disx = Mathf.Abs(racer.transform.position.x - trafficCar.transform.position.x) - (racer.Size.x + trafficCar.Size.x) * 0.5f;
            if (disx < GlobalConfig.Race.nosMaxDistance + NosMaxDistanceOffset && Random.Range(0, 100) < NosChance)
                SendMessageUpwards("AddNitors", SendMessageOptions.DontRequireReceiver);
            trafficCar = null;
        }
    }
}
