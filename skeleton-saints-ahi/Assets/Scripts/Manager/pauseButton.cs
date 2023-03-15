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

    }

    public void SetRestart()
    {
        pauseMenuManager.instance.Restart();
    }

    public void Confirm()
    {

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

}
