using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTransition : MonoBehaviour
{
    [Tooltip("If false, will just load the next scene in the build")]
    [SerializeField] bool loadSpecificScene;
    [SerializeField] string specificSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < gameManager.instance.keyCard.Length; i++)
            {
                gameManager.instance.keyCard[i] = false;
            }
            saveManager.SaveGameData(saveManager.mainData_Current);

            switch (loadSpecificScene)
            {
                case true:
                    sceneControl.instance.LoadSpecificLevel(specificSceneName);
                    break;
                case false:
                    sceneControl.instance.LoadNextLevel();
                    break;
            }
        }
    }
}
