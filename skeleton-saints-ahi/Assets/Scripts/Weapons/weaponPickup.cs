using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : IInteractable
{
    [SerializeField] public weaponStats weapon;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] MeshFilter gunPickupMesh;
    [SerializeField] MeshRenderer gunPickupMaterial;
    [Range(0,360)] [SerializeField] int rotationSpeed;
    public bool canInteract = true;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public override void Interact()
    {
        if (canInteract)
        {
            StartCoroutine(interactCooldown());
            if (gameManager.instance.PlayerScript().weaponInventory.Count >= 3)
            {
                // If the player's inventory is full, swap their weapon with the weapon inside this pickup
                GameObject _givenWeapon = gameManager.instance.PlayerScript().rangedWeaponSwap(weapon, weapon.weaponType);
                replaceWeaponPickup(_givenWeapon.GetComponent<rangedWeapon>());
                Destroy(_givenWeapon);
            }
            else
            {
                // Add the weapon to the player's inventory if there's enough room for it and destroy the pickup
                gameManager.instance.PlayerScript().rangedWeaponPickup(weapon, weapon.weaponType);
                if (pickupSound)
                    gameManager.instance.PlayerScript().audioSource.PlayOneShot(pickupSound);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator noSwappingPistolDelay()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator interactCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(0.25f);
        canInteract = true;
    }

    void replaceWeaponPickup(rangedWeapon _newWeapon)
    {
        weapon = _newWeapon.originalWeapon;
        gunPickupMesh.sharedMesh = _newWeapon.meshFilter.sharedMesh;
        gunPickupMaterial.sharedMaterial = _newWeapon.meshRenderer.sharedMaterial;
    }

    /*
     OLD FUNCTIONALITY
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if ((weapon.weaponType == weaponStats.weaponStatsType.Gun && gameManager.instance.PlayerScript().weaponInventory.Count < 3) 
             || (weapon.weaponType == weaponStats.weaponStatsType.GrappleGun && gameManager.instance.PlayerScript().currentSecondary == null))
            {
                // Check to see if the player already has the weapon. If they do, give them ammo for the respective weapon instead
                GameObject duplicateWeapon = gameManager.instance.PlayerScript().weaponInventory.Find(x => x.GetComponent<rangedWeapon>().weaponName == weapon.weaponName);
                if (duplicateWeapon)
                {
                    duplicateWeapon.GetComponent<rangedWeapon>().giveAmmo(2);
                }
                else
                {
                    gameManager.instance.PlayerScript().rangedWeaponPickup(weapon, weapon.weaponType);
                }
                if (pickupSound)
                    gameManager.instance.PlayerScript().audioSource.PlayOneShot(pickupSound);
                Destroy(gameObject);
            }
            else
            {
                hUDManager.instance.displayWeaponPickUpTrue();
            }
        }
    }
    */
}