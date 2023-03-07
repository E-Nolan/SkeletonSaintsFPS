using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectiblePickup : MonoBehaviour
{
    [Range(0,100)] [SerializeField] int ammoRecoveryMultiplier;
    [Range(0,100)] [SerializeField] float healthRecovery;
    [Tooltip("Positive = Clockwise | Negative = Counter-Clockwise")]
    [Range(-360, 360)] [SerializeField] int rotationSpeed;
    [SerializeField] AudioClip pickupSound;

    private void Update()
    {
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // The pickup can only be collected if the respective resource isn't already full
            if ((ammoRecoveryMultiplier > 0 && !gameManager.instance.PlayerScript().currentWeapon.isAmmoFull()) || (healthRecovery > 0 && !gameManager.instance.PlayerScript().isHealthFull()))
            {
                gameManager.instance.PlayerScript().currentWeapon.giveAmmo(ammoRecoveryMultiplier);
                gameManager.instance.PlayerScript().GiveHealth(healthRecovery);

                gameManager.instance.PlayerScript().audioSource.PlayOneShot(pickupSound);

                Destroy(gameObject);
            }
        }
    }
}
