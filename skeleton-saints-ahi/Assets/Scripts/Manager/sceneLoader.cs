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
    public GameObject loadingValue;
    private float time;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        time = 2f;
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMain());
    }

    public void RestartLevel()
    {

    }

    public void RestartGame()
    {

    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        AsyncOperation control = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        loading.SetActive(true);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);
            control.completed += (sceneComplete) =>
            {
                loading.SetActive(false);
                gameManager.instance.LevelSetup();
                hUDManager.instance.showHUD();
                gameManager.instance.isPaused = false;
                gameManager.instance.FetchEvents();
                
            };
        }
    }

    IEnumerator LoadMain()
    {
        menuManager.instance.toggleGameMenu();
        menuManager.instance.quitGame.SetActive(false);

        loading.SetActive(true);
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        AsyncOperation control = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);
            control.completed += (sceneComplete) =>
            {
                loading.SetActive(false);
                menuManager.instance.ActivateMenu();
                gameManager.instance.beginGame();
            };
        }
    }
}
