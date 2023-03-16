using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadingScreen : MonoBehaviour
{
    public List<GameObject> info;
    public GameObject loading;
    public Slider slider;

    public void LoadLevel (int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation oper = SceneManager.LoadSceneAsync(sceneIndex);

        loading.SetActive(true);

        while (!oper.isDone)
        {
            float progress = Mathf.Clamp01(oper.progress / .9f);
            //Debug.Log(progress);
            slider.value = progress;
            yield return null;
        }
    }
}
