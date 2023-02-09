using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Information -----")]
    public GameObject player;
    public playerController playerScript;

    [Header("----- UI -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Image playerHealthBar;
    public Image playerStaminaBar;
    public TextMeshProUGUI playerAmmoText;
    public TextMeshProUGUI enemiesCounter;

    [Header("----- Game Goals -----")]
    public int enemiesRemaining;

    public bool isPaused;

    private void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)
                pause();
            else
                unPause();
        }
    }

    public void pause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void updateGameGoal(int amount)
    {
        enemiesRemaining += amount;

        if(enemiesRemaining <= 0)
        {
            pause();
            activeMenu = winMenu;
            activeMenu.SetActive(true);
        }
    }

    public void updateEnemyCounter()
    {
        gameManager.instance.enemiesCounter.text = $"{enemiesRemaining}";
    }

    public void playerDead()
    {
        pause();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
}
