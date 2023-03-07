using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableObstacle : MonoBehaviour, IDamage
{
    [Range(1, 10)] [SerializeField] float health;
    
    [Header("----- Optional -----")]
    [SerializeField] GameObject droppedItem;
    [SerializeField] AudioSource destructionSound;

    public void TakeDamage(float damage)
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
            destructionSound.Play();
        }
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        Destroy(gameObject, 5.0f);
    }
}
