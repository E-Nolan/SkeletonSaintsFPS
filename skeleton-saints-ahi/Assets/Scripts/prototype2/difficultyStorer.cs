using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class difficultyStorer : MonoBehaviour
{

    gameManager.GameDifficulty currentGameDifficulty;
    public static difficultyStorer instance;

    public gameManager.GameDifficulty GameDifficulty
    {
        get 
        {
            return currentGameDifficulty;
        }
        set
        {
            currentGameDifficulty = value;
        }
    }
    private void Awake()
    {
        instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
