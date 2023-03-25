using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class collectiblePickup : MonoBehaviour
{
    [Range(0,100)] [SerializeField] int ammoRecoveryMultiplier;
    [Range(0,100)] [SerializeField] float healthRecovery;
    [Tooltip("Positive = Clockwise | Negative = Counter-Clockwise")]
    [Range(-360, 360)] [SerializeField] int rotationSpeed;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] private GameObject pistolText;

    private GameObject tempText;

    private void Update()
    {
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime);

        if (tempText != null)
            tempText.transform.rotation =
                Quaternion.LookRotation(transform.position - gameManager.instance.playerInstance.transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangedWeapon currentWeapon = gameManager.instance.PlayerScript().currentWeapon;

            if ((currentWeapon.weaponName == "Pistol" || currentWeapon.isAmmoFull()) && tempText == null && ammoRecoveryMultiplier < 0)
            {
                tempText = Instantiate(pistolText, transform.position + new Vector3(0f, transform.position.y / 2, 0f), transform.rotation, transform.root);
                tempText.GetComponent<TextMeshPro>().SetText($"{currentWeapon.name} already at Max Ammo");
                Destroy(tempText, 5f);
            }

            // The pickup can only be collected if the respective resource isn't already full
            if ((ammoRecoveryMultiplier > 0 && !currentWeapon.isAmmoFull()) || (healthRecovery > 0 && !gameManager.instance.PlayerScript().isHealthFull()))
            {
                currentWeapon.giveAmmo(ammoRecoveryMultiplier);
                gameManager.instance.PlayerScript().GiveHealth(healthRecovery);

                gameManager.instance.PlayerScript().audioSource.PlayOneShot(pickupSound);

                Destroy(GetComponentInParent<Transform>().gameObject);
            }
        }
    }
}
