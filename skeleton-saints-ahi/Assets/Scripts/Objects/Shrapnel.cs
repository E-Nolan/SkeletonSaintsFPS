using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrapnel : MonoBehaviour
{
    [SerializeField] private GameObject _explosionParticles;

    public float ShrinkDelay;
    public float MinForce;
    public float MaxForce;
    public float Radius;

    private bool shrinkAway;

    void Start()
    {
        Explode();
        StartCoroutine(Delay(ShrinkDelay));
    }

    void Update()
    {
        if (shrinkAway)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale,
                    Vector3.zero, Time.deltaTime);

            if(transform.localScale.x <= 0.01f)
                Destroy(gameObject, 0.5f);
        }

    }

    public void Explode()
    {
        foreach (Transform t in transform)
        {
            Rigidbody rb = t.GetComponent<Rigidbody>();

            if(rb != null)
                rb.AddExplosionForce(Random.Range(MinForce, MaxForce), transform.position, Radius);
        }

        GameObject particles = Instantiate(_explosionParticles, transform.position, transform.rotation);
        //Destroy(particles, particles.GetComponent<ParticleSystem>().main.duration);
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shrinkAway = true;
    }
}
