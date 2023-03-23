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
        hUDManager.instance.TransitionFromPlay();
        AsyncOperation control = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            
            yield return null;

        }
        if (control.progress >= .9f)
        {
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
    }

    IEnumerator LoadMain()
    { 
        menuManager.instance.toggleGameMenu();
        if (menuManager.instance.canToggleGameMenu == false)
            menuManager.instance.canToggleGameMenu = true;
        hUDManager.instance.closeHUD();
        hUDManager.instance.TransitionFromPlay();
        hUDManager.instance.clearWeapons();
        loading.SetActive(true);
        AsyncOperation control = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return null;
        }
        if (control.progress >= .9f)
        {
            control.completed += (sceneComplete) =>
            {
                loading.SetActive(false);
                menuManager.instance.ActivateMenu();
                gameManager.instance.beginGame();
            };
        }

        musicManager.instance.StartTrackWithName("Shamanistic");
    }

    IEnumerator LoadCurrent()
    {
        menuManager.instance.toggleGameMenu();
        hUDManager.instance.closeHUD();
        hUDManager.instance.TransitionFromPlay();
        loading.SetActive(true);
        AsyncOperation control = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        while(!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";

            yield return new WaitForSeconds(time);
        }
        if (control.progress >= .9f)
        {
            control.completed += (sceneComplete) =>
            {
                if (gameManager.instance.playerInstance == null)
                {
                    loading.SetActive(false);
                    gameManager.instance.LevelSetup();
                    menuManager.instance.unPause();
                }

            };
        }
    }

 
    IEnumerator LoadBeginning()
    {
        menuManager.instance.toggleGameMenu();
        hUDManager.instance.closeHUD();
        hUDManager.instance.TransitionFromPlay();
        hUDManager.instance.clearWeapons();
        loading.SetActive(true);
        AsyncOperation control = SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        while (!control.isDone)
        {
            float slide = Mathf.Clamp01(control.progress / .9f);
            loadingSlider.value = slide;
            loadingValue.GetComponent<TextMeshProUGUI>().text = $"{slide * 100}";
      
            yield return new WaitForSeconds(time);
        }
        if (control.progress >= .9f)
        {
            control.completed += (sceneComplete) =>
            {
                if (gameManager.instance.playerInstance == null)
                {
                    loading.SetActive(false);
                    gameManager.instance.LevelSetup();
                    menuManager.instance.unPause();
                }
            };
        }
    }
    public Scene currentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
