using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerCollisionContact : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9) return;
        other.gameObject.layer = 0;
        SendMessageUpwards("OnCrashed", SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != 9) return;
        other.gameObject.layer = 0;
        SendMessageUpwards("OnCrashed", SendMessageOptions.DontRequireReceiver);

        var rigid = other.transform.GetComponent<Rigidbody>();
        rigid.useGravity = true;
        rigid.AddForceAtPosition(Vector3.forward * RaceModel.stats.speed * 0.75f + Vector3.up * 7, other.contacts[0].point, ForceMode.Impulse);

        other.transform.GetComponentInParent<TrafficCar>().Shoot();
    }
}
