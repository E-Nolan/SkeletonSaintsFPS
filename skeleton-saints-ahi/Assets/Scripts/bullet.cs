using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Dmg for the bullet
    public int bulletDmg;

    // Timer for how long the before the bullet is destroyed if no hit occurs
    [SerializeField] int timer;

   
    void Start()
    {
        // Destroys the bullet after X amount of time
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Use the IDamage interface to damage what the bullet collides with
        if (other.GetComponent<IDamage>() != null)
        {
            other.GetComponent<IDamage>().TakeDamage(bulletDmg);
        }

        Destroy(gameObject);
    }


}
