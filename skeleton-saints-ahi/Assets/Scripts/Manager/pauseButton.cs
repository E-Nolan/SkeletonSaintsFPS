using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseButton : MonoBehaviour
{
    public void GameMenu()
    {
        pauseMenuManager.instance.DisplayGame();
    }

    public void SetRespawn()
    {
        pauseMenuManager.instance.Respawn();
    }

    public void ConfirmRespawn()
    {

    }


    public void Resume()
    {
        pauseMenuManager.instance.toggleGameMenu();
    }

    public void SetRestart()
    {
        pauseMenuManager.instance.Restart();
    }

    public void ConfirmRestart()
    {
        gameManager.instance.restartLevel();
    }

    public void ControlMenu()
    {
        pauseMenuManager.instance.DisplayControl();
    }

    public void SettingsMenu()
    {
        pauseMenuManager.instance.DisplaySettings();
    }

    public void SaveSettings()
    {
        pauseMenuManager.instance.SaveSettingsToPlayer();
    }

    public void ResetSetting()
    {
        pauseMenuManager.instance.ResetDefault();
    }

    public void ObjectivesMenu()
    {
        pauseMenuManager.instance.DisplayObjectives();
    }

    public void QuitMenu()
    {
        pauseMenuManager.instance.DisplayQuit();
    }

    public void ConfirmQuit()
    {
        gameManager.instance.restartGame();
    }
}
