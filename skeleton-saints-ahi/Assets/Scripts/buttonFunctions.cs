using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.unPause();
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }

    public void restart()
    {
        gameManager.instance.unPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void startGame()
    {
        gameManager.instance.activeMenu = null;
        gameManager.instance.activeMenu = gameManager.instance.difficultyMenu;
    }

    public void credits()
    {
        gameManager.instance.activeMenu = gameManager.instance.creditsMenu;

    }

    public void easyGame()
    {

    }

    public void normalGame()
    {
        SceneManager.LoadScene("Prototype Level");
    }

    public void hardGame()
    {

    }

    public void menuBack()
    {

    }

}
