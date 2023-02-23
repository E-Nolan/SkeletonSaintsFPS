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


    [Header ("----- Menu's -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject mainMenu;
    public GameObject difficultyMenu;
    public GameObject creditsMenu;

    [Header("----- UI -----")]
    public Image playerHealthBar;
    public Image playerStaminaBar;
    public Image playerArmorBar;
    public TextMeshProUGUI playerAmmoText;
    public Image playerActiveGun;

    public Image card01;
    public TextMeshProUGUI keyCard01Text;
    public Image card02;
    public TextMeshProUGUI keyCard02Text;
    public Image card03;
    public TextMeshProUGUI keyCard03Text;
    
    [Header("----- Game Goals -----")]
    public int enemiesRemaining;
    public TextMeshProUGUI enemiesCounter;
    [SerializeField] public bool[] keyCard = new bool[3];
    [Range(.1f,2.0f)][SerializeField] public float easyMode;
    [Range(.1f, 2.0f)] [SerializeField] public float normalMode;
    [Range(.1f, 2.0f)] [SerializeField] public float hardMode;

    [Header ("----- MISC -----")]
    public GameObject damageFlashScreen;

    /// <summary>
    /// Extra resources needed for gameManager tools 
    /// </summary>
    
    public bool isPaused;
    public bool isUIActive;
    float maxHealth;
    float currentHealth;
    float maxStamina;
    float currentStamina;
    float maxArmor;
    float currentArmor;

    public Vector3 labOneSpawn;
    public Vector3 labTwoSpawn;
    public Vector3 labThreeSpawn;

    private void Awake()
    {
        /// <summary>
        /// if mainmenu is active scenet then disable the ui additions so that
        /// only one thing is availble to be seen and interacted with
        /// </summary>
        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            isUIActive = !isUIActive;
            activeMenu = mainMenu;
            activeMenu.SetActive(true);
            Time.timeScale = 3;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            GameObject.Find("Reticle").SetActive(false);
            GameObject.Find("PlayerStats").SetActive(false);
            GameObject.Find("EnemyStats").SetActive(false);
            GameObject.Find("WeaponStats").SetActive(false);
            GameObject.Find("Key Cards").SetActive(false);
        }
        //when not main menu, should be operating normally
        else
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<playerController>();
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameObject.Find("Reticle").SetActive(true);
            GameObject.Find("PlayerStats").SetActive(true);
            GameObject.Find("EnemyStats").SetActive(true);
            GameObject.Find("WeaponStats").SetActive(true);
            GameObject.Find("Key Cards").SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)
                pause();
            else
                unPause();
        }

        if(!isUIActive)
        {
            updatePlayerHealthBar();
            updatePlayerStaminaBar();
            updatePlayerArmorBar();
            updateActiveGun();
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

    public void updatePlayerHealthBar()
    {
        currentHealth = gameManager.instance.playerScript.GetCurrentHealth();
        maxHealth = gameManager.instance.playerScript.GetMaxHealth();
        playerHealthBar.fillAmount = currentHealth / maxHealth;
    }

    public void updatePlayerStaminaBar()
    {
        currentStamina = gameManager.instance.playerScript.GetCurrentStamina();
        maxStamina = gameManager.instance.playerScript.GetMaxStamina();
        playerStaminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void updatePlayerArmorBar()
    {
        currentArmor = gameManager.instance.playerScript.GetCurrentArmor();
        maxArmor = gameManager.instance.playerScript.GetMaxArmor();
        playerArmorBar.fillAmount = currentArmor / maxArmor;
    }


    public void updateActiveGun()
    {
        playerActiveGun.GetComponent<Image>().sprite = gameManager.instance.playerScript.currentWeapon.activeImage;
    }

    public IEnumerator setEasyMode()
    {
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator setNormalMode()
    {
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator setHardMode()
    {
        yield return new WaitForSeconds(1f);
    }
}
