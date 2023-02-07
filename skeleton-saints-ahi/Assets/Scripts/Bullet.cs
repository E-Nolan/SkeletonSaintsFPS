using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(0.1f,10f)] [SerializeField] public float timer;

    private playerController playerController;

    void Start()
    {
        Destroy(gameObject, timer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
            
        }
    }
}
