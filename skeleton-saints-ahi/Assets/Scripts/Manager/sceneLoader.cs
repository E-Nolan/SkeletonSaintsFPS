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
        StartCoroutine(LoadNextLevel());
        //gameManager.instance.PlayerScript().OnDeserialize();
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMain());
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadCurrent());
    }

    public void RestartGame()
    {
        StartCoroutine(LoadBeginning());
    }

    IEnumerator LoadNextLevel()
    {
        loading.SetActive(true);
        hUDManager.instance.closeHUD();
        AsyncOperation control = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);

        }
        control.completed += (sceneComplete) =>
        {
            loading.SetActive(false);
            if (gameManager.instance.playerInstance == null)
            {
                gameManager.instance.LevelSetup();
                hUDManager.instance.showHUD();
                gameManager.instance.isPaused = false;
            }
        };
    }

    IEnumerator LoadMain()
    {
        if (menuManager.instance.canToggleGameMenu == false)
            menuManager.instance.canToggleGameMenu = true;
        menuManager.instance.toggleGameMenu();
        hUDManager.instance.closeHUD();
        gameEventManager.instance.ClearEventListUI();
        loading.SetActive(true);
        AsyncOperation control = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);
        }
        control.completed += (sceneComplete) =>
        {
            loading.SetActive(false);
            menuManager.instance.ActivateMenu();
            gameManager.instance.beginGame();
        };
    }

    IEnumerator LoadCurrent()
    {
        menuManager.instance.toggleGameMenu();
        hUDManager.instance.closeHUD();
        loading.SetActive(true);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        AsyncOperation control = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);
        }
        control.completed += (sceneComplete) =>
        {
            if (gameManager.instance.playerInstance == null)
            {
                loading.SetActive(false);
                hUDManager.instance.showHUD();
                gameManager.instance.LevelSetup();
            }

        };
    }

 
    IEnumerator LoadBeginning()
    {
        menuManager.instance.toggleGameMenu();
        hUDManager.instance.closeHUD();
        loading.SetActive(true);
        AsyncOperation control = SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        while (!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);
        }
        control.completed += (sceneComplete) =>
        {
            if (gameManager.instance.playerInstance == null)
            {
                loading.SetActive(false);
                hUDManager.instance.showHUD();
                gameManager.instance.LevelSetup();
            }
        };
    }
    
}
