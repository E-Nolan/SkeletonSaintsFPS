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

    // This function will be called when the weapon is switched to from another one
    // Useful for updating UI elements
    void onSwitch();
}
