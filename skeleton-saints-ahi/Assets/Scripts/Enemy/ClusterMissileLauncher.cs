using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class ClusterMissileLauncher : MonoBehaviour
{
    [Tooltip("Number of missiles to spawn at once")]
    public int MissileCount;

    [Tooltip("Timer until next wave of missiles is spawned")]
    [Range(0.1f, 10f)] public float MissileTimer;

    [Tooltip("Radius to spawn missiles within")]
    [Range(0.1f, 5f)] public float SpawnRadius;

    [Tooltip("Cluster missile prefab")]
    [SerializeField] private GameObject missile;

    [SerializeField] private bool missilesLaunched = false;


    void Start()
    {
        missilesLaunched = false;
    }

    void Update()
    {
        if (!missilesLaunched)
        {
            missilesLaunched = true;

            for (int i = 0; i < MissileCount; ++i)
            {
                Vector3 tempVector = Random.insideUnitCircle * SpawnRadius;
                Vector3 spawnPosition = transform.position + tempVector;
                Vector3 spawnDirection = -(transform.forward).normalized;
                //GameObject tempMissile = Instantiate(missile, transform.position, transform.rotation);
                //missile.transform.LookAt(missile.transform.forward, gameManager.instance.playerInstance.transform.position);
                Instantiate(missile, spawnPosition, 
                    Quaternion.LookRotation(spawnDirection));
            }

            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(MissileTimer);
        missilesLaunched = false;
    }
}
