using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerTrafficCounter : MonoBehaviour
{
    private TrafficCar trafficCar = null;

    public float SideDistance { set; get; }
    public int TotalTrafficPassed { get; set; }

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
                SideDistance = Mathf.Abs(transform.position.x - trafficCar.transform.position.x);
                SendMessageUpwards("OnTrafficPassed", this, SendMessageOptions.DontRequireReceiver);
            }

            trafficCar = null;
        }
    }
}
