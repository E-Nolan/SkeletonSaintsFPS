using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class hUDManager : MonoBehaviour
{
    public static hUDManager instance;

    [Header("----- UI -----")]
    public GameObject playerHealthBar;
    public GameObject parentHealth;
    public List<GameObject> playerHealthTick;
    public GameObject playerStaminaBar;
    public GameObject parentStamina;
    public List<GameObject> playerStaminaTick;
    public GameObject playerArmorBar;
    public GameObject parentArmor;
    public List<GameObject> playerArmorTick;
    public Image reticle;
    public TextMeshProUGUI playerAmmoText;
    public Image playerActiveGun;

    public Image card01;
    public TextMeshProUGUI keyCard01Text;
    public Image card02;
    public TextMeshProUGUI keyCard02Text;
    public Image card03;
    public TextMeshProUGUI keyCard03Text;

    public GameObject damageFlashScreen;


    public int enemiesRemaining;
    public TextMeshProUGUI enemiesCounter;

    float maxHealth;
    int healthTick;
    float currentHealth;
    float maxStamina;
    int staminaTick;
    float currentStamina;
    float maxArmor;
    int armorTick;
    float currentArmor;

    void Awake()
    {
        instance = this;
    }
    #region Access Methods

    //public Image PlayerHPBar
    //{
      //  get { return playerHealthBar; }
        //set { playerHealthBar = value; }
    //}
    #endregion
    #region Public Methods
    public void createPlayerHealthBar()
    {
        maxHealth = gameManager.instance.PlayerScript().GetMaxHealth();
        healthTick = (int)maxHealth / 1;
        for (int i = 0; i < healthTick; i++)
        {
            GameObject tick = Instantiate(playerHealthBar, new Vector3(playerHealthBar.transform.position.x - (i * 18), playerHealthBar.transform.position.y, playerHealthBar.transform.position.z), Quaternion.identity, parentHealth.transform);
            tick.SetActive(true);
            playerHealthTick.Add(tick);
        }
    }

    public void updatePlayerHealthBar()
    {
        currentHealth = gameManager.instance.PlayerScript().GetCurrentHealth();
        maxHealth = gameManager.instance.PlayerScript().GetMaxHealth();
        int currentTick = (int)currentHealth;
        int diff = currentTick - playerHealthTick.Count;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        for (int i = 0; i < currentTick; i++)
        {
            playerHealthTick[i].SetActive(true);
        }
        for (int i = currentTick; i < playerHealthTick.Count; i++)
        {
            playerHealthTick[i].SetActive(false);
        }
        if (diff >= 0)
            playerHealthTick[(playerHealthTick.Count - 1) - diff].SetActive(true);
    }

    public void createPlayerStaminaBar()
    {
        maxStamina = gameManager.instance.PlayerScript().GetMaxStamina();
        staminaTick = (int)maxStamina / 10;
        for (int i = 0; i < staminaTick; i++)
        {
            GameObject tick = Instantiate(playerStaminaBar, new Vector2(playerStaminaBar.transform.position.x - (i * 18), playerStaminaBar.transform.position.y), Quaternion.identity, parentStamina.transform);
            tick.SetActive(true);
            playerStaminaTick.Add(tick);
        }
    }

    public void updatePlayerStaminaBar()
    {
        currentStamina = gameManager.instance.PlayerScript().GetCurrentStamina();
        maxStamina = gameManager.instance.PlayerScript().GetMaxStamina();
        int currentTick = (int)currentStamina / 10;
        int diff = playerStaminaTick.Count - currentTick;
        if ((gameManager.instance.PlayerScript().isDashing || gameManager.instance.PlayerScript().isSprinting) && currentStamina > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                playerStaminaTick[(playerStaminaTick.Count - 1) - i].SetActive(false);
            }
        }
        else if ((!gameManager.instance.PlayerScript().isDashing && !gameManager.instance.PlayerScript().isSprinting) || currentStamina == maxStamina)
        {
            if (diff >= 0)
                playerStaminaTick[(playerStaminaTick.Count - 1) - diff].SetActive(true);
        }
    }

    public void createPlayerArmorBar()
    {
        maxArmor = gameManager.instance.PlayerScript().GetMaxArmor();
        armorTick = (int)maxArmor / 1;
        for (int i = 0; i < armorTick; i++)
        {
            GameObject tick = Instantiate(playerArmorBar, new Vector2(playerArmorBar.transform.position.x - (i * 18), playerArmorBar.transform.position.y), Quaternion.identity, parentArmor.transform);
            tick.SetActive(true);
            playerArmorTick.Add(tick);
        }
    }

    public void updatePlayerArmorBar()
    {
        currentArmor = gameManager.instance.PlayerScript().GetCurrentArmor();
        maxArmor = gameManager.instance.PlayerScript().GetMaxArmor();
        int currentTick = (int)currentArmor;
        int diff = currentTick - playerArmorTick.Count;
        currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
        for (int i = 0; i < currentTick; i++)
        {
            playerArmorTick[i].SetActive(true);
        }
        for (int i = currentTick; i < playerArmorTick.Count; i++)
        {
            playerArmorTick[i].SetActive(false);
        }
        if (diff >= 0)
            playerArmorTick[(playerArmorTick.Count - 1) - diff].SetActive(true);
    }


    public void updateActiveGun()
    {
        playerActiveGun.GetComponent<Image>().sprite = gameManager.instance.PlayerScript().currentWeapon.activeImage;
    }
    public void toggleCursorVisibility()
    {
        if (Cursor.visible == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            reticle.gameObject.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            reticle.gameObject.SetActive(true);
        }
    }
    public void showHUD()
    {
        playerHealthBar.gameObject.SetActive(true);
        reticle.gameObject.SetActive(true);
        //TaskList.gameObject.SetActive(true);
    }
    public void closeHUD()
    {
        playerHealthBar.gameObject.SetActive(true);
        reticle.gameObject.SetActive(false);
        //TaskList.gameObject.SetActive(false);
    }
    #endregion
    #region Accessors
    public GameObject DamageFlashScreen()
    {
        return damageFlashScreen;
    }
    #endregion
}