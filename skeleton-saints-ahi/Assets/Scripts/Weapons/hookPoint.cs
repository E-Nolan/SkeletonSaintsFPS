using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookPoint : MonoBehaviour
{
    Rigidbody rb;
    GameObject weaponFirePos;
    GameObject currentCatch;
    grappleGun grappleGunScript;
    BoxCollider hookCollider;
    bool retracting = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hookCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (retracting)
            gradualRetract();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!retracting)
        {
            // If the hook collides with a grapple point, freeze the hook in place and accelerate the player towards the hook
            if (collision.transform.CompareTag("GrapplePoint"))
            {

            }
            // If the hook collided with something else, retract it
            else
            {

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<collectiblePickup>())
        {
            // Set the pickup's parent to the hook, so that it will be moved with the hook, then retract the hook
            if (currentCatch == null)
            {
                rb.velocity = Vector3.zero;
                other.transform.parent = transform;
                currentCatch = other.gameObject;
                beginRetracting();
            }
        }
    }

    public void getWeaponFirePosition(GameObject _firePos)
    {
        weaponFirePos = _firePos;
    }

    public void getGrappleGunScript(grappleGun _grappleGunScript)
    {
        grappleGunScript = _grappleGunScript;
    }

    void beginRetracting()
    {
        retracting = true;
        hookCollider.enabled = false;
    }

    public void gradualRetract()
    {
        transform.position = Vector3.Lerp(transform.position, weaponFirePos.transform.position, Time.deltaTime * 20);
        if ((transform.position - weaponFirePos.transform.position).magnitude <= 0.4f)
        {
            instantRetract();
        }
    }

    public void instantRetract()
    {
        // Immediately pull the hook back to the weapon, and detach whatever pickup is attached to it.
        transform.SetPositionAndRotation(weaponFirePos.transform.position, weaponFirePos.transform.rotation);
        transform.parent = grappleGunScript.gameObject.transform;
        grappleGunScript.hookIsOut = false;
        retracting = false;
        grappleGunScript.lineRender.enabled = false;

        if (currentCatch)
        {
            currentCatch.transform.parent = null;
            currentCatch = null;
        }
    }

    public void fireHook(Quaternion _firePosRot, int _velocity)
    {
        // Unattach the hook from the gun and fire it in the chosen direction
        grappleGunScript.hookIsOut = true;
        transform.parent = null;
        transform.rotation = _firePosRot;
        rb.velocity = transform.forward * _velocity;
        hookCollider.enabled = true;
        grappleGunScript.lineRender.enabled = true;
    }
}
