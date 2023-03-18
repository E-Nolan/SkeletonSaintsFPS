using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class mainMenuManager : MonoBehaviour
{
    public static mainMenuManager instance;
    public GameObject mainMenu;

    [Header("----- Start Panels -----")]
    public GameObject startMenu;
    public List<GameObject> difficulty;
    public GameObject currentDifficulty;

    [Header("----- Controls Panels -----")]
    public GameObject controlMenu;

    [Header("----- Settings Panels -----")]
    public GameObject settingsMenu;
    public TextMeshProUGUI master, music, sfx;
    public Slider masterSlider, musicSlider, sfxSlider;
    public TextMeshProUGUI horizontal, vertical;
    public Slider horizontalSlider, verticalSlider;
    public Toggle invertX;
    bool changesSaved;

    [Header("----- Credits Panels -----")]
    public GameObject creditsMenu;
    public List<GameObject> credits;
    public GameObject currentCredits;
    public int currentPage;
    public GameObject nextPage;
    public GameObject previousPage;

    [Header("----- Quit Panels -----")]
    public GameObject quitMenu;

    [Header("----- Menu Options -----")]
    public GameObject activeMenu;

    [Header("----- Audio -----")]
    public AudioSource Audio;
    public AudioClip Click;
    public AudioMixer MAM;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainMenu.SetActive(true);
        startMenu.SetActive(true);
        activeMenu = startMenu;
        currentPage = 0;
        difficulty[0].SetActive(true);
        currentDifficulty = difficulty[0];
        gameManager.instance.currentDifficulty = gameManager.Difficulty.Normal;
    }

    //Start menu
    public void DisplayStart()
    {
        SavedChanges();
        if (activeMenu != startMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = startMenu;
            startMenu.SetActive(true);
        }
    }

    public void easyMode()
    {
        
        if(currentDifficulty != difficulty[0])
        {
            currentDifficulty.SetActive(false);
            currentDifficulty = difficulty[0];
            difficulty[0].SetActive(true);
            gameManager.instance.currentDifficulty = gameManager.Difficulty.Easy;
        }
    }

    public void normalMode()
    {
        
        if (currentDifficulty != difficulty[1])
        {
            currentDifficulty.SetActive(false);
            currentDifficulty = difficulty[1];
            difficulty[1].SetActive(true);
            gameManager.instance.currentDifficulty = gameManager.Difficulty.Normal;
        }
    }

    public void playGame()
    {
        gameManager.instance.InitializePlay();
    }

    public void hardMode()
    {
        
        if (currentDifficulty != difficulty[2])
        {
            currentDifficulty.SetActive(false);
            currentDifficulty = difficulty[2];
            difficulty[2].SetActive(true);
            gameManager.instance.currentDifficulty = gameManager.Difficulty.Hard;
        }
    }

    public void PlayGame()
    {

    }

    //Control menu
    public void DisplayControl()
    {
        SavedChanges();
        if (activeMenu != controlMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = controlMenu;
            controlMenu.SetActive(true);
        }
    }


    //settings menu
    public void DisplaySettings()
    {
        if (activeMenu != settingsMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = settingsMenu;
            settingsMenu.SetActive(true);
            changesSaved = false;
        }
    }

    public void SceanicPreset()
    {

    }

    public void ActionPreset()
    {

    }

    //assigning to player preferences
    public void SaveSettingsToPlayer()
    {
        playerPreferences.instance.masterVolume = masterSlider.value;
        playerPreferences.instance.musicVolume = musicSlider.value;
        playerPreferences.instance.sfxVolume = sfxSlider.value;
        playerPreferences.instance.horizontal = horizontalSlider.value;
        playerPreferences.instance.vertical = verticalSlider.value;
        playerPreferences.instance.invertX = invertX.isOn;

        changesSaved = true;
    }

    public void ResetDefault()
    {
        playerPreferences.instance.masterVolume = playerPreferences.instance.masterVolumeDefault;
        master.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(playerPreferences.instance.masterVolume * 100)}";
        masterSlider.value = playerPreferences.instance.masterVolumeDefault;
        playerPreferences.instance.musicVolume = playerPreferences.instance.musicVolumeDefault;
        music.text = $"{Mathf.FloorToInt(playerPreferences.instance.musicVolume * 100)}";
        musicSlider.value = playerPreferences.instance.musicVolumeDefault;
        playerPreferences.instance.sfxVolume = playerPreferences.instance.sfxVolumeDefault;
        sfx.text = $"{Mathf.FloorToInt(playerPreferences.instance.sfxVolume * 100)}";
        sfxSlider.value = playerPreferences.instance.sfxVolumeDefault;

        playerPreferences.instance.horizontal = playerPreferences.instance.horizontalDefault;
        horizontal.text = $"{(playerPreferences.instance.horizontal)}";
        horizontalSlider.value = playerPreferences.instance.horizontalDefault;
        playerPreferences.instance.vertical = playerPreferences.instance.verticalDefault;
        vertical.text = $"{(playerPreferences.instance.vertical)}";
        verticalSlider.value = playerPreferences.instance.verticalDefault;
        playerPreferences.instance.invertX = playerPreferences.instance.invertXDefault;
        invertX.isOn = playerPreferences.instance.invertX;
    }

    public void UpdateSliderText()
    {
        master.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(masterSlider.value * 100)}";
        music.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(musicSlider.value * 100)}";
        sfx.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(sfxSlider.value * 100)}";
        horizontal.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(horizontalSlider.value)}";
        vertical.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(verticalSlider.value)}";
    }

    public void SavedChanges()
    {
        if(activeMenu == settingsMenu)
        {
            if (changesSaved != true)
            {
                ResetDefault();
            }
        }  
    }

    //credits menu
    public void DisplayCredits()
    {
        SavedChanges();
        if (activeMenu != creditsMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = creditsMenu;
            creditsMenu.SetActive(true);
            currentCredits = credits[currentPage];
            credits[currentPage].SetActive(true);
        }
    }

    public void NextPage()
    {
        credits[currentPage].SetActive(false);
        currentPage++;
        currentCredits = credits[currentPage];
        credits[currentPage].SetActive(true);
    }

    public void PreviousPage()
    {
        credits[currentPage].SetActive(false);
        currentPage--;
        currentCredits = credits[currentPage];
        credits[currentPage].SetActive(true);
    }

   //quit menu
    public void DisplayQuit()
    {
        SavedChanges();
        if (activeMenu != quitMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = quitMenu;
            quitMenu.SetActive(true);
        }
    }

    public void DeactivateMainMenu()
    {
        mainMenu.SetActive(false);
    }
}
