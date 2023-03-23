using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class homingMissile : MonoBehaviour
{
    [SerializeField] private GameObject _explosionObject;
    [SerializeField] private GameObject rocket;

    [Header("----- Missile Vars -----")]
    [Range(0.0f, 10.0f)] [SerializeField] float turnSpeed;
    [Range(0.0f, 10.0f)] [SerializeField] float accelerationRate;
    [Range(0.0f, 10.0f)] [SerializeField] float explosionRadius;
    [Range(0.0f, 10.0f)] [SerializeField] float explosionDamage;

    Vector3 toPlayerDir;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        toPlayerDir = (gameManager.instance.playerInstance.transform.position - transform.position).normalized;
        rb.velocity = transform.forward * (rb.velocity.magnitude + accelerationRate * Time.deltaTime * 2f);
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.LookRotation(toPlayerDir), Time.deltaTime * turnSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rocket.GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }
    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
            other.gameObject.layer == LayerMask.NameToLayer("Player Bullet"))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            GameObject explosion = Instantiate(_explosionObject, transform.position, Quaternion.Euler(Vector3.up));

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    gameManager.instance.PlayerScript().TakeDamage(explosionDamage);
                    break;
                }
            }

            Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration);
            GetComponentInChildren<MeshRenderer>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;
            enabled = false;
        }
    }
    */
}
