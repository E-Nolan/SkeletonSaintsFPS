using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private EnemyAI _enemyAi;
    [SerializeField] private Transform gunPosition;
    [SerializeField] private Transform handTransform;
    [SerializeField] float _materialFlashSpeed;
    [SerializeField] Material _material;
    [Range(0,10)] [SerializeField] private int _health;

    public bool isShooting = false;
    public rangedWeapon currentWeapon;
    public bool acquiringWeapon;
    public bool hopefullyChildrenInstantiated;

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

        acquiringWeapon = false;
        hopefullyChildrenInstantiated = false;
    }

    void Update()
    {
        // If not shooting and can see the player
        if(isShooting == false && _enemyAi.CanDetectPlayer && _enemyAi.CanShoot) 
            Shoot();
    }

    void OnDestroy()
    {
        // Health check so game goal doesn't go wonky
        if (_health <= 0)
        {
            Debug.Log($"{gameObject.name} has died");
            gameManager.instance.updateGameGoal(-1);
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        StartCoroutine(FlashMaterial());

        if (_health <= 0)
            Destroy(gameObject);
    }

    public void PickupWeapon(weaponStats newWeaponStats)
    {
        if (currentWeapon == null && !acquiringWeapon)
        {
            acquiringWeapon = true;
            GameObject newWeapon = new GameObject(newWeaponStats.name, typeof(rangedWeapon), typeof(AudioSource));
            newWeapon.transform.SetParent(gunPosition);
            newWeapon.transform.position = gunPosition.position;
            newWeapon.transform.rotation = gunPosition.rotation;

            GameObject weaponObject = Instantiate(newWeapon, newWeapon.transform.position, 
                newWeapon.transform.rotation, newWeapon.transform.parent);

            weaponObject.GetComponent<rangedWeapon>().copyFromWeaponStats(newWeaponStats,
                weaponObject.transform, false);
            //weaponObject.GetComponent<rangedWeapon>().copyFromWeaponStats(newWeaponStats,
            //        weaponObject.transform.GetChild(0).GetChild(0).transform, false);

            currentWeapon = weaponObject.GetComponent<rangedWeapon>();

            currentWeapon.weaponFirePos = weaponObject.transform.GetChild(0).GetChild(0).transform;

            // this took 4 hours to realize
            Destroy(newWeapon);
        }
    }

    public void Shoot()
    {
        if (currentWeapon == null)
            return;

        Vector3 directionToTarget = (gameManager.instance.player.transform.position - gunPosition.transform.position);
        currentWeapon.shoot(directionToTarget.normalized);
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
