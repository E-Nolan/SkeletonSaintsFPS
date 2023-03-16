using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonFunctions : MonoBehaviour
{
    #region Game Menu Buttons
    public void Resume()
    {
        pauseMenuManager.instance.toggleGameMenu();
    }
    public void Restart()
    {
        gameManager.instance.restartLevel();
    }
    public void Respawn()
    {
        gameManager.instance.respawn();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void QuitToMenu()
    {
        gameManager.instance.restartGame();
    }
    #endregion

    #region Main Menu Buttons
    public void StartLevel()
    {
        //Get rid of main menu camera
        Destroy(Camera.main.gameObject);
        gameManager.instance.InitializePlay();
    }
    public void OpenCreditsPanel()
    {
        menuManager.instance.DisplayCreditsMenu();
    }
    public void OpenSettingsPanel()
    {
        menuManager.instance.DisplaySettingsMenu();
    }
    #endregion

    public void PlayButtonSound()
    {
        menuManager.instance.MenusAudio.PlayOneShot(menuManager.instance.ButtonClick);
    }
}
