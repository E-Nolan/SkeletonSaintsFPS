using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cratePhysics : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    // Start is called before the first frame update
    void FixedUpdate()
    {
        if (rb.IsSleeping())
            rb.WakeUp();
    }
}
