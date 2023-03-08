using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyActivationArea : interactableArea
{
    public GameObject enemyToActivate;
    public Transform spawnPos;

    public bool bossSpawned;
    public override void InteractWithArea()
    {
        if (!bossSpawned)
        {
            Instantiate(enemyToActivate, spawnPos);
            bossSpawned = true;
        }
    }
}
