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
        // Done this way so original material isn't touched and possibly kept altered
        Material flashMaterial = Instantiate(_material);
        flashMaterial.color = Color.red;
        gameObject.GetComponent<Renderer>().material = flashMaterial;
        yield return new WaitForSeconds(_materialFlashSpeed);
        gameObject.GetComponent<Renderer>().material = _material;
        Destroy(flashMaterial);
    }
}
