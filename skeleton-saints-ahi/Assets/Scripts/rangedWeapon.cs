using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangedWeapon : MonoBehaviour, IWeapon
{
    #region Member Fields
    [Header("----- Components -----")]
    [SerializeField] Transform weaponFirePos;
    [SerializeField] Transform targetFinder;
    [SerializeField] GameObject bullet;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip shotSound;

    [Header("----- Settings -----")]
    [SerializeField] bool usedByPlayer;
    [SerializeField] bool automaticFire;

    [Header("----- Stats -----")]
    [Range(5, 200)] [SerializeField] int bulletSpeed;
    [Range(0.0f, 5.0f)] [SerializeField] float fireRate; // in seconds
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

    // If this weapon is being used by an enemy, access 
    Enemy enemyScript;

    #endregion

    private void Start()
    {
        // If this weapon is being used an enemy, set a reference to its Enemy script to toggle its isShooting bool
        if (!usedByPlayer)
            enemyScript = transform.parent.GetComponent<Enemy>();
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
    /// Increases the weapon's ammo by the given amount.
    /// </summary>
    /// <param name="amount"></param>
    public void giveAmmo(int amount)
    {
        if (!infiniteAmmo)
            updateAmmo(amount);
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
        gameManager.instance.playerAmmoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    #endregion

    #region Shoot Functions
    /// <summary>
    /// Shoots a bullet using the gun's forward direction
    /// </summary>
    public void shootForward()
    {
        shoot(targetFinder.position - weaponFirePos.position);
    }

    /// <summary>
    /// Fires a round from the weapon in the direction of the given normalized vector
    /// </summary>
    /// <param name="fireDirection"></param>
    public void shoot(Vector3 fireDirection)
    {
        // Check to see whether or not the weapon has enough ammo to shoot
        // If it does, fire a bullet in the provided direction
        if (!isAmmoEmpty())
        {
            StartCoroutine(startShootCooldown());
            // For each shot in a burst, fire a bullet with a delay between each shot
            for (int i = 0; i  < bulletsPerBurst; i++)
            {
                StartCoroutine(shootBullet(fireDirection, i * burstFireDelay));
            }
        }
    }

    // Fire a bullet after a delay. A delay of 0 will fire immediately
    // Delays greater than 0 are used for guns with burst fire, and delay is equal to the time between each shot
    IEnumerator shootBullet(Vector3 fireDirection, float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);

        // Fire a bullet for each bullet in the spread.
        for (int i = 0; i < bulletsPerSpread; i++)
        {
            // Find the rotation that will be applied to the new bullet
            targetFinder.rotation.SetLookRotation(fireDirection, Vector3.up);
            if (spreadAngle > 0)
                getRandomSpreadTarget();

            // Instantiate a bullet at the Fire Position with the targetFinder's rotation and give it forward velocity
            // Reset the targetFinder's rotation for the next fired bullet
            GameObject newBullet = Instantiate(bullet, weaponFirePos.position, targetFinder.rotation);
            newBullet.GetComponent<bullet>().bulletDmg = damage;
            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
            targetFinder.rotation = weaponFirePos.rotation;
        }

        spendAmmo(1);
    }

     IEnumerator startShootCooldown()
     {
        if (usedByPlayer)
            gameManager.instance.playerScript.isShooting = true;
        else
            enemyScript.isShooting = true;

         yield return new WaitForSeconds(fireRate);

        if (usedByPlayer)
            gameManager.instance.playerScript.isShooting = false;
        else
            enemyScript.isShooting = false;
     }

    // Get a random angle within a cone from the gun. Used for spread shots.
    void getRandomSpreadTarget()
    {
        targetFinder.Rotate(Mathf.Pow(Random.Range(-1.0f, 1.0f), 3) * spreadAngle,
                            Mathf.Pow(Random.Range(-1.0f, 1.0f), 3) * spreadAngle,
                            Random.Range(0.0f, 360.0f),
                            Space.Self);
    }
    #endregion

    public void onSwitch()
    {
        if (usedByPlayer)
        {

        }
    }
}
