using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangedWeapon : MonoBehaviour
{
    #region Member Fields
    [Header("----- Components -----")]
    [SerializeField] GameObject gunModel;
    [SerializeField] public Transform weaponFirePos;
    [SerializeField] protected GameObject gunBullet;
    [SerializeField] AudioSource audioSource;
    [SerializeField] protected AudioClip shotSound;

    [Header("----- Settings -----")]
    [SerializeField] bool usedByPlayer;

    [Header("----- Stats -----")]
    [Range(5, 200)] [SerializeField] protected int bulletSpeed;
    [Range(0.0f, 5.0f)] [SerializeField] public float fireRate; // in seconds
    [Range(1, 20)] [SerializeField] int damage;
    [Range(1, 20)] [SerializeField] int bulletsPerSpread;
    [Range(0, 60)] [SerializeField] int spreadAngle;
    [Range(1, 10)] [SerializeField] int bulletsPerBurst;
    [Tooltip("For burst fire weapons, this is the time between each shot in a round")]
    [Range(0.0f, 0.5f)] [SerializeField] float burstFireDelay;

    [Header("----- Ammo -----")]
    [SerializeField] bool infiniteAmmo;
    [Tooltip("Will be rounded down if it exceeds maxAmmo.")]
    [Range(0, 200)] [SerializeField] int currentAmmo;
    [Range(0, 200)] [SerializeField] int maxAmmo;
    [Range(0, 40)] [SerializeField] int ammoRecovery;

    // If this weapon is being used by an enemy, access 
    Enemy enemyScript;
    protected GameObject targetFinder;

   
    public Sprite activeImage;
    public Sprite inactiveImage;
    public string weaponName;

    #endregion

    private void Start()
    {
        // Instantiate a new object in front of the Fire Position. This object will be used to find the angle with which to fire bullets
        targetFinder = new GameObject("Target Finder");
        targetFinder.transform.rotation = weaponFirePos.rotation;
        targetFinder.transform.position = weaponFirePos.position + weaponFirePos.forward;
        targetFinder.transform.parent = weaponFirePos;

        // If this weapon is being used an enemy, set a reference to its Enemy script to toggle its isShooting bool
        if (!usedByPlayer)
            enemyScript = transform.root.GetComponent<Enemy>();
        else
        {
            updateAmmoDisplay();
        }
        
    }

    #region Ammo Functions
    /// <summary>
    /// returns true if the current weapon has no ammo
    /// </summary>
    /// <returns></returns>
    public bool isAmmoEmpty()
    {
        return (currentAmmo == 0 && !infiniteAmmo);
    }

    /// <summary>
    /// returns true if the current weapon's ammo is full or if the weapon has infinite ammo
    /// </summary>
    /// <returns></returns>
    public bool isAmmoFull()
    {
        return ((currentAmmo == maxAmmo) || infiniteAmmo);
    }
    
    public bool isAmmoInfinite()
    {
        return infiniteAmmo;
    }
    
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    /// <summary>
    /// Decreases the weapon's ammo by the given amount. 
    /// </summary>
    /// <param name="amount"></param>
    public void spendAmmo(int amount)
    {
        if (!infiniteAmmo)
            updateAmmo(-amount);
    }

    /// <summary>
    /// Increases the weapon's ammo by an amount specific to the weapon, multiplied by the given amount
    /// </summary>
    /// <param name="amount"></param>
    public void giveAmmo(int amount = 1)
    {
        if (!infiniteAmmo)
            updateAmmo(amount * ammoRecovery);
    }

    // Add or subtract from the player's ammo count then update the Ammo Display on the UI
    void updateAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        if (usedByPlayer)
            updateAmmoDisplay();
    }

    void updateAmmoDisplay()
    {
        if (infiniteAmmo)
            hUDManager.instance.playerAmmoText.text = "";
        else
            hUDManager.instance.playerAmmoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    #endregion

    #region Shoot Functions
    /// <summary>
    /// Shoots a bullet using the gun's forward direction
    /// </summary>
    virtual public void shootForward()
    {
        shoot(targetFinder.transform.position - weaponFirePos.position);
    }

    /// <summary>
    /// Fires a round from the weapon at the position of the given target
    /// </summary>
    /// <param name="fireDirection"></param>
    virtual public void shoot(Vector3 fireTarget)
    {
        // Check to see whether or not the weapon has enough ammo to shoot
        // If it does, fire a bullet in the provided direction
        if (!isAmmoEmpty())
        {
            StartCoroutine(startShootCooldown());
            // For each shot in a burst, fire a bullet with a delay between each shot
            for (int i = 0; i  < bulletsPerBurst; i++)
            {
                StartCoroutine(shootBullet(fireTarget, i * burstFireDelay));
            }
        }
    }

    // Fire a bullet after a delay. A delay of 0 will fire immediately
    // Delays greater than 0 are used for guns with burst fire, and delay is equal to the time between each shot
    IEnumerator shootBullet(Vector3 _fireTarget, float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);

        if (shotSound)
            audioSource.PlayOneShot(shotSound);

        // Fire a bullet for each bullet in the spread.
        for (int i = 0; i < bulletsPerSpread; i++)
        {
            // Find the rotation that will be applied to the new bullet

            targetFinder.transform.rotation.SetLookRotation(_fireTarget, Vector3.up);
            if (spreadAngle > 0)
                getRandomSpreadTarget();

            // Instantiate a bullet at the Fire Position with the targetFinder's rotation and give it forward velocity
            // Reset the targetFinder's rotation for the next fired bullet
            GameObject newBullet = Instantiate(gunBullet, weaponFirePos.position, targetFinder.transform.rotation);
            newBullet.GetComponent<bullet>().bulletDmg = damage;
            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
            Debug.DrawRay(weaponFirePos.position, targetFinder.transform.forward * 10.0f, Color.blue, 1.0f);
            targetFinder.transform.rotation = weaponFirePos.rotation;
        }

        spendAmmo(1);
    }

     IEnumerator startShootCooldown()
     {
        if (usedByPlayer)
            gameManager.instance.PlayerScript().isPrimaryShooting = true;
        else
            enemyScript.isShooting = true;

         yield return new WaitForSeconds(fireRate);

        if (usedByPlayer)
            gameManager.instance.PlayerScript().isPrimaryShooting = false;
        else
            enemyScript.isShooting = false;
     }

    // Get a random angle within a cone from the gun. Used for spread shots.
    void getRandomSpreadTarget()
    {
        targetFinder.transform.Rotate(Mathf.Pow(Random.Range(-1.0f, 1.0f), 3) * spreadAngle,
                            Mathf.Pow(Random.Range(-1.0f, 1.0f), 3) * spreadAngle,
                            Random.Range(0.0f, 360.0f),
                            Space.Self);
    }
    #endregion

    virtual public void copyFromWeaponStats(weaponStats _stats, Transform _weaponFirePos, bool _isUsedByPlayer)
    {
        weaponFirePos = _weaponFirePos;
        if (_stats.weaponModel)
            Instantiate(_stats.weaponModel, weaponFirePos);

        usedByPlayer = _isUsedByPlayer;
        gunBullet = _stats.gunBullet;
        shotSound = _stats.shotSound;

        bulletSpeed = _stats.bulletSpeed + _stats.bulletSpeedBonus * (int)gameManager.instance.currentDifficulty;
        fireRate = _stats.fireRate - _stats.fireRateBonus * (int)gameManager.instance.currentDifficulty;
        damage = _stats.damage + _stats.damageBonus * (int)gameManager.instance.currentDifficulty;
        bulletsPerSpread = _stats.bulletsPerSpread + _stats.bulletsPerSpreadBonus * (int)gameManager.instance.currentDifficulty;
        spreadAngle = _stats.spreadAngle + _stats.spreadAngleBonus * (int)gameManager.instance.currentDifficulty;
        bulletsPerBurst = _stats.bulletsPerBurst;
        burstFireDelay = _stats.burstFireDelay;

        infiniteAmmo = _stats.infiniteAmmo;
        currentAmmo = _stats.startingAmmo;
        maxAmmo = _stats.maxAmmo;
        ammoRecovery = _stats.ammoRecovery;
        activeImage = _stats.activeweaponIcon;
        inactiveImage = _stats.activeweaponIcon;
        weaponName = _stats.weaponName;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        if (usedByPlayer)
            audioSource.volume = 0.5f;
    }

    virtual public void onSwitch()
    {
        if (gunModel)
            gunModel.SetActive(true);

        if (usedByPlayer)
        {
            gameManager.instance.PlayerScript().isPrimaryShooting = false;
            updateAmmoDisplay();
        }
    }

    virtual public void offSwitch()
    {
        if (gunModel)
            gunModel.SetActive(false);
    }
}
