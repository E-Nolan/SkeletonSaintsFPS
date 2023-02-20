using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private EnemyAI _enemyAi;
    [SerializeField] private Transform gunPosition;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Transform handTransform;
    [SerializeField] float _materialFlashSpeed;
    [SerializeField] Material _material;
    [Range(0,10)] [SerializeField] private float _health;

    public bool isShooting = false;
    public rangedWeapon currentWeapon;
    public bool acquiringWeapon;
    public bool isDead = false;
    public bool isDamaged = false;
    public bool fadeHealthBar = false;

    private float _maxHealth;

    // Property to update _health field
    public float Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    void Start()
    {
        gameManager.instance.updateEnemyCounter();

        _maxHealth = _health;
        _healthBar.value = CalculateHealth();

        if (_material == null)
            _material = GetComponentInChildren<SkinnedMeshRenderer>().material;

        if(_enemyAi == null)
            _enemyAi = GetComponent<EnemyAI>();

        if(_animator == null)
            _animator = GetComponent<Animator>();

        acquiringWeapon = false;
    }

    void Update()
    {
        // If not shooting and can see the player
        if(isShooting == false && _enemyAi.CanDetectPlayer && _enemyAi.CanShoot && !isDead) 
            Shoot();

        if(fadeHealthBar)
            healthBarUI.GetComponent<CanvasGroup>().alpha =
                Mathf.MoveTowards(healthBarUI.GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 2f);
    }

    private float CalculateHealth()
    {
        return _health / _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        _animator.SetTrigger("Damage");
        StartCoroutine(FlashMaterial());
        _healthBar.value = CalculateHealth();

        if (_health > _maxHealth)
            _health = _maxHealth;
        
        if(_health < _maxHealth)
            healthBarUI.SetActive(true);

        if(_health == _maxHealth)
            healthBarUI.SetActive(false);

        if (_health <= 0)
        {
            isDead = true;
            _animator.SetBool("Dead", true);
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            Debug.Log($"{gameObject.name} has died");
            gameManager.instance.updateGameGoal(-1);
            fadeHealthBar = true;
            Destroy(gameObject, 5f);
        }
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

        _animator.SetFloat("fireSpeed", currentWeapon.fireRate * 250f); // Sets the multiplier for firing animation
        _animator.SetTrigger("Shoot");
        currentWeapon.shoot(directionToTarget.normalized);
    }

    private IEnumerator FlashMaterial()
    {
        // Made a new material so the original material isn't touched and possibly kept altered should it be interupted
        Material flashMaterial = Instantiate(_material);
        flashMaterial.color = Color.white;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = flashMaterial;
        yield return new WaitForSeconds(_materialFlashSpeed);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _material;

        Destroy(flashMaterial);
    }
}
