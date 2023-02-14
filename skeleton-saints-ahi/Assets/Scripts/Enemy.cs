using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private EnemyAI _enemyAi;
    [SerializeField] private GameObject firePosition;
    [SerializeField] private GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] float bulletSpeed;
    [SerializeField] float _materialFlashSpeed;
    [SerializeField] Material _material;
    [Range(0,10)] [SerializeField] private int _health;

    public bool isShooting = false;

    // Property to update _health field
    public int Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    void Start()
    {
        gameManager.instance.updateEnemyCounter();

        if (_material == null)
            _material = GetComponentInChildren<SkinnedMeshRenderer>().material;

        if(_enemyAi == null)
            _enemyAi = GetComponent<EnemyAI>();
    }

    void Update()
    {
        // If not shooting and can see the player
        if(isShooting == false && _enemyAi.CanDetectPlayer) 
            StartCoroutine(Shoot());
    }

    void OnDestroy()
    {
        Debug.Log($"{gameObject.name} has died");
        gameManager.instance.updateGameGoal(-1);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        StartCoroutine(FlashMaterial());

        if (_health <= 0)
            Destroy(gameObject);
    }

    public IEnumerator Shoot()
    {
        isShooting = true;
        GameObject bulletClone = Instantiate(bullet, firePosition.transform.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    private IEnumerator FlashMaterial()
    {
        // Made a new material so the original material isn't touched and possibly kept altered should it be interupted
        Material flashMaterial = Instantiate(_material);
        flashMaterial.color = Color.red;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = flashMaterial;
        yield return new WaitForSeconds(_materialFlashSpeed);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _material;

        Destroy(flashMaterial);
    }

}
