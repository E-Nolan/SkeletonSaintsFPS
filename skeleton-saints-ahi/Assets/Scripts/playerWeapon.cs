using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerWeapon : MonoBehaviour
{
    [SerializeField] Transform weaponFirePos;
    [SerializeField] GameObject bullet;

    [Range(5, 200)] [SerializeField] int speed;
    [Range(0.0f, 5.0f)] [SerializeField] float fireRate; // in seconds
    [Range(1, 200)] [SerializeField] int range;
    [Range(0, 200)] [SerializeField] int maxAmmo;
    [Tooltip("Will be rounded down if it exceeds maxAmmo.\nSet to 200 to always be equal to maxAmmo")]
    [Range(0, 200)] [SerializeField] int currentAmmo;
    [Range(1, 20)] [SerializeField] int damage;

    [SerializeField] bool infiniteAmmo;

    /// <summary>
    /// returns true if the current weapon has no ammo
    /// </summary>
    /// <returns></returns>
    public bool isAmmoEmpty()
    {
        return currentAmmo == 0;
    }

    /// <summary>
    /// returns true if the current weapon's ammo is full or if the weapon has infinite ammo
    /// </summary>
    /// <returns></returns>
    public bool isAmmoFull()
    {
        return (currentAmmo == maxAmmo) || infiniteAmmo;
    }

    /// <summary>
    /// Decreases the weapon's ammo by the given amount. 
    /// </summary>
    /// <param name="amount"></param>
    public void spendAmmo(int amount)
    {
        updateAmmo(-amount);
    }

    /// <summary>
    /// Increases the weapon's ammo by the given amount.
    /// </summary>
    /// <param name="amount"></param>
    public void giveAmmo(int amount)
    {
        updateAmmo(amount);
    }

    // Add or subtract from the player's ammo count then update the Ammo Display on the UI
    void updateAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        // TODO: Update the gameManager's Ammo count

    }

    IEnumerator shoot()
    {
        gameManager.instance.playerScript.isShooting = true;
        yield return new WaitForSeconds(fireRate);
        gameManager.instance.playerScript.isShooting = false;
    }

}
