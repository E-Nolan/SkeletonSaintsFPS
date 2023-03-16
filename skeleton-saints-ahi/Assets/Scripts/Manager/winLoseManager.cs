using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winLoseManager : MonoBehaviour
{
    public static winLoseManager instance;
    public GameObject victory;
    public GameObject defeat;

    public void Restart()
    {
        gameManager.instance.restartLevel();
    }

    public void Respawn()
    {

    }

    public void Credits()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
