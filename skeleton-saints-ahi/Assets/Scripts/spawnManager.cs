using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemyTypes;
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
        GameObject newEnemy;
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
            newEnemy = enemyTypes[Random.Range(0, enemyTypes.Length)];
        }
        else
        {
            newEnemy = enemyTypes[enemyIter++];
            if (enemyIter >= enemyTypes.Length)
                enemyIter = 0;
        }

        Instantiate(newEnemy, newSpawnPos.position, newEnemy.transform.rotation);

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}
