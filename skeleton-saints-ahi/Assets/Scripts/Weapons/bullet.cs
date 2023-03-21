using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Dmg for the bullet
    public float bulletDmg;

    [SerializeField] AudioSource bulletImpactSound;
    [SerializeField] private Transform damagePopupPrefab;

    private Transform damageNumber;
    private Camera mainCamera;

    public void setTimer(float _timer)
    {
        StartCoroutine(timerWait(_timer));
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Camera.main != null && mainCamera == null)
            mainCamera = Camera.main;

        if(damageNumber != null && mainCamera != null)
            damageNumber.rotation = Quaternion.LookRotation(damageNumber.position - mainCamera.transform.position, mainCamera.transform.up);
    }

    IEnumerator timerWait(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        stopBullet();
    }

    void stopBullet()
    {
        if(GetComponent<SphereCollider>())
            GetComponent<SphereCollider>().enabled = false;
        if(GetComponent<CapsuleCollider>())
            GetComponent<CapsuleCollider>().enabled = false;
        if (GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;


        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Use the IDamage interface to damage what the bullet collides with
        if (other.GetComponent<IDamage>() != null)
        {
            other.GetComponent<IDamage>().TakeDamage(bulletDmg);

            Vector3 direction = new Vector3(transform.position.x, 0, 
                transform.position.z) - new Vector3(other.transform.position.x, 0, other.transform.position.z);
            damageNumber = Instantiate(damagePopupPrefab, 
                GetComponent<Rigidbody>().transform.position, Quaternion.LookRotation(direction));

            if (other.CompareTag("Player"))
            {
                damageNumber.position -= transform.forward * 2.5f;
            }

            damageNumber.rotation = Quaternion.LookRotation(damageNumber.position - Camera.main.transform.position, Camera.main.transform.up);
            DamagePopup damagePopup = damageNumber.GetComponent<DamagePopup>();
            damagePopup.Setup(bulletDmg);
        } else if (other.GetComponent<Turret>())
        {
            Vector3 direction = new Vector3(transform.position.x, 0, 
                transform.position.z) - new Vector3(other.transform.position.x, 0, other.transform.position.z);

            damageNumber = Instantiate(damagePopupPrefab, 
                GetComponent<Rigidbody>().transform.position, Quaternion.LookRotation(direction));

            damageNumber.rotation = Quaternion.LookRotation(damageNumber.position - Camera.main.transform.position, Camera.main.transform.up);
            //damageNumber.GetComponent<TMPro.TextMeshPro>().enableAutoSizing = true;
            damageNumber.GetComponent<TMPro.TextMeshPro>().autoSizeTextContainer = true;
            damageNumber.GetComponent<TMPro.TextMeshPro>().fontSize = 2000f;
            DamagePopup damagePopup = damageNumber.GetComponent<DamagePopup>();
            damagePopup.Setup("IMMUNE");
        }

        if (bulletImpactSound)
            bulletImpactSound.Play();
        stopBullet();
    }


}
