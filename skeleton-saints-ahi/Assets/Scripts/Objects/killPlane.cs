using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            gameManager.instance.loseGame();

        else if (other.GetComponent<IDamage>() != null)
            other.GetComponent<IDamage>().TakeDamage(10000);
    }
}
