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
        // Checks to see if the tag of the object is player
        if (other.CompareTag("Player"))
        {
            // If true then causes players to take damage equal to the bullet damage
            gameManager.instance.playerScript.TakeDamage(bulletDmg);
        }
        // Destroys the bullet
        Destroy(gameObject);
    }


}
