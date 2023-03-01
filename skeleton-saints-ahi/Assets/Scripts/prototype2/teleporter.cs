using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporter : MonoBehaviour
{
    [SerializeField] Transform teleporterExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            gameManager.instance.playerInstance.transform.position = teleporterExit.position;
            other.GetComponent<CharacterController>().enabled = true;
        }
    }
}
