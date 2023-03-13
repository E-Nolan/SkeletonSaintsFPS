using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class difficultyStorer : MonoBehaviour
{

    gameManager.Difficulty currentDifficulty;
    public static difficultyStorer instance;

    public gameManager.Difficulty GameDifficulty
    {
        get 
        {
            return currentDifficulty;
        }
        set
        {
            currentDifficulty = value;
        }
    }
    private void Awake()
    {
        instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
