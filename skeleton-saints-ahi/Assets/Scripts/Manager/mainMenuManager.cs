using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuManager : MonoBehaviour
{
    public static mainMenuManager instance;

    [Header("----- Start Panels -----")]
    public GameObject startMenu;
    public List<GameObject> difficulty;
    public GameObject currentDifficulty;

    [Header("----- Controls Panels -----")]
    public GameObject controlMenu;

    [Header("----- Settings Panels -----")]
    public GameObject settingsMenu;

    [Header("----- Credits Panels -----")]
    public GameObject creditsMenu;
    public List<GameObject> credits;
    public GameObject currentCredits;
    public int currentPage;
    public GameObject nextPage;
    public GameObject previousPage;

    [Header("----- Quit Panels -----")]
    public GameObject quitMenu;

    [Header("----- Menu Options -----")]
    public GameObject activeMenu;

    private void Start()
    {
        instance = this;
        startMenu.SetActive(true);
        activeMenu = startMenu;
        currentPage = 0;
        difficulty[0].SetActive(true);
    }

    //Start menu
    public void DisplayStart()
    {
        if (activeMenu != startMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = startMenu;
            startMenu.SetActive(true);
        }
    }

    public void easyMode()
    {
        
        if(currentDifficulty != difficulty[0])
        {
            currentDifficulty.SetActive(false);
            currentDifficulty = difficulty[0];
            difficulty[0].SetActive(true);
            gameManager.instance.currentDifficulty = gameManager.Difficulty.Easy;
        }
    }

    public void normalMode()
    {
        
        if (currentDifficulty != difficulty[1])
        {
            currentDifficulty.SetActive(false);
            currentDifficulty = difficulty[1];
            difficulty[1].SetActive(true);
            gameManager.instance.currentDifficulty = gameManager.Difficulty.Normal;
        }
    }

    public void hardMode()
    {
        
        if (currentDifficulty != difficulty[2])
        {
            currentDifficulty.SetActive(false);
            currentDifficulty = difficulty[2];
            difficulty[2].SetActive(true);
            gameManager.instance.currentDifficulty = gameManager.Difficulty.Hard;
        }
    }

    //Control menu
    public void DisplayControl()
    {
        if (activeMenu != controlMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = controlMenu;
            controlMenu.SetActive(true);
        }
    }


    //settings menu
    public void DisplaySettings()
    {
        if (activeMenu != settingsMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = settingsMenu;
            settingsMenu.SetActive(true);
        }
    }

    //credits menu
    public void DisplayCredits()
    {
        if (activeMenu != creditsMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = creditsMenu;
            creditsMenu.SetActive(true);
            currentCredits = credits[currentPage];
            credits[currentPage].SetActive(true);
        }
    }

    public void NextPage()
    {
        credits[currentPage].SetActive(false);
        currentPage++;
        currentCredits = credits[currentPage];
        credits[currentPage].SetActive(true);
    }

    public void PreviousPage()
    {
        credits[currentPage].SetActive(false);
        currentPage--;
        currentCredits = credits[currentPage];
        credits[currentPage].SetActive(true);
    }

   //quit menu
    public void DisplayQuit()
    {
        if (activeMenu != quitMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = quitMenu;
            quitMenu.SetActive(true);
        }
    }
}
