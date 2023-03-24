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

    public void updatePreferences()
    {
        savedWeaponIndex = gameManager.instance.PlayerScript().currWepIndex;

        playerPreferences.instance.MainWeapons.Clear();
        for (int i = 0; i < gameManager.instance.PlayerScript().weaponInventory.Count; i++)
        {
            if (gameManager.instance.PlayerScript().weaponInventory[i].GetComponent<rangedWeapon>().gunType == weaponStats.weaponStatsType.Gun)
            {
                GameObject _brandNewWeapon = Instantiate(gameManager.instance.PlayerScript().weaponInventory[i], gameManager.instance.transform);
                playerPreferences.instance.MainWeapons.Add(_brandNewWeapon);
            } else if (gameManager.instance.PlayerScript().weaponInventory[i].GetComponent<rangedWeapon>().gunType == weaponStats.weaponStatsType.SideArm)
            {
                GameObject _brandNewWeapon = Instantiate(gameManager.instance.PlayerScript().weaponInventory[i], gameManager.instance.transform);
                playerPreferences.instance.MainWeapons.Add(_brandNewWeapon);
            }
            else
            {
                Debug.Log("No Gun found for: " + gameManager.instance.PlayerScript().weaponInventory[i]);
            }
        }
        if (gameManager.instance.PlayerScript().offHandWeapon != null)
        {
            playerPreferences.instance.OffWeapon = true;
        }
    }

    public void updatePlayer()
    {
        for (int i = 0; i < playerPreferences.instance.MainWeapons.Count; i++)
        {
            GameObject _brandNewWeapon = Instantiate(playerPreferences.instance.MainWeapons[i], gameManager.instance.transform);
            weaponPickup weaponTemp = _brandNewWeapon.AddComponent<weaponPickup>();
            weaponTemp.weapon = weaponManager.instance.GetPlayerWeaponStats(_brandNewWeapon.GetComponent<rangedWeapon>().weaponName);
            gameManager.instance.PlayerScript().rangedWeaponPickup(_brandNewWeapon.GetComponent<weaponPickup>().weapon, _brandNewWeapon.GetComponent<weaponPickup>().weapon.weaponType);
        }
        if (playerPreferences.instance.OffWeapon)
        {
            gameManager.instance.PlayerScript().rangedWeaponPickup(weaponManager.instance.grappleGun, weaponStats.weaponStatsType.GrappleGun);
        }
    }
}
