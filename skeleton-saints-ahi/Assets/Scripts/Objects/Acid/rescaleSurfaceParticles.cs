using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rescaleSurfaceParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent)
        {
            ParticleSystem particles = GetComponent<ParticleSystem>();
            float scaleX = transform.parent.transform.localScale.x;
            float scaleY = transform.parent.transform.localScale.z;
            ParticleSystem.ShapeModule particleShape = particles.shape;
            ParticleSystem.EmissionModule particleEmissions = particles.emission;
            float emissionRate = particleEmissions.rateOverTime.constant;
            particleShape.scale = new Vector3(scaleX, scaleY, 1.0f);
            particleEmissions.rateOverTime =  emissionRate * scaleX * scaleY;
        }
    }
}
