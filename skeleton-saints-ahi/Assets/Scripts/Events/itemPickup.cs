using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : collectionItem
{
    [Tooltip("Positive = Clockwise | Negative = Counter-Clockwise")]
    [Range(-360, 360)][SerializeField] int rotationSpeed;
    [SerializeField] AudioClip pickupSound;

    // Update is called once per frame
    void Update()
    {
        //rotates the keycards 
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupSound)
                gameManager.instance.PlayerScript().audioSource.PlayOneShot(pickupSound);
            Collect();
            Destroy(gameObject);
        }
    }
}
