using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] float fullExplosionScale;
    [Tooltip("In seconds")]
    [Range(0.01f, 1.0f)] [SerializeField] float explosionTime;
    [SerializeField] int explosionDamage;
    [Range(0, 100)] [SerializeField] int explosionForce;

    MeshRenderer meshRender;
    bool exploding = false;

    void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
        meshRender.enabled = false;
        // If explosionTime is somehow set to zero, increase it to prevent dividing by zero
        if (explosionTime == 0.0f)
            explosionTime = 0.01f;
    }

     //Update is called once per frame
    void Update()
    {
        if (exploding)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * fullExplosionScale, Time.deltaTime * (1.0f / explosionTime));
            if (transform.localScale.magnitude >= (Vector3.one * fullExplosionScale).magnitude - 0.05f)
                Destroy(gameObject, 0.1f);
        }
    }

    public void explode()
    {
        meshRender.enabled = true;
        exploding = true;
        //transform.localScale = Vector3.one * fullExplosionScale;
        Destroy(gameObject, explosionTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamage>() != null)
            other.GetComponent<IDamage>().TakeDamage(explosionDamage);
        if (other.CompareTag("Player"))
            gameManager.instance.PlayerScript().giveExternalVelocity((gameManager.instance.playerInstance.transform.position - transform.position).normalized * explosionForce);
    }
}

