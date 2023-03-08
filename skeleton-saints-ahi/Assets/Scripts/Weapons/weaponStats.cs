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
    public Sprite bulletIcon;
    public AudioClip shotSound;

    [Range(5,200)] public int bulletSpeed;
    [Range(0.0f, 5.0f)] public float fireRate;
    [Range(0, 20)] public float damage;
    [Range(0.0f, 5.0f)] public float lastBulletBonus;
    [Range(1, 20)] public int bulletsPerSpread;
    [Range(0,60)] public int spreadAngle;
    [Range(1,10)] public int bulletsPerBurst;
    [Range(0.0f, 0.5f)] public float burstFireDelay;
    [Range(0.0f, 100.0f)] public float recoilForce;

    public bool infiniteAmmo;
    public bool doesNotUseAmmo;
    [Range(0,200)] public int startingAmmo;
    [Range(0, 200)] public int maxAmmo;
    [Range(0, 100)] public int currentClip;
    [Range(0, 100)] public int maxClipSize;
    [Range(0, 40)] public int ammoRecovery;
    [Range(0, 5)] public float reloadSpeed;
    [Tooltip("Currently only used for grapple gun range")]
    [Range(5, 1000)] public int range;
    public bool lobbedWeapon;

    [Header("Enemy Difficulty")]
    [Range(0,200)] public int bulletSpeedBonus;
    [Range(0.0f, 5.0f)] public float fireRateBonus;
    [Range(0, 20)] public float damageBonus;
    [Range(0, 20)] public int bulletsPerSpreadBonus;
    [Range(0, 60)]public int spreadAngleBonus;
}
