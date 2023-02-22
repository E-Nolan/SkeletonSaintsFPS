using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Dmg for the bullet
    public int bulletDmg;

    // Timer for how long the before the bullet is destroyed if no hit occurs
    [SerializeField] int timer;

    [SerializeField] AudioSource bulletImpactSound;
    [SerializeField] private Transform damagePopupPrefab;

   
    void Start()
    {
        // Destroys the bullet after X amount of time
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Use the IDamage interface to damage what the bullet collides with
        if (other.GetComponent<IDamage>() != null)
        {
            other.GetComponent<IDamage>().TakeDamage(bulletDmg);
            Vector3 direction = new Vector3(other.transform.position.x, 0, 
                other.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
            Transform damageNumber = Instantiate(damagePopupPrefab, 
                GetComponent<Rigidbody>().transform.position, Quaternion.LookRotation(direction));
            DamagePopup damagePopup = damageNumber.GetComponent<DamagePopup>();
            damagePopup.Setup(bulletDmg);
        }

        if (bulletImpactSound)
            bulletImpactSound.Play();
        GetComponent<SphereCollider>().enabled = false;
        if (GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;


        Destroy(gameObject, 5.0f);
    }


}
