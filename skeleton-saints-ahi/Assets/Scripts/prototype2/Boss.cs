using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] int extraHealth;

    private void OnDestroy()
    {
        gameManager.instance.winGame();
    }
}
