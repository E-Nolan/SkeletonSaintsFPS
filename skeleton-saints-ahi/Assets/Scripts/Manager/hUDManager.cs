using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class hUDManager : MonoBehaviour
{
    public static hUDManager instance;
    public GameObject hud;

    [Header("----- Player UI -----")]
    public GameObject playerUISystem;
    public Slider health;
    public Slider armor;
    public Slider stamina;

    [Header("----- Weapon UI -----")]
    public GameObject weaponUISystem;
    public Image activeGun;
    public TextMeshProUGUI activeClip;
    public TextMeshProUGUI activeReserve;
    public Image pistolReserve;
    public Image[] inactiveGuns;
    public Image reticle;

    [Header("----- KeyCard UI -----")]
    public Image card01;
    public TextMeshProUGUI keyCard01Text;
    public Image card02;
    public TextMeshProUGUI keyCard02Text;
    public Image card03;
    public TextMeshProUGUI keyCard03Text;

    [Header("----- Dialogue -----")]
    public TextMeshProUGUI gateText;
    public GameObject swap;

    [Header("----- Misc UI -----")]
    public GameObject damageFlashScreen;
    public int enemiesRemaining;
    public TextMeshProUGUI enemiesCounter;
    public InputReader inputReader;

    public GameObject EventList;

    float maxHealth;
    int healthTick;
    float currentHealth;
    float maxStamina;
    int staminaTick;
    float currentStamina;
    float maxArmor;
    int armorTick;
    float currentArmor;
    int currentClip;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #region Access Methods

    private void Update()
    {
        for(int i = 0; i < gameManager.instance.keyCard.Length; i++)
        {

        }
    }

    #endregion
    #region Public Methods
    public void updateHealth()
    {
        health.maxValue = gameManager.instance.PlayerScript().MaxHealth;
        health.value = gameManager.instance.PlayerScript().currentHealth;
    }

    public void updateStamina()
    {
        stamina.maxValue = gameManager.instance.PlayerScript().MaxStamina;
        stamina.value = gameManager.instance.PlayerScript().currentStamina;
    }

    public void updateArmor()
    {
        armor.maxValue = gameManager.instance.PlayerScript().MaxArmor;
        armor.value = gameManager.instance.PlayerScript().currentArmor;
    }

    public void updateUi()
    {
        health.value = gameManager.instance.PlayerScript().currentHealth;
        stamina.value = gameManager.instance.PlayerScript().currentStamina;
        armor.value = gameManager.instance.PlayerScript().currentArmor;
    }

    public void updateWeaponDisplay()
    {
        if (gameManager.instance.PlayerScript() != null)
        {
            if (activeGun != null)
            {
                activeGun.GetComponent<Image>().sprite = gameManager.instance.PlayerScript().currentWeapon.activeImage;


                for (int i = 0; i < gameManager.instance.PlayerScript().inactiveWeapons.Count; i++)
                {
                    if (gameManager.instance.PlayerScript().inactiveWeapons[i] != null)
                        inactiveGuns[i].GetComponent<Image>().sprite = gameManager.instance.PlayerScript().inactiveWeapons[i].inactiveImage;
                }
            }
            updateWeaponText();
        }
    }
    public void updateWeaponText()
    {
        if (gameManager.instance.PlayerScript() != null)
        {
            activeClip.GetComponent<TextMeshProUGUI>().text = $"{gameManager.instance.PlayerScript().currentWeapon.CurrentClip}";
            activeReserve.GetComponent<TextMeshProUGUI>().text = $"{gameManager.instance.PlayerScript().currentWeapon.CurrentAmmo}";
            if (gameManager.instance.PlayerScript().currentWeapon.weaponName == "Pistol")
            {
                activeReserve.enabled = false;
                pistolReserve.enabled = true;
            }
            else
            {
                activeReserve.enabled = true;
                pistolReserve.enabled = false;
            }
        }
    }
    public void displayWeaponPickUpTrue()
    {
        if (gameManager.instance.PlayerScript().weaponInventory.Count == 3)
        {
        }
    }

    public void displayWeaponPickupFalse()
    {
        swap.SetActive(false);
    }

    public void setGate(bool active, bool keyFound = false)
    {
        gateText.gameObject.SetActive(active);
        if (keyFound)
        {
            gateText.text = "Gate acess: GRANTED\nGate terminal status: UNAUTHORIZED";
        }
        else
        {
            gateText.text = "Gate acess: DENIED\nGate terminal status: UNAUTHORIZED";
        }
        
    }

    public void displayReloadWeaponText()
    {
        //Debug.Log("Press e to reload you weapon");

    }

    public void toggleCursorVisibility()
    {

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void showHUD()
    {
        hud.SetActive(true);
    }

    public void closeHUD()
    {
        hud.SetActive(false);
    }
    public void TransitionFromPlay()
    {
        ResetKeyColors();
        gateText.gameObject.SetActive(false);
        swap.SetActive(false);

    }
    public void clearWeapons()
    {
        if (playerPreferences.instance != null)
        {
            playerPreferences.instance.MainWeapons.Clear();
            playerPreferences.instance.OffWeapon = false;
        }
        updateWeaponDisplay();
    }
    #endregion
    void ResetKeyColors()
    {
        for (int i = 0; i < gameManager.instance.keyCard.Length; i++)
        {
            gameManager.instance.keyCard[i] = false;

        }
        card01.color = new Color(card01.color.r, card01.color.g, card01.color.b, 0.25f);
        keyCard01Text.color = new Color(keyCard01Text.color.r, keyCard01Text.color.g, keyCard01Text.color.b, 0.75f);

        card02.color = new Color(card02.color.r, card02.color.g, card02.color.b, 0.25f);
        keyCard02Text.color = new Color(keyCard02Text.color.r, keyCard02Text.color.g, keyCard02Text.color.b, 0.75f);

        card03.color = new Color(card03.color.r, card03.color.g, card03.color.b, 0.25f);
        keyCard03Text.color = new Color(keyCard03Text.color.r, keyCard03Text.color.g, keyCard03Text.color.b, 0.75f);
    }
    #region Accessors
    public IEnumerator flashDamage(float dmg)
    {
        SetFlashIntensity(dmg).SetActive(true);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(FadeDamageScreen(0.1f));
    }
    public GameObject SetFlashIntensity(float intensity = 0)
    {
        Color damageFlash = damageFlashScreen.GetComponent<Image>().color;
        if (damageFlash.a <= 0)
        {
            damageFlash.a = 1;
        }
        if (intensity != 0)
            damageFlash.a = intensity <= 1 ? 0.5f : 1f;
        return damageFlashScreen;
    }
    public IEnumerator FadeDamageScreen(float timer)
    {
        Color damageFlash = damageFlashScreen.GetComponent<Image>().color;
        while (damageFlash.a >= 0)
        {
            //Debug.Log("Turning damage screen down");
            damageFlash.a -= 0.1f;
            yield return new WaitForSeconds(timer);
        }
        damageFlashScreen.SetActive(false);
    }
    #endregion
}
