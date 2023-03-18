using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuButton : MonoBehaviour
{
    public void DisplayStart()
    {
        menuManager.instance.DisplayStart();
    }

    public void EasyMode()
    {
        menuManager.instance.EasyMode();
        gameManager.instance.currentDifficulty = gameManager.Difficulty.Easy;
    }

    public void NormalMode()
    {
        menuManager.instance.NormalMode();
        gameManager.instance.currentDifficulty = gameManager.Difficulty.Normal;
    }

    public void HardMode()
    {
        menuManager.instance.HardMode();
        gameManager.instance.currentDifficulty = gameManager.Difficulty.Hard;
    }

    public void PlayGame()
    {
        gameManager.instance.InitializePlay();
    }

    public void DisplayControls()
    {
        menuManager.instance.DisplayControls();
    }

    public void DisplaySettings()
    {
        menuManager.instance.DisplaySettings();
    }

    public void DisplayMenuQuit()
    {
        menuManager.instance.DisplayExitGame();
    }

    public void DisplayObjective()
    {
       
    }

    public void DisplayCredits()
    {
        
    }

    public void DisplayGame()
    {
        menuManager.instance.DisplayGame();
    }

    public void DisplayObjectives()
    {
        menuManager.instance.DisplayObjective();
    }

    public void DisplayExitGame()
    {
        menuManager.instance.DisplayExitGame();
    }

    public void DisplayGameQuit()
    {
        menuManager.instance.DisplayGameQuit();
    }

    public void GameQuit()
    {
        menuManager.instance.GameQuit();
    }
}
