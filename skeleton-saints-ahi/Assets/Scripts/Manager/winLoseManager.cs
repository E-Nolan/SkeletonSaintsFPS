using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winLoseManager : MonoBehaviour
{
    public static winLoseManager instance;
    public GameObject victory;
    public GameObject defeat;
    public GameObject credits;
    public GameObject current;

    public void DisplayWin()
    {
        victory.SetActive(true);
        pauseMenuManager.instance.pause();
    }

    public void DisplayLose()
    {
        defeat.SetActive(true);
        pauseMenuManager.instance.pause();
    }

    public void PlayCredits()
    {
        int scrollSpeed = 2;
        int i = 3200;
        victory.SetActive(false);
        credits.SetActive(true);
        
    }
}
