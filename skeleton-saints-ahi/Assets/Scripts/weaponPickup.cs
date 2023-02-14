using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] weaponStats weapon;
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
            gameManager.instance.playerScript.rangedWeaponPickup(weapon);
            if (pickupSound)
                gameManager.instance.playerScript.audioSource.PlayOneShot(pickupSound);

            Destroy(gameObject);
        }
    }
}
