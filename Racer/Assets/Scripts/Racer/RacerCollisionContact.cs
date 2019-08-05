using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerCollisionContact : MonoBehaviour
{
    private PlayerPresenter player = null;
    private TrafficCar trafficCar = null;
    private float trafficDistance = float.MaxValue;

    public int NosChance { get; set; }
    public float NosMaxDistanceOffset { get; set; }

    private void Awake()
    {
        NosChance = 100;
        NosMaxDistanceOffset = 0;
    }

    private void Start()
    {
        player = transform.GetComponentInParent<PlayerPresenter>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9) return;
        other.gameObject.layer = 0;
        player.OnCrashed();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != 9) return;
        other.gameObject.layer = 0;
        player.OnCrashed();

        var rigid = other.transform.GetComponent<Rigidbody>();
        rigid.useGravity = true;
        rigid.AddForceAtPosition(Vector3.forward * PlayModel.CurrentPlaying.speed * 0.75f + Vector3.up * 7, other.contacts[0].point, ForceMode.Impulse);

        other.transform.GetComponentInParent<TrafficCar>().Shoot();
    }

    private void FixedUpdate()
    {
        var tc = FindTrafficCar(1);
        if (tc == null)
            tc = FindTrafficCar(-1);

        //  drag begins
        if (tc != null && trafficCar == null)
        {
            trafficCar = tc;
            trafficDistance = float.MaxValue;
            FindMinimumDistance();
        }
        //  drag continues
        else if (tc != null && trafficCar != null)
        {
            FindMinimumDistance();
        }
        // drag end
        else if (tc == null && trafficCar != null)
        {
            if (trafficDistance < GlobalConfig.Race.nosMaxDistance + NosMaxDistanceOffset && Random.Range(0, 100) < NosChance)
                player.AddNitors();
            trafficCar = null;
        }
    }

    private TrafficCar FindTrafficCar(float leftright)
    {
        var dir = leftright * transform.right;
        RaycastHit hit;
        Physics.SphereCast(transform.position, 1, dir, out hit, 20, 1 << 9);
        return hit.collider ? hit.collider.GetComponentInParent<TrafficCar>() : null;
    }

    private void FindMinimumDistance()
    {
        var disx = Mathf.Abs(player.racer.transform.position.x - trafficCar.transform.position.x) - (player.racer.Size.x + trafficCar.Size.x) * 0.5f;
        if (disx < trafficDistance) trafficDistance = disx;
    }

#if OFF
    private void OnApplicationPause(bool pause)
    {
        undamageTime = 0;
    }
#endif
}
