using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class homingMissile : MonoBehaviour
{
    [Range(0.0f, 10.0f)] [SerializeField] float turnSpeed;
    [Range(0.0f, 10.0f)] [SerializeField] float accelerationRate;
    Vector3 toPlayerDir;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        toPlayerDir = (gameManager.instance.player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(toPlayerDir), Time.deltaTime * turnSpeed);
        rb.velocity = transform.forward * (rb.velocity.magnitude + accelerationRate * Time.deltaTime);
    }
}
