using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTransition : MonoBehaviour
{
    public static levelTransition instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchToLevel();
        }
    }

    /// <summary>
    /// Leave string name blank to switch to the next level
    /// </summary>
    /// <param name="_sceneName"></param>
    public static void SwitchToLevel()
    {
        for (int i = 0; i < gameManager.instance.keyCard.Length; i++)
        {
            gameManager.instance.keyCard[i] = false;
        }
        savePlayer.instance.updatePlayer();
        SwitchToLevel();
        sceneLoader.instance.LoadNextScene();
        //saveManager.SaveGameData(saveManager.mainData_Current);


    }
}
