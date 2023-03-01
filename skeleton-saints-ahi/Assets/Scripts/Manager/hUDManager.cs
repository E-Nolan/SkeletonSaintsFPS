using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class hUDManager : MonoBehaviour
{
    public static hUDManager instance;

    [Header("----- UI -----")]
    public Image playerHealthBar;
    public Image playerStaminaBar;
    public Image playerArmorBar;
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
    float currentHealth;
    float maxStamina;
    float currentStamina;
    float maxArmor;
    float currentArmor;

    void Awake()
    {
        instance = this;
    }
    #region Access Methods

    public Image PlayerHPBar
    {
        get { return playerHealthBar; }
        set { playerHealthBar = value; }
    }
    #endregion
    #region Public Methods
    public void updatePlayerHealthBar()
    {
        currentHealth = gameManager.instance.PlayerScript().GetCurrentHealth();
        maxHealth = gameManager.instance.PlayerScript().GetMaxHealth();
        playerHealthBar.fillAmount = currentHealth / maxHealth;
    }

    public void updatePlayerStaminaBar()
    {
        currentStamina = gameManager.instance.PlayerScript().GetCurrentStamina();
        maxStamina = gameManager.instance.PlayerScript().GetMaxStamina();
        playerStaminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void updatePlayerArmorBar()
    {
        currentArmor = gameManager.instance.PlayerScript().GetCurrentArmor();
        maxArmor = gameManager.instance.PlayerScript().GetMaxArmor();
        playerArmorBar.fillAmount = currentArmor / maxArmor;
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
