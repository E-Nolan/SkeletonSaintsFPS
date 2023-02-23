using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class weaponStats : ScriptableObject
{
    public enum weaponStatsType
    {
        Gun,
        GrappleGun
    }

    public string weaponName;
    public weaponStatsType weaponType;

    public GameObject weaponModel;
    public GameObject gunBullet;
    public Sprite activeweaponIcon;
    public Sprite inactiveweaponIcon;
    public AudioClip shotSound;

    [Range(5,200)] public int bulletSpeed;
    [Range(0.0f, 5.0f)] public float fireRate;
    [Range(0, 20)] public int damage;
    [Range(1, 20)] public int bulletsPerSpread;
    [Range(0,60)] public int spreadAngle;
    [Range(1,10)] public int bulletsPerBurst;
    [Range(0.0f, 0.5f)] public float burstFireDelay;

    public bool infiniteAmmo;
    [Range(0,200)] public int startingAmmo;
    [Range(0, 200)] public int maxAmmo;
    [Range(0, 40)] public int ammoRecovery;
    [Tooltip("Currently only used for grapple gun range")]
    [Range(5, 100)] public int range;
    public bool lobbedWeapon;

    [Header("Enemy Difficulty")]
    public int bulletSpeedBonus;
    public float fireRateBonus;
    public int damageBonus;
    public int bulletsPerSpreadBonus;
    public int spreadAngleBonus;
}
