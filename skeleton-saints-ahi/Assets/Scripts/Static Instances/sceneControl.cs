using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class sceneControl
{
    static sceneControl()
    {
    }
    private sceneControl()
    {
    }
    public static sceneControl instance { get; } = new sceneControl();

    public void LoadMainMenuScene()
    {
        //Load 1st scene in build order which should be made sure is the main menu scene. Can also do this by name once that is stuctrued but could be slower
        AsyncOperation control = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        control.completed += (sceneComplete) => {
            menuManager.instance.InitializeMenusText();
        };

    }

    public void LoadMainLevel()
    {
        AsyncOperation control = SceneManager.LoadSceneAsync("Level One", LoadSceneMode.Single);
        control.completed += (sceneComplete) => {
            gameManager.instance.FetchEvents();
            gameManager.instance.LevelSetup();
        };
    }
    public void SceneRestart_Game()
    {
        //If scence currently loaded is a level scene (player with enemies and objectives)
        if (!SceneManager.GetSceneByName("Main Menu").isLoaded)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
        //Game manager will make call to BeginGame() again which loads main menu
    }
    public void SceneRestart()
    {
        int currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Current Scene should be the one loaded after the paersistent one (1)

        AsyncOperation control = SceneManager.LoadSceneAsync(currSceneIndex, LoadSceneMode.Single);
        control.completed += (sceneComplete) => {
            gameManager.instance.FetchEvents();
            gameManager.instance.LevelSetup();
        };
    }

    public string SceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
