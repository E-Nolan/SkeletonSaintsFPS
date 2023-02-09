using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectiblePickup : MonoBehaviour
{
    [Range(0,100)] [SerializeField] int ammoRecovery;
    [Range(0,100)] [SerializeField] int healthRecovery;
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
            if ((ammoRecovery > 0 && !gameManager.instance.playerScript.isAmmoFull()) || (healthRecovery > 0 && !gameManager.instance.playerScript.isHealthFull()))
            {
                gameManager.instance.playerScript.giveAmmo(ammoRecovery);
                gameManager.instance.playerScript.GiveHealth(healthRecovery);

                gameManager.instance.playerScript.audioSource.PlayOneShot(pickupSound);

                Destroy(gameObject);
            }
        }
    }
}
