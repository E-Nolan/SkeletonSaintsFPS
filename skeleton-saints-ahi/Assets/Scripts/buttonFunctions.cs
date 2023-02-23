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

    public void easyGame()
    {
        gameManager.instance.setEasyMode();
        SceneManager.LoadScene("Prototype 2 Final");

    }

    public void normalGame()
    {
        gameManager.instance.setNormalMode();
        SceneManager.LoadScene("Prototype 2 Final");
    }

    public void hardGame()
    {
        gameManager.instance.setHardMode();
        SceneManager.LoadScene("Prototype 2 Final");
    }

}
