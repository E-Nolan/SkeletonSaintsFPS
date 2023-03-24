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
        hUDManager.instance.TransitionFromPlay();
        saveManager.SaveGameData(saveManager.mainData_Current);

        savePlayer.instance.updatePreferences();
        sceneLoader.instance.LoadNextScene();
    }

}
