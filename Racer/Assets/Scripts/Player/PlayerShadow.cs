
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private PlayerPresenter player = null;
    private Vector3 initScale = Vector3.zero;

    // Use this for initialization
    private void Start()
    {
        player = GetComponentInParent<PlayerPresenter>();
        initScale = transform.localScale;
    }

    // Update is called once per frame
    private void Update()
    {
        var ldir = GameMap.Current.sunSource.transform.forward;
        var scale = initScale.Scale(1 + Mathf.Abs(ldir.x), 0, 1 + Mathf.Abs(ldir.z));
        var posis = Vector3.one.Scale(scale.x - initScale.x, 0, scale.z - initScale.z) * 2;
        posis.x *= Mathf.Sign(ldir.x);
        posis.z *= Mathf.Sign(ldir.z);
        posis.x += player.racer.transform.localPosition.x;
        transform.localScale = scale;
        transform.localPosition = posis;
    }
}
