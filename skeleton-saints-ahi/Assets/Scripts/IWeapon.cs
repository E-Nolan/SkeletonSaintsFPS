using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void shootForward();
    void shoot(Vector3 fireDirection);

    bool isAmmoFull();
    bool isAmmoInfinite();
    int GetCurrentAmmo();
    int GetMaxAmmo();
    void giveAmmo(int amount);

    void copyFromWeaponStats(weaponStats _stats, Transform _weaponFirePos, bool _isUsedByPlayer);

    void onSwitch();
    void offSwitch();
}
