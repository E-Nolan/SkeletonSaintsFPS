using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    IGate gateCheck;

    private void Awake()
    {
        gateCheck = GetComponent<IGate>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (gateCheck.isUnlocked)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Saving at checkpoint");
                saveManagerInterface.instance.SaveMain();
            }
        }
    }
}
