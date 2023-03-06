using System;
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
    [Range(0, 1)] [SerializeField] private float _easyHealthMultiplier;
    [Range(1, 5)] [SerializeField] private float _HardHealthMultiplier;
    [Range(0, 1000)] [SerializeField] private float _health;

    public bool isShooting = false;
    public rangedWeapon currentWeapon;
    public GameObject Bullet;
    public bool acquiringWeapon;
    public bool isDead = false;
    public bool isAttacking;
    public bool fadeHealthBar = false;
    private float _maxHealth;
    private bool isBossEnemy;
    private bool isMutant;
    private int attackDamage;
    private gameManager.GameDifficulty _difficulty;

    // Property to update _health field
    public float Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    void Start()
    {
        isAttacking = false;
        _difficulty = gameManager.instance.currentDifficulty;

        switch (_difficulty)
        {
            case gameManager.GameDifficulty.Easy:
                _health *= _easyHealthMultiplier;
                break;
            case gameManager.GameDifficulty.Hard:
                _health *= _HardHealthMultiplier;
                break;
            case gameManager.GameDifficulty.Normal:
                break;
            default:
                break;
        }

        if (_enemyAi.BossEnemy || _enemyAi.IsMutant)
        {
            if(_enemyAi.BossEnemy) 
                isBossEnemy = true;
            else if (_enemyAi.IsMutant)
                isMutant = true;
            attackDamage = _enemyAi.AttackDamage;
        }
        else
        {
            isBossEnemy = false;
            isMutant = false;
        }

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
        if(isShooting == false && _enemyAi.CanDetectPlayer && _enemyAi.CanAttack && !isDead) 
            Attack();

        if(fadeHealthBar && !isBossEnemy)
            healthBarUI.GetComponent<CanvasGroup>().alpha =
                Mathf.MoveTowards(healthBarUI.GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (isBossEnemy || isMutant))
        {
            gameManager.instance.PlayerScript().TakeDamage(attackDamage);
        }
    }

    private float CalculateHealth()
    {
        return _health / _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        // Only show damaged animation for every enemy normal enemy hit or
        // every quarter of Boss health
        if (!isBossEnemy || ((isBossEnemy || isMutant) && Math.Abs(_maxHealth % _health - _maxHealth / 4) < 0.001f))
        {
            _animator.SetTrigger("Damage");

            if(isBossEnemy || isMutant)
                _animator.SetFloat("HitRandom", UnityEngine.Random.Range(0f, 1f));
        }

        StartCoroutine(FlashMaterial());
        _healthBar.value = CalculateHealth();

        if (_health > _maxHealth)
            _health = _maxHealth;
        
        if(_health < _maxHealth && !isBossEnemy)
            healthBarUI.SetActive(true);

        if(Math.Abs(_health - _maxHealth) < 0.01f && !isBossEnemy)
            healthBarUI.SetActive(false);

        if (_health <= 0)
        {
            isDead = true;
            _animator.SetBool("Dead", true);
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            gameManager.instance.updateGameGoal(-1);
            fadeHealthBar = true;

            // If the enemy was a boss, give the player a win after they're destroyed
            if (_enemyAi.BossEnemy)
            {
                gameManager.instance.queuePlayerVictory(5.1f);
            }
            Destroy(gameObject, 5f);

        }

        if(_enemyAi.GetAgent().isActiveAndEnabled)
            _enemyAi.SetAgentDestination(gameManager.instance.playerInstance.transform.position);
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

    public void Attack()
    {
        if (currentWeapon == null && !isBossEnemy && !isMutant)
            return;

        if (!isBossEnemy && !isMutant)
        {
            _animator.SetFloat("fireSpeed", currentWeapon.fireRate * 250f); // Sets the multiplier for firing animation
            _animator.SetTrigger("Shoot");
            Vector3 distanceToPlayer =
                (gameManager.instance.playerInstance.transform.position - gunPosition.transform.position);
            currentWeapon.shoot(gameManager.instance.playerInstance.transform.position + 
                                new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-3f, 3f)));
        }
        else
        {
            if (!isAttacking)
            {
                if (_enemyAi.GetAgentRemainingDistance() <= _enemyAi.GetAgentStoppingDistance())
                {
                    _animator.SetTrigger("Attacking");
                    _animator.SetFloat("AttackRandom", UnityEngine.Random.Range(-1, 2));
                    _enemyAi.IncreaseAgentSpeed();
                }
            }
            else
            {
                _enemyAi.DecreaseAgentSpeed();
            }
        }
    }

    private void EnableAttackCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    private void DisableAttackCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    private void Attacking()
    {
        isAttacking = true;
    }

    private void NotAttacking()
    {
        isAttacking = false;
    }

    private IEnumerator FlashMaterial()
    {
        // Made a new material so the original material isn't touched and possibly kept altered should it be interupted
        Material flashMaterial = Instantiate(_material);
        flashMaterial.color = !isMutant ? Color.white : Color.red;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = flashMaterial;
        yield return new WaitForSeconds(_materialFlashSpeed);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _material;

        Destroy(flashMaterial);
    }
}
