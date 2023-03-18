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
            SwitchToLevel(specificSceneName);
        }
    }

    /// <summary>
    /// Leave string name blank to switch to the next level
    /// </summary>
    /// <param name="_sceneName"></param>
    public static void SwitchToLevel(string _sceneName = "")
    {
        for (int i = 0; i < gameManager.instance.keyCard.Length; i++)
        {
            gameManager.instance.keyCard[i] = false;
        }
        saveManager.SaveGameData(saveManager.mainData_Current);

        if (_sceneName == "")
        {
            sceneControl.instance.LoadNextLevel();
        }
        else
        {
            sceneControl.instance.LoadSpecificLevel(_sceneName);
        }
    }
}
