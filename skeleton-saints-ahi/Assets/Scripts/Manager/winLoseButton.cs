using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winLoseButton : MonoBehaviour
{
    public void Restart()
    {
        gameManager.instance.restartLevel();
        winLoseManager.instance.victory.SetActive(false);
        winLoseManager.instance.defeat.SetActive(false);
        pauseMenuManager.instance.unPause();
    }

    public void Respawn()
    {

    }

    public void Credits()
    {
        winLoseManager.instance.PlayCredits();
    }

    public void Quit()
    {
        gameManager.instance.restartGame();
    }
}
