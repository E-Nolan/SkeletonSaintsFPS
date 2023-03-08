using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : collectionItem
{
    private void OnTriggerEnter(Collider other)
    {
        Collect();
        Destroy(gameObject);
    }
}
