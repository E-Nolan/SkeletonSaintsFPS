using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] public weaponStats weapon;
    [SerializeField] AudioClip pickupSound;
    [Range(0,360)] [SerializeField] int rotationSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check to see if the player already has the weapon. If they do, give them ammo for the respective weapon instead
            GameObject duplicateWeapon = gameManager.instance.playerScript.weaponInventory.Find(x => x.GetComponent<rangedWeapon>().weaponName == weapon.weaponName);
            if (duplicateWeapon)
            {
                duplicateWeapon.GetComponent<rangedWeapon>().giveAmmo(2);
            }
            else
            {
                gameManager.instance.playerScript.rangedWeaponPickup(weapon, weapon.weaponType);
            }
            if (pickupSound)
                gameManager.instance.playerScript.audioSource.PlayOneShot(pickupSound);
            Destroy(gameObject);
        }
    }
}
