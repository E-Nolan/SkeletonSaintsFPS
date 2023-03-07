using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energyWave : MonoBehaviour
{

    [Range(1, 10)] [SerializeField] int waveDamage;
    [Range(0, 10)] [SerializeField] int waveDamageDifficultyBonus;
    [Tooltip("How long the wave waits before expanding")]
    [Range(0.0f, 2.0f)] [SerializeField] float telegraphDelay;

    [Header("----- Expansion Rate -----")]
    [SerializeField] bool flatExpansion;
    [Range(0.0f, 10.0f)] [SerializeField] float expansionSpeed;
    [SerializeField] bool acceleratedExpansion;
    [Range(0.0f, 10.0f)] [SerializeField] float expansionAcceleration;
    [Range(10.0f, 1000.0f)] [SerializeField] float maxScale;

    [Header("----- Components -----")]
    [SerializeField] Collider[] colliderGroup;
    [SerializeField] ParticleSystem[] particles;

    [Header("----- Script Control -----")]
    [SerializeField] bool isExpanding = false;
    [SerializeField] Vector3 currentScale;
    [SerializeField] Vector3 startingScale;
    [SerializeField] float startingParticleRadius;
    [SerializeField] float startingEmissionRate;
    void Start()
    {
        startingScale = transform.localScale;
        startWaveAttack(transform.position);
        if (particles.Length > 0)
        {
            startingParticleRadius = particles[0].shape.radius;
            startingEmissionRate = particles[0].emission.rateOverTime.constant;
        }

    }

    void Update()
    {
        // if the ring is expanding, increase its horizontal scale over time
        if (isExpanding)
        {
            // Flat Expansion increases the scale linearly over time
            if (flatExpansion)
            {
                currentScale.Set(transform.localScale.x + expansionSpeed * Time.deltaTime, 1.0f, transform.localScale.z + expansionSpeed * Time.deltaTime);
                adjustScale();
            }
            // Accelerated Expansion increases the scale exponentially over time
            if (acceleratedExpansion)
            {
                currentScale.Set(transform.localScale.x + transform.localScale.x * expansionAcceleration * Time.deltaTime, 1.0f, transform.localScale.z + transform.localScale.z * expansionAcceleration * Time.deltaTime);
                adjustScale();
            }
            // Once the ring reaches a certain size, disable it until it is needed again
            if (currentScale.x >= maxScale)
            {
                disableRing();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IDamage>().TakeDamage(waveDamage);
        }
    }

    IEnumerator startExpanding()
    {
        yield return new WaitForSeconds(telegraphDelay);
        isExpanding = true;
    }

    public void startWaveAttack(Vector3 _bossPos)
    {
        disableRing();
        transform.localScale = startingScale;
        transform.position = _bossPos;
        isExpanding = false;
        enableRing();
        StartCoroutine(startExpanding());
    }

    void disableRing()
    {
        isExpanding = false;
        for (int i = 0; i < colliderGroup.Length; i++)
        {
            colliderGroup[i].enabled = false;
        }
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop();
        }
    }

    void enableRing()
    {
        for (int i = 0; i < colliderGroup.Length; i++)
        {
            colliderGroup[i].enabled = true;
        }
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
        }
    }

    void adjustScale()
    {
        transform.localScale = currentScale;
        for (int i = 0; i < particles.Length; i++)
        {
            var _shape = particles[i].shape;
            _shape.radius = startingParticleRadius * currentScale.x;


            var _emission = particles[i].emission;
            _emission.rateOverTime = currentScale.x * startingEmissionRate;
        }
    }
}
