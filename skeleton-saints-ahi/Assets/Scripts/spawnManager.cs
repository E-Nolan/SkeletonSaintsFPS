using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemyTypes;
    [SerializeField] GameObject[] weaponTypes;
    [SerializeField] GameObject[] particles;
    [Range(1,10)] [SerializeField] int spawnMaxNum;
    [Range(0.0f, 10.0f)] [SerializeField] float spawnCooldown;
    [SerializeField] Transform[] spawnPos;

    [SerializeField] bool spawnInRandomPositions;
    [SerializeField] bool spawnRandomEnemyTypes;

    int enemyIter = 0;
    int posIter = 0;
    int enemiesSpawned;
    bool playerInRange;
    bool isSpawning;

    void Start()
    {
        gameManager.instance.updateGameGoal(spawnMaxNum);
    }

    void Update()
    {
        if (playerInRange && !isSpawning && enemiesSpawned < spawnMaxNum)
        {
            StartCoroutine(spawnEnemy());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    IEnumerator spawnEnemy()
    {
        isSpawning = true;
        enemiesSpawned++;

        Transform newSpawnPos;
        GameObject tempEnemy;
        int weaponIndex = 0;
        GameObject newWeapon = null;

        if (weaponTypes.Length > 0)
        {
            weaponIndex = Random.Range(0, weaponTypes.Length);
            newWeapon = weaponTypes[weaponIndex];
        }

        // If the spawner uses random spawn Positions, it will choose a random spawn Position to spawn the next enemy
        // Otherwise it will iterate/loop through the spawnPos array
        if (spawnInRandomPositions)
        {
            newSpawnPos = spawnPos[Random.Range(0, spawnPos.Length)];
        }
        else
        {
            newSpawnPos = spawnPos[posIter++];
            if (posIter >= spawnPos.Length)
                posIter = 0;
        }

        // If the spawner spawns random enemy types, it will choose a random enemy each time one is spawned.
        // Otherwise it will iterate/loop through the enemyTypes array
        if (spawnRandomEnemyTypes)
        {
            tempEnemy = enemyTypes[Random.Range(0, enemyTypes.Length)];
        }
        else
        {
            tempEnemy = enemyTypes[enemyIter++];
            if (enemyIter >= enemyTypes.Length)
                enemyIter = 0;
        }
        GameObject newEnemy = Instantiate(tempEnemy, newSpawnPos.position, tempEnemy.transform.rotation);
        if (newEnemy != null)
        {
            if(weaponTypes.Length > 0 && newWeapon != null)
                newEnemy.GetComponent<Enemy>().PickupWeapon(newWeapon.GetComponent<weaponPickup>().weapon);

            // NOTE: Particles only spawn one Particle System per spawn position
            if (particles.Length > 1)
            {
                Vector3 particleOffset = newEnemy.transform.position + new Vector3(0f, newEnemy.transform.localScale.y / 2, 0f);

                foreach (GameObject particle in particles)
                {
                    GameObject temp = Instantiate(particle, particleOffset,
                        Quaternion.Euler(newEnemy.transform.forward));

                    Destroy(temp, temp.GetComponent<ParticleSystem>().main.duration);
                }
            } 
            else if (particles.Length == 1)
            {
                Vector3 particleOffset = newEnemy.transform.position + new Vector3(0f, newEnemy.transform.localScale.y / 2, 0f);
                //GameObject temp = Instantiate(particles[0], particleOffset,
                //    Quaternion.Euler(newEnemy.transform.forward));

                GameObject temp = Instantiate(particles[0], particleOffset,
                    particles[0].transform.rotation);

                Destroy(temp, temp.GetComponent<ParticleSystem>().main.duration);
            }

            newEnemy.SetActive(true);

        }

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}
