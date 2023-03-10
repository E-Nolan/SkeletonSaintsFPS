using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private EnemyAI _enemyAi;
    [Range(0, 1000)] [SerializeField] private float _health;

    [Header("----- GameObjects/Transforms -----")]
    [SerializeField] private Transform gunPosition;
    [SerializeField] private Transform handTransform;
    [SerializeField] private GameObject healthBarUI;

    [Header("----- Material -----")]
    [SerializeField] float _materialFlashSpeed;
    [SerializeField] Material _material;
    [SerializeField] private Material _flashMaterial;

    [Header("----- Difficulty Vars -----")]
    [Range(0, 1)] [SerializeField] private float _easyHealthMultiplier;
    [Range(1, 5)] [SerializeField] private float _HardHealthMultiplier;
    [Range(0, 1)] [SerializeField] private float _nextEnemyFireDelay;

    [Header("----- Enemy Drops -----")]
    [SerializeField] private List<GameObject> enemyDrops;
    [Range(0, 1)] [SerializeField] float dropChance;

    [Header("----- Publics -----")]
    public bool isShooting = false;
    public rangedWeapon currentWeapon;
    public bool acquiringWeapon;
    public bool isDead = false;
    public bool isAttacking;
    public bool fadeHealthBar = false;
    private bool shrinkAway = false;
    private float _maxHealth;
    private bool isBossEnemy;
    private bool isMutant;
    private int attackDamage;
    private gameManager.GameDifficulty _difficulty;

    [Header("----- Misc -----")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Animator _animator;

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
        if(isShooting == false && _enemyAi.CanDetectPlayer && _enemyAi.CanAttack &&
           !isDead && !gameManager.instance.getEnemyFiring()) 
            Attack();

        if(fadeHealthBar && !isBossEnemy)
            healthBarUI.GetComponent<CanvasGroup>().alpha =
                Mathf.MoveTowards(healthBarUI.GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 2f);

        if (shrinkAway)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime);
            if(transform.localScale.x <= 0f)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (isBossEnemy || isMutant))
        {
            gameManager.instance.PlayerScript().TakeDamage(attackDamage);
        }
    }

    private void OnDestroy()
    {
        if(isBossEnemy)
            gameManager.instance.queuePlayerVictory(1f);
    }

    private float CalculateHealth()
    {
        return _health / _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        // Only show damaged animation for every enemy normal enemy hit or
        // every quarter of Boss health
        if (!isBossEnemy || ((isBossEnemy || isMutant) && _maxHealth / 2 >  _health))
        {
            if (isBossEnemy)
            {
                foreach (Turret turret in GetComponentsInChildren<Turret>())
                    turret.enabled = true;
            }

            if (!isBossEnemy || isBossEnemy && _maxHealth / 3 > _health)
            {
                _animator.SetTrigger("Damage");

                if (isBossEnemy || isMutant)
                    _animator.SetFloat("HitRandom", UnityEngine.Random.Range(0f, 1f));
            }
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
            enemyDropPickup();

            // If the enemy was a boss, give the player a win after they're destroyed
            if (_enemyAi.BossEnemy)
            {
                foreach (Turret turret in GetComponentsInChildren<Turret>())
                    turret.enabled = false;
            }
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
           // currentWeapon.shoot(gameManager.instance.playerInstance.transform.position + 
           //                     new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-3f, 3f)));

            // Polite AI Queue
            StartCoroutine(NextEnemyFireDelay(_nextEnemyFireDelay));

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

    private void ShrinkAway()
    {
        shrinkAway = true;
    }

    private IEnumerator NextEnemyFireDelay(float delay)
    {
        gameManager.instance.setEnemyFiring(true);

        currentWeapon.shoot(gameManager.instance.playerInstance.transform.position + 
                            new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-3f, 3f)));

        if (_difficulty == gameManager.GameDifficulty.Easy)
            yield return new WaitForSeconds(delay);

        gameManager.instance.setEnemyFiring(false);
    }

    private IEnumerator FlashMaterial()
    {
        if (!isBossEnemy && !isMutant)
        {
            foreach (SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>())
                rend.material = _flashMaterial;
        }
        else
        {
            if (isBossEnemy)
            {
                foreach (SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (!GetComponent<Turret>())
                        rend.GetComponent<SkinnedMeshRenderer>().material = _flashMaterial;
                }
            }
            else
            {
                GetComponentInChildren<SkinnedMeshRenderer>().material = _flashMaterial;
            }
        }

        yield return new WaitForSeconds(_materialFlashSpeed);

        if (!isBossEnemy && !isMutant)
        {
            foreach (SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>())
                rend.material = _material;
        }
        else
        {
            if (isBossEnemy)
            {
                foreach (SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (!GetComponent<Turret>())
                        rend.GetComponent<SkinnedMeshRenderer>().material = _material;
                }
            }
            else
            {
                GetComponentInChildren<SkinnedMeshRenderer>().material = _material;
            }
        }
    }

    public void enemyDropPickup()
    {
        int dropSelected;
        if (UnityEngine.Random.value <= dropChance && enemyDrops != null)
        {
            dropSelected = UnityEngine.Random.Range(0, enemyDrops.Count);
            Debug.Log($"drop selected is {dropSelected}");
            GameObject drop = Instantiate(enemyDrops[dropSelected], transform.position, transform.rotation);
        }
    }
}
