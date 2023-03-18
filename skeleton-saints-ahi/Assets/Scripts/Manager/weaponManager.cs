using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponManager: MonoBehaviour
{
    public static weaponManager instance;

    public weaponStats pistol;
    public weaponStats SMG;
    public weaponStats shotgun;
    public weaponStats assaultRifle;
    public weaponStats grenadeLauncher;

    public weaponStats enemyPistol;
    public weaponStats enemySMG;
    public weaponStats enemyShotgun;
    public weaponStats enemyAssaultRifle;
    public weaponStats enemyGrenadeLauncher;
    public weaponStats enemyMissileLauncher;
    private void Awake()
    {
        instance = this;
    }
    public weaponStats GetPlayerWeaponStats(string weapon)
    {
        switch (weapon)
        {
            case "Pistol":
                return pistol;
            case "SMG":
                return SMG;
            case "Shotgun":
                return shotgun;
            case "Assault Rifle":
                return assaultRifle;
            case "Grenade Launcher":
                return grenadeLauncher;
            default:
                break;
        }
        Debug.Log("No suitable constructor found, returning pistol");
        return pistol;
    }
    public weaponStats GetEnemyWeaponStats(string weapon)
    {
        switch (weapon)
        {
            case "Enemy Pistol":
                return enemyPistol;
            case "Enemy SMG":
                return enemySMG;
            case "Enemy Shotgun":
                return enemyShotgun;
            case "Enemy Assault Rifle":
                return enemyAssaultRifle;
            case "Enemy Grenade Launcher":
                return enemyGrenadeLauncher;
            case "Enemy Missile Launcher":
                return enemyMissileLauncher;
            default:
                break;
        }
        Debug.Log("No " + weapon + " constructor found, returning pistol");
        return enemyPistol;
    }

}
