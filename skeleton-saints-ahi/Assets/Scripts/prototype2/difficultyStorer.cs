using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class difficultyStorer : MonoBehaviour
{
    public gameManager.GameDifficulty currentGameDifficulty = gameManager.GameDifficulty.Easy;
    public static difficultyStorer instance;
    private void Awake()
    {
        instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
