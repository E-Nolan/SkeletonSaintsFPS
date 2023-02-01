using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int _health;

    void Start()
    {

    }

    void Update()
    {
        // debug
        if (Input.GetButtonDown("Fire1"))
            TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Debug.Log($"{gameObject.name} has died");
            Destroy(gameObject);
        }
    }
}
