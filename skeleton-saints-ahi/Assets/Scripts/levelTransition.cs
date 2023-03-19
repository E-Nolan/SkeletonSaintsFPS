using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTransition : MonoBehaviour
{
    [Tooltip("If false, will just load the next scene in the build")]
    [SerializeField] bool loadSpecificScene;
    [SerializeField] string specificSceneName;

    private void Awake()
    {
        if (!loadSpecificScene)
            specificSceneName = "";
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
        saveManager.SaveGameData(saveManager.mainData_Current);
        savePlayer.instance.updatePlayer();
        sceneLoader.instance.LoadNextScene();
    }
}
