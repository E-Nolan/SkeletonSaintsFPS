using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class pauseMenuManager : MonoBehaviour
{
    public static pauseMenuManager instance;
    public GameObject pauseMenu;

    [Header("----- Game Panels -----")]
    public GameObject gameMenu;
    public List<GameObject> gameState;
    public GameObject currentState;

    [Header("----- Controls Panels -----")]
    public GameObject controlMenu;

    [Header("----- Settings Panels -----")]
    public GameObject settingsMenu;
    public TextMeshProUGUI master, music, sfx;
    public Slider masterSlider, musicSlider, sfxSlider;
    public TextMeshProUGUI horizontal, vertical;
    public Slider horizontalSlider, verticalSlider;
    public Toggle invertX;
    public List<GameObject> difficulty;
    public GameObject currentDifficulty;
    bool changesSaved;

    [Header("----- Objectives Panels -----")]
    public GameObject objectivesMenu;

    [Header("----- Quit Panels -----")]
    public GameObject quitMenu;

    [Header("----- Menu Options -----")]
    public GameObject activeMenu;
    public bool canToggleGameMenu;

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
        gameMenu.SetActive(true);
        activeMenu = gameMenu;
        currentState = gameState[0];
        currentState.SetActive(true);
    }

    private void LateUpdate()
    {
        if (gameManager.instance.PlayStarted())
            HandleInGameMenuInput();
    }

    void HandleInGameMenuInput()
    {
        //Using the menu manager here to toggle the menus, but in game manager because it affects the timescale and pausestate.
        if (canToggleGameMenu)
            if (Input.GetButtonDown(playerPreferences.instance.Button_Menu))
            {
                toggleGameMenu();
            }
    }

    public void toggleGameMenu()
    {
        if(!gameManager.instance.isPaused)
        {
            pauseMenu.SetActive(true);
            pause();
        }
        else
        {
            pauseMenu.SetActive(false);
            unPause();
        }
    }

    public void pause()
    {
        hUDManager.instance.closeHUD();
        gameManager.instance.isPaused = true;
        Time.timeScale = 0f;
        hUDManager.instance.toggleCursorVisibility();
    }

    public void unPause()
    {
        hUDManager.instance.showHUD();
        gameManager.instance.isPaused = false;
        Time.timeScale = 1f;
        hUDManager.instance.toggleCursorVisibility();
    }

    public void Resume()
    {
        unPause();
    }

    //game menu
    public void DisplayGame()
    {
        SavedChanges();
        if(activeMenu != gameMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = gameMenu;
            gameMenu.SetActive(true);
            currentState = gameState[0];
            currentState.SetActive(true);
        }
    }

    public void Respawn()
    {
        if(currentState != gameState[1])
        {
            currentState.SetActive(false);
            currentState = gameState[1];
            gameState[1].SetActive(true);
        }
    }

    public void Restart()
    {
        if (currentState != gameState[0])
        {
            currentState.SetActive(false);
            currentState = gameState[0];
            gameState[0].SetActive(true);
        }
    }

    //control menu
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

    //setting menu
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
        if (activeMenu == settingsMenu)
        {
            if (changesSaved != true)
            {
                ResetDefault();
            }
        }
    }

    //objectives menu
    public void DisplayObjectives()
    {
        SavedChanges();
        if (activeMenu != objectivesMenu)
        {
            activeMenu.SetActive(false);
            activeMenu = objectivesMenu;
            objectivesMenu.SetActive(true);
            changesSaved = false;
        }
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
}
