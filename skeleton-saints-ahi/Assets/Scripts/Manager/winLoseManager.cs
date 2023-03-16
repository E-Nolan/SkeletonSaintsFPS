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

    private void Awake()
    {
        instance = this;
    }

    public void DisplayWin()
    {
        victory.SetActive(true);
        pauseMenuManager.instance.pause();
    }

    public void DisplayLose()
    {
        pauseMenuManager.instance.pause();
        defeat.SetActive(true);
        
    }

    public void PlayCredits()
    {
        int scrollSpeed = 2;
        int i = 3200;
        victory.SetActive(false);
        credits.SetActive(true);
        
    }
}
