using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuButton : MonoBehaviour
{
   public void StartMenu()
    {
        mainMenuManager.instance.DisplayStart();
    }

    public void ControlMenu()
    {
        mainMenuManager.instance.DisplayControl();
    }

    public void SettingsMenu()
    {
        mainMenuManager.instance.DisplaySettings();
    }

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

    public void QuitMenu()
    {
        mainMenuManager.instance.DisplayQuit();
    }
}
