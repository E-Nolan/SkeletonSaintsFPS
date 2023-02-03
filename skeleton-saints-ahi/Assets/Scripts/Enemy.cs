using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private GameObject firePosition;
    [SerializeField] private GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] float bulletSpeed;
    [SerializeField] float _materialFlashSpeed;
    [SerializeField] Material _material;
    [Range(0,10)] [SerializeField] private int _health;

    private EnemyAI _enemyAI;

    public bool isShooting = false;
    public bool isPlayerInRange = false;

    // Property to update _health field
    public int Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    void Start()
    {
        if (_material == null)
            _material = GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    void Update()
    {
        if(isPlayerInRange && (isShooting == false)) 
            StartCoroutine(Shoot());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
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

    private IEnumerator Shoot()
    {
        isShooting = true;
        GameObject bulletClone = Instantiate(bullet, firePosition.transform.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    private IEnumerator FlashMaterial()
    {
        // Done this way so original material isn't touched and possibly kept altered
        Material flashMaterial = Instantiate(_material);
        flashMaterial.color = Color.red;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = flashMaterial;
        yield return new WaitForSeconds(_materialFlashSpeed);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _material;

        Destroy(flashMaterial);
    }

}
