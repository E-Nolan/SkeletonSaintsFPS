using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuButton : MonoBehaviour
{
    //start menu
    public void StartMenu()
    {
        mainMenuManager.instance.DisplayStart();
    }

    public void SetEasy()
    {
        mainMenuManager.instance.easyMode();
    }

    public void SetNormal()
    {
        mainMenuManager.instance.normalMode();
    }

    public void SetHard()
    {
        mainMenuManager.instance.hardMode();
    }

    //control menu
    public void ControlMenu()
    {
        mainMenuManager.instance.DisplayControl();
    }


    //settings menu
    public void SettingsMenu()
    {
        mainMenuManager.instance.DisplaySettings();
    }

    //credits menu
    public void CreditsMenu()
    {
        mainMenuManager.instance.DisplayCredits();
    }

    public void NextPage()
    {
        mainMenuManager.instance.NextPage();
    }

    public void PreviousPage()
    {
        mainMenuManager.instance.PreviousPage();
    }

    //quit menu
    public void QuitMenu()
    {
        mainMenuManager.instance.DisplayQuit();
    }
}
