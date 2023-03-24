using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    IGate gateCheck;
    bool savedOnce = false;

    private void Start()
    {
        gateCheck = gameObject.GetComponent<IGate>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (gateCheck.isUnlocked)
        {
            if (other.CompareTag("Player"))
            {
                if (!savedOnce) {
                    savedOnce = true;
                    saveManagerInterface.instance.SaveMain();
                }
            }
        }
    }
    public void OnSerialize()
    {
        gateCheck = gameObject.GetComponent<IGate>();
    }
}
