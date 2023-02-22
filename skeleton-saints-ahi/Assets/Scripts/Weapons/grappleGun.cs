using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappleGun : rangedWeapon
{
    GameObject grappleHookPoint;
    hookPoint grappleHookScript;
    public LineRenderer lineRender;
    public int hookRange;

    public bool hookIsOut = false;

    private void Start()
    {
        targetFinder = new GameObject("Target Finder");
        targetFinder.transform.rotation = weaponFirePos.rotation;
        targetFinder.transform.position = weaponFirePos.position + weaponFirePos.forward;
        targetFinder.transform.parent = weaponFirePos;
    }

    private void Update()
    {
        if (hookIsOut)
        {
            lineRender.SetPosition(0, weaponFirePos.transform.position);
            lineRender.SetPosition(1, grappleHookPoint.transform.position);
        }
    }

    override public void shootForward()
    {
        shoot(targetFinder.transform.position - weaponFirePos.position);
    }

    override public void shoot(Vector3 fireDirection)
    {
        StartCoroutine(startShootCooldown());
        if (hookIsOut)
        {
            // If the grappling hook is out, bring it back to the fire pos
            grappleHookScript.beginRetracting();
        }
        else
        {
            // Otherwise, send the grappling hook out
            shootGrapplingHook(fireDirection);
        }
    }

    void shootGrapplingHook(Vector3 fireDirection)
    {
        targetFinder.transform.rotation.SetLookRotation(fireDirection, Vector3.up);
        grappleHookScript.fireHook(targetFinder.transform.rotation, bulletSpeed);
    }

    IEnumerator startShootCooldown()
    {
        gameManager.instance.playerScript.isShooting = true;
        yield return new WaitForSeconds(fireRate);
        gameManager.instance.playerScript.isShooting = false;
    }

    override public void copyFromWeaponStats(weaponStats _stats, Transform _weaponFirePos, bool _isUsedByPlayer)
    {
        weaponFirePos = _weaponFirePos;
        if (_stats.weaponModel)
            Instantiate(_stats.weaponModel, weaponFirePos);
        
        shotSound = _stats.shotSound;

        bulletSpeed = _stats.bulletSpeed;
        fireRate = _stats.fireRate;
        hookRange = _stats.range;

        grappleHookPoint = Instantiate(_stats.gunBullet, weaponFirePos.position, weaponFirePos.rotation, weaponFirePos);
        grappleHookPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        grappleHookScript = grappleHookPoint.GetComponent<hookPoint>();
        grappleHookScript.getGrappleGunScript(gameObject.GetComponent<grappleGun>());
        grappleHookScript.getWeaponFirePosition(weaponFirePos.gameObject);

        lineRender = grappleHookPoint.GetComponent<LineRenderer>();
        lineRender.positionCount = 2;

        activeImage = _stats.activeweaponIcon;
        inactiveImage = _stats.inactiveweaponIcon;
        weaponName = _stats.weaponName;
    }

    override public void onSwitch()
    {
        updateAmmoDisplay();
    }

    override public void offSwitch()
    {
        // Retract the hook back to the fire position
        grappleHookScript.instantRetract();
        hookIsOut = false;
    }

    void updateAmmoDisplay()
    {
        gameManager.instance.playerAmmoText.SetText("");
    }
}
