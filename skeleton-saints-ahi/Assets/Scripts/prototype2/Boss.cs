using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] int extraHealth;

    void Start()
    {

    }

    private void OnDestroy()
    {
        gameManager.instance.updateGameGoal(-1000);
    }
}
