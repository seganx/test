using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerTrafficCounter : MonoBehaviour
{
    private float width = 1;
    private TrafficCar trafficCar = null;

    public float SideDistance { set; get; }
    public float MinSizeDistance { set; get; }
    public float MaxSizeDistance { set; get; }
    public int TotalTrafficPassed { get; set; }
    public bool SideLeft { get; set; }

    private void Awake()
    {
        MinSizeDistance = GlobalConfig.Race.nosTrafficMinDistance;
        MaxSizeDistance = GlobalConfig.Race.nosTrafficMaxDistance;
    }

    private void Start()
    {
        var racer = transform.GetComponent<RacerPresenter>();
        width = racer.Size.x / 2;
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
            if (trafficCar.CanMove)
            {
                TotalTrafficPassed++;
                SideLeft = transform.position.x > trafficCar.transform.position.x;
                SideDistance = Mathf.Abs(transform.position.x - trafficCar.transform.position.x) - width - trafficCar.Width * 0.5f;                
                SendMessageUpwards("OnTrafficPassed", this, SendMessageOptions.DontRequireReceiver);
            }

            trafficCar = null;
        }
    }
}
