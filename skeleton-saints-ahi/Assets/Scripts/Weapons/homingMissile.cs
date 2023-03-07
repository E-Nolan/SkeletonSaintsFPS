using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class homingMissile : MonoBehaviour
{
    [SerializeField] private GameObject _explosionObject;
    [Range(0.0f, 10.0f)] [SerializeField] float turnSpeed;
    [Range(0.0f, 10.0f)] [SerializeField] float accelerationRate;
    [SerializeField] private float timer;
    Vector3 toPlayerDir;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timer);
    }

    // Update is called once per frame
    void Update()
    {
        toPlayerDir = (gameManager.instance.playerInstance.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.LookRotation(toPlayerDir), Time.deltaTime * turnSpeed);
        rb.velocity = transform.forward * (rb.velocity.magnitude + accelerationRate * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            GameObject explosion = Instantiate(_explosionObject, transform.position, Quaternion.Euler(Vector3.up));
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
            GetComponentInChildren<MeshRenderer>().enabled = false;
            //Destroy(gameObject);
        }
    }
}
