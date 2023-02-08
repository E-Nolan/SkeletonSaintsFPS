using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemyTypes;
    [Range(1,10)] [SerializeField] int spawnMaxNum;
    [Range(0.0f, 10.0f)] [SerializeField] float spawnCooldown;
    [SerializeField] Transform[] spawnPos;

    int enemiesSpawned;
    bool playerInRange;
    bool isSpawning;

    void Start()
    {
        
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
        // Choose a random enemy from the enemyTypes array and spawn it a random spawn Position
        isSpawning = true;
        GameObject newEnemy = enemyTypes[Random.Range(0, enemyTypes.Length)];
        Instantiate(newEnemy, spawnPos[Random.Range(0, spawnPos.Length)].position, newEnemy.transform.rotation);
        enemiesSpawned++;

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}
