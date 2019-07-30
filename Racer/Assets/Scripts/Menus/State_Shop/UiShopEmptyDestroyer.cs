using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiShopEmptyDestroyer : MonoBehaviour
{
    private void OnTransformChildrenChanged()
    {
        if (transform.childCount < 2) Destroy(gameObject);
    }
}
