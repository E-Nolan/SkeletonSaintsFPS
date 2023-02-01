using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] int _health;
    [SerializeField] float _materialFlashSpeed = 0f;
    [SerializeField] Material _material = null;

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
        StartCoroutine(FlashMaterial());

        if (_health <= 0)
        {
            Debug.Log($"{gameObject.name} has died");
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashMaterial()
    {
        Color originalColor = _material.color;
        _material.color = Color.red;
        yield return new WaitForSeconds(_materialFlashSpeed);
        _material.color = originalColor;
    }
}
