using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class savePlayer : MonoBehaviour
{
    public static savePlayer instance;

    private void Awake()
    {
        instance = this;
    }

    public void updatePlayer()
  {
        Debug.Log("player saved");
        for(int i = 0;i < gameManager.instance.PlayerScript().weaponInventory.Count; i++)
        {
            Debug.Log($"{gameManager.instance.PlayerScript().weaponInventory[i]}");
        }
        Debug.Log($"{gameManager.instance.PlayerScript().offHandWeapon}");
        playerPreferences.instance.MainWeapons = gameManager.instance.PlayerScript().weaponInventory;
        playerPreferences.instance.OffWeapon = gameManager.instance.PlayerScript().offHandWeapon;
        for (int i = 0; i < playerPreferences.instance.MainWeapons.Count; i++)
        {
            Debug.Log($"{playerPreferences.instance.MainWeapons[i]}");
        }
        Debug.Log($"{playerPreferences.instance.OffWeapon}");
    }
}
