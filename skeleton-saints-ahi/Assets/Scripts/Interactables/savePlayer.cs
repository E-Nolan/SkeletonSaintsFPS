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

    public void updatePlayer()
  {
        savedWeaponIndex = gameManager.instance.PlayerScript().currWepIndex;

        playerPreferences.instance.MainWeapons.Clear();
        for (int i = 0; i < gameManager.instance.PlayerScript().weaponInventory.Count; i++)
        {
            GameObject _brandNewWeapon = Instantiate(gameManager.instance.PlayerScript().weaponInventory[i], gameManager.instance.transform);
            playerPreferences.instance.MainWeapons.Add(_brandNewWeapon);
        }
        if (gameManager.instance.PlayerScript().offHandWeapon)
        {
            playerPreferences.instance.OffWeapon = true;
        }
    }
}
