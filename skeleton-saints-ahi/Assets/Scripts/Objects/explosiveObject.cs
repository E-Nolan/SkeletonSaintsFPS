using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosiveObject : MonoBehaviour, IDamage
{
    [SerializeField] GameObject explosion;
    [SerializeField] bool explodeOnImpact;
    [SerializeField] bool explodeFromDamage;
    [SerializeField] bool explodeOnTimer;
    [Range(0,100)] [SerializeField] int health;
    [Range(0.0f, 10.0f)] [SerializeField] float detonationTimer;
    [SerializeField] AudioSource explosionAud;

    bool detonated;
    float currentHealth;

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        // If the object explodes after a timer, make it explode after the timer expires
        if (explodeOnTimer && detonationTimer > 0)
        {
            detonationTimer -= Time.deltaTime;
            if (detonationTimer <= 0 && !detonated)
                explode();
        }
    }

    public void TakeDamage(float damage)
    {
        // If the object explodes from damage, make it explode after its health is reduced to 0
        currentHealth -= damage;
        if (currentHealth <= 0 && explodeFromDamage && !detonated)
            explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the object explodes on impact, make it explode when it collides with anything
        if (explodeOnImpact && !detonated)
            explode();
    }

    void explode()
    {
        // Turn the object's model and gravity off, generate a damaging explosion at its center, and delete the object after the explosion finishes
        if (gameObject.GetComponent<MeshRenderer>())
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (gameObject.GetComponent<Rigidbody>())
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        if (gameObject.GetComponent<Collider>())
            gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5.0f);

        explosion.GetComponent<explosion>().explode();
    }
}
