using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sceneLoader : MonoBehaviour
{
    public static sceneLoader instance;
    public GameObject loading;
    public Slider loadingSlider;
    public GameObject lSlider;
    public GameObject loadingValue;
    public GameObject confirm;
    public bool isReady;
    public bool confirmed;

    private void Awake()
    {
        instance = this;
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadScenAsynchronously(sceneName));
    }

    IEnumerator LoadScenAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loading.SetActive(true);
        while (!operation.isDone)
        {
            float slide = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";
            yield return null;
        }
        /*
        if (operation.isDone)
        {
            lSlider.SetActive(false);
            loadingValue.SetActive(false);
            confirm.SetActive(true);
            if (Input.GetButton(playerPreferences.instance.Button_Interact))
                confirmed = !confirmed;
        }
        */
        //confirmed = !confirmed;
        loading.SetActive(false);
    }
}
