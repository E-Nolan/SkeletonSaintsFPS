using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableObstacle : MonoBehaviour, IDamage
{
    [Range(1, 10)] [SerializeField] int health;
    
    [Header("----- Optional -----")]
    [SerializeField] GameObject droppedItem;
    [SerializeField] AudioClip destructionSound;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            destruction();
    }

    void destruction()
    {
        if (droppedItem != null)
        {
            Instantiate(droppedItem, transform.position, droppedItem.transform.rotation);
        }
        if (destructionSound != null)
        {
            // TODO
        }
        Destroy(gameObject);
    }
}
