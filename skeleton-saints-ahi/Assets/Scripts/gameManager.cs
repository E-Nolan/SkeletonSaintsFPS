using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject mainMenu;
    public GameObject difficultyMenu;
    public GameObject creditsMenu;
    public Image playerHealthBar;
    public Image playerStaminaBar;
    public Image playerArmorBar;
    public TextMeshProUGUI playerAmmoText;
    public TextMeshProUGUI keyCard01Text;
    public TextMeshProUGUI keyCard02Text;
    public TextMeshProUGUI enemiesCounter;
    public GameObject damageFlashScreen;
    public Image card01;
    public Image card02;

    [Header("----- Game Goals -----")]
    public int enemiesRemaining;
    public GameObject keyCard01;
    [SerializeField] public bool kCard01;
    public GameObject keyCard02;
    [SerializeField] public bool kCard02;

    public bool isPaused;

    private void Awake()
    {   
        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            activeMenu = mainMenu;
            activeMenu.SetActive(true);
            Time.timeScale = 3;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            GameObject.Find("Reticle").SetActive(false);
            GameObject.Find("PlayerStats").SetActive(false);
            GameObject.Find("EnemyStats").SetActive(false);
            GameObject.Find("WeaponStats").SetActive(false);
        }
        else
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<playerController>();
        }
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
        updateEnemyCounter();
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
