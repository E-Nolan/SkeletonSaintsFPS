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
    bool pulling = false;
    float pullSpeed = 5.0f;
    public float retractSpeed;

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

        else if (pulling)
            gradualPull();

        else if (!gameManager.instance.PlayerScript().isGrappling && (transform.position - grappleGunScript.transform.position).magnitude >= grappleGunScript.hookRange)
            beginRetracting();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the hook collides with a grapple point, freeze the hook in place and accelerate the player towards the hook
        if (collision.transform.CompareTag("GrapplePoint") || collision.transform.CompareTag("Enemy") && currentCatch == null)
        {
            rb.isKinematic = true;
            beginPulling();
        }
        // If the hook collided with something else, retract it
        else
        {
            beginRetracting();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Collectible"))
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

    void beginPulling()
    {
        pulling = true;
        hookCollider.enabled = false;
        rb.velocity = Vector3.zero;
        gameManager.instance.PlayerScript().isGrappling = true;
    }

    void gradualPull()
    {
        Vector3 addedForce = (transform.position - grappleGunScript.transform.position) * Time.deltaTime * pullSpeed;
        gameManager.instance.PlayerScript().giveExternalVelocity(addedForce);
        if ((transform.position - gameManager.instance.playerInstance.transform.position).magnitude <= 2.5f)
            beginRetracting();
    }

    public void beginRetracting()
    {
        retracting = true;
        hookCollider.enabled = false;
        rb.velocity = Vector3.zero;
        gameManager.instance.PlayerScript().isGrappling = false;
    }

    void gradualRetract()
    {
        transform.position = Vector3.MoveTowards(transform.position, weaponFirePos.transform.position, retractSpeed * Time.deltaTime);
        if ((transform.position - weaponFirePos.transform.position).magnitude <= 0.5f)
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
        pulling = false;
        grappleGunScript.lineRender.enabled = false;
        rb.isKinematic = true;
        gameManager.instance.PlayerScript().isGrappling = false;

        // Release whatever object the hook is attached to
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

        rb.isKinematic = false;
        rb.velocity = transform.forward * _velocity;

        hookCollider.enabled = true;
        grappleGunScript.lineRender.enabled = true;
    }
}
