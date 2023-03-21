using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class savePlayer : MonoBehaviour
{
    public static savePlayer instance;
    public int savedWeaponIndex = 0;

    private void Awake()
    {
        instance = this;
    }
    //This could maybe be updatePreferences
    public void updatePlayer()
    {
        savedWeaponIndex = gameManager.instance.PlayerScript().currWepIndex;

        playerPreferences.instance.MainWeapons.Clear();
        for (int i = 0; i < gameManager.instance.PlayerScript().weaponInventory.Count; i++)
        {
            if (gameManager.instance.PlayerScript().weaponInventory[i].GetComponent<rangedWeapon>().gunType == weaponStats.weaponStatsType.Gun)
            {
            GameObject _brandNewWeapon = Instantiate(gameManager.instance.PlayerScript().weaponInventory[i], gameManager.instance.transform);
            playerPreferences.instance.MainWeapons.Add(_brandNewWeapon);
            }
        }
    }
    //Would like this to be updatePlayer
    public void updatePlayerInstance()
    {
        for (int i = 0; i < playerPreferences.instance.MainWeapons.Count; i++)
        {
            GameObject _brandNewWeapon = Instantiate(playerPreferences.instance.MainWeapons[i], gameManager.instance.transform);
            gameManager.instance.PlayerScript().rangedWeaponPickup(_brandNewWeapon.GetComponent<weaponPickup>().weapon, _brandNewWeapon.GetComponent<weaponPickup>().weapon.weaponType);
        }
        if (playerPreferences.instance.OffWeapon)
        {
            gameManager.instance.PlayerScript().rangedWeaponPickup(weaponManager.instance.grappleGun, weaponStats.weaponStatsType.GrappleGun);
        }
    }
}
