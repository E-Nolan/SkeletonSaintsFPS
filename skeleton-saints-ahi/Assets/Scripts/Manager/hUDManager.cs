using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class hUDManager : MonoBehaviour
{
    public static hUDManager instance;

    [Header("----- Player UI -----")]
    public GameObject playerUISystem;
    public GameObject playerHealthBar;
    public GameObject parentHealth;
    public List<GameObject> playerHealthTick;
    public GameObject playerStaminaBar;
    public GameObject parentStamina;
    public List<GameObject> playerStaminaTick;
    public GameObject playerArmorBar;
    public GameObject parentArmor;
    public List<GameObject> playerArmorTick;

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
        instance = this;
    }
    #region Access Methods

    public GameObject PlayerUISystem
    {
        get { return playerUISystem; }
        set { playerHealthBar = value; }
    }

    #endregion
    #region Public Methods
    public void createPlayerHealthBar()
    {
        maxHealth = gameManager.instance.PlayerScript().GetMaxHealth();
        healthTick = (int)maxHealth / 1;
        for (int i = 0; i < healthTick; i++)
        {
            GameObject tick = Instantiate(playerHealthBar, new Vector3(playerHealthBar.transform.position.x - (i * 24), playerHealthBar.transform.position.y, playerHealthBar.transform.position.z), Quaternion.identity, parentHealth.transform);
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
            GameObject tick = Instantiate(playerStaminaBar, new Vector2(playerStaminaBar.transform.position.x - (i * 24), playerStaminaBar.transform.position.y), Quaternion.identity, parentStamina.transform);
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
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if ((gameManager.instance.PlayerScript().isDashing || gameManager.instance.PlayerScript().isSprinting) && currentStamina > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                int index = (playerStaminaTick.Count - 1) - i;
                if (index >= 0)
                    playerStaminaTick[index].SetActive(false);
            }
        }
        else if ((!gameManager.instance.PlayerScript().isDashing && !gameManager.instance.PlayerScript().isSprinting) || currentStamina == maxStamina)
        {
            if (diff >= 0 && diff < playerStaminaTick.Count)
                playerStaminaTick[(playerStaminaTick.Count - 1) - diff].SetActive(true);

        }
    }

    public void createPlayerArmorBar()
    {
        maxArmor = gameManager.instance.PlayerScript().GetMaxArmor();
        armorTick = (int)maxArmor / 1;
        for (int i = 0; i < armorTick; i++)
        {
            GameObject tick = Instantiate(playerArmorBar, new Vector2(playerArmorBar.transform.position.x - (i * 24), playerArmorBar.transform.position.y), Quaternion.identity, parentArmor.transform);
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
        for (int i = 0; i < currentTick && i < playerArmorTick.Count; i++)
        {
            playerArmorTick[i].SetActive(true);
        }
        for (int i = currentTick; i < playerArmorTick.Count; i++)
        {
            playerArmorTick[i].SetActive(false);
        }
        if (diff >= 0 && (playerArmorTick.Count - 1) - diff < playerArmorTick.Count)
            playerArmorTick[(playerArmorTick.Count - 1) - diff].SetActive(true);
    }

    public void updateWeaponDisplay()
    {
        activeGun.GetComponent<Image>().sprite = gameManager.instance.PlayerScript().currentWeapon.activeImage;
        for (int i = 0; i < gameManager.instance.PlayerScript().inactiveWeapons.Count; i++)
        {
            if (gameManager.instance.PlayerScript().inactiveWeapons[i] != null)
                inactiveGuns[i].GetComponent<Image>().sprite = gameManager.instance.PlayerScript().inactiveWeapons[i].inactiveImage;
        }

        updateWeaponText();
    }

    public void updateWeaponText()
    {
        activeClip.GetComponent<TextMeshProUGUI>().text = $"{gameManager.instance.PlayerScript().currentWeapon.CurrentClip}";
        activeReserve.GetComponent<TextMeshProUGUI>().text = $"{gameManager.instance.PlayerScript().currentWeapon.CurrentAmmo}";
        if(gameManager.instance.PlayerScript().currentWeapon.weaponName == "Pistol")
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
        Debug.Log("Press e to reload you weapon");

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
        playerUISystem.gameObject.SetActive(true);
        weaponUISystem.gameObject.SetActive(true);
        reticle.gameObject.SetActive(true);
        inputReader.displayText.gameObject.SetActive(true);
        EventList.SetActive(true);
    }
    public void closeHUD()
    {
        damageFlashScreen.SetActive(false);
        playerUISystem.gameObject.SetActive(false);
        weaponUISystem.gameObject.SetActive(false);
        reticle.gameObject.SetActive(false);
        inputReader.displayText.gameObject.SetActive(false);
        EventList.SetActive(false);
    }
    #endregion
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
            Debug.Log("Turning damage screen down");
            damageFlash.a -= 0.1f;
            yield return new WaitForSeconds(timer);
        }
        damageFlashScreen.SetActive(false);
    }
    #endregion
}
