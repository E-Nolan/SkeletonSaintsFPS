using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mutantContainerBreak : MonoBehaviour
{
    [Header("These should be the same size")]
    [SerializeField] GameObject[] startingObjects;
    [SerializeField] GameObject[] brokenObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < startingObjects.Length; i++)
            {
                startingObjects[i].SetActive(false);
                brokenObjects[i].SetActive(true);
            }
        }
    }
}
