using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageZone : MonoBehaviour
{
    [SerializeField] float damagePerSecond;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.PlayerScript().SetInvincibilityState(false);
            gameManager.instance.PlayerScript().TakeDamage(damagePerSecond * Time.fixedDeltaTime);
        }
    }
}
