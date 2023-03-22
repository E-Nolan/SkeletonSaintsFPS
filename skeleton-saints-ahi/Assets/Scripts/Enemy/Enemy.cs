using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private EnemyAI _enemyAi;
    [Range(0, 1000)] [SerializeField] private float _health;

    [Header("----- Audio -----")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip turretEnableScreech;
    [Range(0f, 1f)] [SerializeField] private float turretEnableScreechVolume;
    [SerializeField] private AudioClip spawnScreech;
    [Range(0f, 1f)] [SerializeField] private float spawnScreechVolume;
    [SerializeField] private AudioClip deathScreech;
    [Range(0f, 1f)] [SerializeField] private float deathScreechVolume;

    [Header("----- GameObjects/Transforms -----")]
    [SerializeField] private Transform gunPosition;
    [SerializeField] private Transform handTransform;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] Transform damagePopupPrefab;

    [Header("----- Material -----")]
    [SerializeField] float _materialFlashSpeed;
    [SerializeField] public Material _material;
    [SerializeField] public Material _flashMaterial;

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
    public string savedWeapon;
    public bool acquiringWeapon;
    public bool isDead = false;
    public bool isAttacking;
    public bool fadeHealthBar = false;
    public bool DummyEnemy;
    public bool canDetectPlayer;
    public bool canAttack;

    private bool shrinkAway = false;
    private float _maxHealth;
    private bool enableTurrets = false;
    private bool isBossEnemy;
    private bool isMutant;
    private int attackDamage;
    private bool turretsEnabled;
    private gameManager.Difficulty _difficulty;

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
        _enemyAi = GetComponent<EnemyAI>();

        if(SceneManager.GetActiveScene().name != "Main Menu") 
            _difficulty = gameManager.instance.currentDifficulty;

        switch (_difficulty)
        {
            case gameManager.Difficulty.Easy:
                _health *= _easyHealthMultiplier;
                break;
            case gameManager.Difficulty.Hard:
                _health *= _HardHealthMultiplier;
                break;
            case gameManager.Difficulty.Normal:
                break;
            default:
                break;
        }

        if (_enemyAi.BossEnemy || _enemyAi.IsMutant)
        {
            if (_enemyAi.BossEnemy)
            {
                isBossEnemy = true;
                audioSource = GetComponent<AudioSource>();
            }
            else if (_enemyAi.IsMutant)
                isMutant = true;

            attackDamage = _enemyAi.AttackDamage;
        }
        else
        {
            isBossEnemy = false;
            isMutant = false;
        }

        _maxHealth = _health;
        _healthBar.value = CalculateHealth();

        if (_material == null)
            _material = GetComponentInChildren<SkinnedMeshRenderer>().material;

        _enemyAi = GetComponent<EnemyAI>();

        if(_animator == null)
            _animator = GetComponent<Animator>();

        acquiringWeapon = false;

        if(isBossEnemy)
            _animator.SetTrigger("Spawned");
    }

    void Update()
    {
        if(isShooting == false && _enemyAi.CanDetectPlayer && _enemyAi.CanAttack &&
           !isDead && !gameManager.instance.getEnemyFiring() && 
           (!(isBossEnemy && GetComponent<bossAttackManager>().goingToWaveLocation) || 
            (isBossEnemy && !GetComponent<bossAttackManager>().goingToWaveLocation))) 
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

        if(!isBossEnemy && healthBarUI.activeSelf && gameManager.instance.playerInstance != null)
            healthBarUI.transform.LookAt(gameManager.instance.playerInstance.transform.position);

        if (!isBossEnemy && !isMutant && currentWeapon != null)
        {
            if(currentWeapon.CurrentClip <= 0)
                currentWeapon.startReload();
        }

        if (isBossEnemy && enableTurrets)
        {
            foreach (GameObject rotator in GameObject.FindGameObjectsWithTag("TurretRotator"))
            {
                Quaternion tempRotation = new Quaternion(0f, rotator.transform.localRotation.y, rotator.transform.localRotation.z, 1f);
                rotator.transform.localRotation = Quaternion.Lerp(rotator.transform.localRotation, tempRotation, Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (isBossEnemy || isMutant))
        {
            gameManager.instance.PlayerScript().TakeDamage(attackDamage);
            Vector3 direction = new Vector3(transform.position.x, 0,
    transform.position.z) - new Vector3(other.transform.position.x, 0, other.transform.position.z);
            Transform damageNumber = Instantiate(damagePopupPrefab,
                transform.position + transform.up * 2.0f, Quaternion.LookRotation(direction));
            DamagePopup damagePopup = damageNumber.GetComponent<DamagePopup>();
            damagePopup.Setup(attackDamage);
        }
    }

    private void OnDestroy()
    {
        if(isBossEnemy && Health <= 0 && SceneManager.GetActiveScene().name != "Main Menu")
            gameManager.instance.queuePlayerVictory(1f);
    }

    private float CalculateHealth()
    {
        return _health / _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(!DummyEnemy) 
            _health -= damage;

        if (!isBossEnemy || ((isBossEnemy || isMutant) && _maxHealth / 2 >  _health))
        {
            if (isBossEnemy)
            {
                if (!turretsEnabled)
                    StartCoroutine(EnableTurrets());
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

           // Polite AI Queue
            StartCoroutine(NextEnemyFireDelay(_nextEnemyFireDelay));

        }
        else
        {
            if (!isAttacking)
            {
                if (_enemyAi.GetAgentRemainingDistance() <= _enemyAi.GetAgentStoppingDistance() && 
                    (!(isBossEnemy && GetComponent<bossAttackManager>().goingToWaveLocation) ||
                     (isBossEnemy && !GetComponent<bossAttackManager>().goingToWaveLocation)))
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

    public void SetAttacking(bool p_isAttacking)
    {
        isAttacking = p_isAttacking;
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

        if (_difficulty == gameManager.Difficulty.Easy)
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
            ////Debug.Log($"drop selected is {dropSelected}");
            GameObject drop = Instantiate(enemyDrops[dropSelected], transform.position, transform.rotation);
        }
    }

    private void PlayTurretEnableScreech()
    {
        audioSource.PlayOneShot(turretEnableScreech, turretEnableScreechVolume);
    }

    private void PlayDeathScreech()
    {
        audioSource.PlayOneShot(deathScreech, deathScreechVolume);
    }

    private void PlaySpawnScreech()
    {
        if (spawnScreech != null)
            audioSource.PlayOneShot(spawnScreech, spawnScreechVolume);
    }

    public void OnDeserialize()
    {
        PickupWeapon(weaponManager.instance.GetEnemyWeaponStats(savedWeapon));
    }

    private IEnumerator EnableTurrets()
    {
        turretsEnabled = true;
        enableTurrets = true;
        _animator.SetTrigger("TurretEnable");

        yield return new WaitForSeconds(3f);

        foreach (Turret turret in GetComponentsInChildren<Turret>())
            turret.enabled = true;

        enableTurrets = false;
    }
}
