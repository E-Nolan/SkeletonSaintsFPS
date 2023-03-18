using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class menuManager : MonoBehaviour
{
	public static menuManager instance;

	[Header("----- Panels -----")]
	public GameObject mainMenu;
	public GameObject pauseMenu;
	public GameObject victoryMenu;
	public GameObject defeatMenu;

	[Header("----- Main Menu -----")]
	public GameObject startMenu;
	public GameObject easyMode;
	public GameObject normalMode;
	public GameObject hardMode;
	public GameObject difficulty;
	public GameObject exitGame;

	[Header("----- Pause Menu -----")]
	public GameObject gameMenu;
	public GameObject objectivesMenu;
	public GameObject quitGame;
	public GameObject restartLevel;
	public GameObject restartGame;
	public GameObject respawn;
	public GameObject gameState;

	[Header("----- Setttings Menu -----")]
	public GameObject settingsMenu;
	public TextMeshProUGUI master, music, sfx;
	public Slider masterSlider, musicSlider, sfxSlider;
	public TextMeshProUGUI horizontal, vertical;
	public Slider horizontalSlider, verticalSlider;
	public Toggle invertX;

	[Header("----- Shared -----")]
	public GameObject sharedMenu;
	public GameObject controlMenu;

	[Header("----- Menu Options -----")]
	public GameObject activeMenu;
	public bool changesSaved;
	public bool canToggleGameMenu;

	[Header("----- Audio -----")]
	public AudioSource Audio;
	public AudioClip Click;
	public AudioMixer MAM;

	private void Awake()
	{
		instance = this;
		difficulty = easyMode;
		difficulty.SetActive(true);
		activeMenu = startMenu;
	}

    private void LateUpdate()
    {
		if (gameManager.instance.PlayStarted())
			HandleInGameMenuInput();
    }

    //mainmenu functions
    public void TurnOnMain()
    {
		Debug.Log("running");
		
		mainMenu.SetActive(true);
		startMenu.SetActive(true);
		sharedMenu.SetActive(true);
    }

	public void DisplayStart()
    {
		if(activeMenu != startMenu)
        {
			activeMenu.SetActive(false);
			activeMenu = startMenu;
			activeMenu.SetActive(true);
			difficulty = easyMode;
			difficulty.SetActive(true);
			
        }
    }

	public void EasyMode()
    {
		if(difficulty != easyMode)
        {
			difficulty.SetActive(false);
			difficulty = easyMode;
			difficulty.SetActive(true);
		}
    }

	public void NormalMode()
	{
		if (difficulty != normalMode)
		{
			difficulty.SetActive(false);
			difficulty = normalMode;
			difficulty.SetActive(true);
		}
	}

	public void HardMode()
	{
		if (difficulty != hardMode)
		{
			difficulty.SetActive(false);
			difficulty = hardMode;
			difficulty.SetActive(true);
		}
	}

	public void DisplayExitGame()
	{
		if (activeMenu != exitGame)
		{
			activeMenu.SetActive(false);
			activeMenu = exitGame;
			activeMenu.SetActive(true);
		}
	}

	//pause menu
	void HandleInGameMenuInput()
	{
		if (canToggleGameMenu)
			if (Input.GetButtonDown(playerPreferences.instance.Button_Menu))
			{
				toggleGameMenu();
			}
	}

	public void toggleGameMenu()
	{
		if (!gameManager.instance.isPaused)
		{
			pauseMenu.SetActive(true);
			sharedMenu.SetActive(true);
			activeMenu = gameMenu;
			activeMenu.SetActive(true);
			gameState = restartLevel;
			pause();
		}
		else
		{
			pauseMenu.SetActive(false);
			sharedMenu.SetActive(false);
			activeMenu.SetActive(false);
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

	public void DisplayGame()
	{
		if (activeMenu != gameMenu)
		{
			activeMenu.SetActive(false);
			activeMenu = gameMenu;
			activeMenu.SetActive(true);
		}
	}

	public void Respawn()
	{
		if (gameState != respawn)
		{
			gameState.SetActive(false);
			gameState = respawn;
			gameState.SetActive(true);
		}
	}

	public void ConfirmRespawn()
    {
		Debug.Log("Game should respawn");
	}

	public void RestartLevel()
	{
		if (gameState != restartLevel)
		{
			gameState.SetActive(false);
			gameState = restartLevel;
			gameState.SetActive(true);
		}
	}

	public void ConfirmLevelRestart()
    {
		gameManager.instance.restartLevel();
    }

	public void RestartGame()
	{
		if (gameState != restartGame)
		{
			gameState.SetActive(false);
			gameState = restartGame;
			gameState.SetActive(true);
		}
	}

	public void ConfirmGameRestart()
	{
		gameManager.instance.restartGame();
	}

	public void DisplayObjective()
    {
		if (activeMenu != objectivesMenu)
		{
			activeMenu.SetActive(false);
			activeMenu = objectivesMenu;
			activeMenu.SetActive(true);
		}
	}

	//setting menu
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


	//shared functions
	public void DisplayControls()
    {
		if (activeMenu != controlMenu)
		{
			activeMenu.SetActive(false);
			activeMenu = controlMenu;
			activeMenu.SetActive(true);
		}
	}

	public void DisplaySettings()
	{
		if (activeMenu != settingsMenu)
		{
			activeMenu.SetActive(false);
			activeMenu = settingsMenu;
			activeMenu.SetActive(true);
		}
	}

	public void DisplayGameQuit()
	{
		if (activeMenu != quitGame)
		{
			activeMenu.SetActive(false);
			activeMenu = quitGame;
			activeMenu.SetActive(true);
		}
	}

	public void GameQuit()
    {
		gameManager.instance.GoToMain();

	}

	

	public void DeactivateMain()
    {
		mainMenu.SetActive(false);
		sharedMenu.SetActive(false);
		startMenu.SetActive(false);
		gameManager.instance.playStarted = true;
    }

	/*
	[Header("UI Panels")]
    public GameObject MainMenuPanel;
	public GameObject SettingsMenu;
	public GameObject PauseMenu;
	public GameObject CreditsPanel;
	public GameObject WinMenu;
	public GameObject LoseMenu;

	#region Internal Variables
	private GameObject previousMenuPanel;
	private GameObject activeMenuPanel;
	#endregion;

	[Header("Menu States")]
	//Handles toggle for the in-game menu
	[SerializeField]
	private bool canToggleGameMenu;
	[SerializeField]
	private bool gameMenuIsUp;


	[Header("Settings Panel UI")]
	public TextMeshProUGUI masterVolume_Value;
	public TextMeshProUGUI musicVolume_Value, sfxVolume_Value;

	public TextMeshProUGUI cameraSensitivityHorizontal_Value, cameraSensitivityVertical_Value;
	public Toggle invertX;
	public Slider masterVolume_Slider, musicVolume_Slider, sfxVolume_Slider;
	public Slider cameraSensitivityHorizontal_Slider, cameraSensitivityVertical_Slider;

	bool changesSaved;
	[Header("Audio")]
	public AudioSource MenusAudio;
	public AudioClip ButtonClick;
	public AudioMixer MasterAudioMixer;
	#region Menu Access Methods
	public bool CanToggleGameMenu
	{
		get { return canToggleGameMenu; }
		set { canToggleGameMenu = value; }
	}
	public bool GameMenuIsUp()
	{
		return gameMenuIsUp;
	}
	#endregion


	private void Awake()
    {
		instance = this;
		if (MenusAudio == null)
        {
			MenusAudio = GetComponent<AudioSource>();
			//if (MenusAudio == null)
				//Debug.Log("No menu audio source found. Please ensure there is an AudioSource attached to the menuManager gameobject.");
        }
    }

	#region Run-Time Methods

	#endregion

	#region Global Menu Methods
	public void DeactivateAllMenus()
    {
		if (MainMenuPanel.activeInHierarchy)
			MainMenuPanel.SetActive(false);
		if (SettingsMenu.activeInHierarchy)
			SettingsMenu.SetActive(false);
		if (PauseMenu.activeInHierarchy)
		{
			PauseMenu.SetActive(false);
			gameMenuIsUp = false;
		}
	}
	public void OpenPreviousMenuPanel()
	{
		if (previousMenuPanel == null)
			return;
		else
		{
			if (activeMenuPanel == SettingsMenu && !changesSaved)
            {
				RevertSettings();
            } else if (activeMenuPanel == SettingsMenu && changesSaved)
            {
				SetPlayerPreferencesFromText();
			}
			CloseActiveMenuPanel(false);
			DisplayMenu(previousMenuPanel);
		}
	}

    public void InitializeMenusText()
    {
		SetTextFromPlayerPreferences();
	}
	public void RevertSettings()
    {
		MasterAudioMixer.SetFloat("masterVolume",Mathf.Log10(playerPreferences.instance.masterVolume) * 20);
		MasterAudioMixer.SetFloat("musicVolume", Mathf.Log10(playerPreferences.instance.musicVolume) * 20);
		MasterAudioMixer.SetFloat("sfxVolume", Mathf.Log10(playerPreferences.instance.sfxVolume) * 20);

		gameManager.instance.currentDifficulty = difficultyStorer.instance.GameDifficulty;

		SetTextFromPlayerPreferences();
	}
	public void SetTextFromPlayerPreferences()
	{
		masterVolume_Value.text = playerPreferences.instance.masterVolume.ToString();
		masterVolume_Slider.value = playerPreferences.instance.masterVolume;

		musicVolume_Value.text = playerPreferences.instance.musicVolume.ToString();
		musicVolume_Slider.value = playerPreferences.instance.musicVolume;


		sfxVolume_Value.text = playerPreferences.instance.sfxVolume.ToString();
		sfxVolume_Slider.value = playerPreferences.instance.sfxVolume;


		cameraSensitivityHorizontal_Value.text = playerPreferences.instance.horizontal.ToString();
		cameraSensitivityHorizontal_Slider.value = playerPreferences.instance.horizontal;

		cameraSensitivityVertical_Value.text = playerPreferences.instance.vertical.ToString();
		cameraSensitivityVertical_Slider.value = playerPreferences.instance.vertical;

		invertX.isOn = playerPreferences.instance.invertX;
	}

	public void SetPlayerPreferencesFromText()
	{
		playerPreferences.instance.masterVolume = float.Parse(masterVolume_Value.text);
		playerPreferences.instance.musicVolume = float.Parse(musicVolume_Value.text);
		playerPreferences.instance.sfxVolume = float.Parse(sfxVolume_Value.text);

		playerPreferences.instance.horizontal = float.Parse(cameraSensitivityHorizontal_Value.text);
		playerPreferences.instance.vertical = float.Parse(cameraSensitivityVertical_Value.text);
		
		playerPreferences.instance.invertX = invertX.isOn;

		difficultyStorer.instance.GameDifficulty = gameManager.instance.currentDifficulty;

		changesSaved = true;
	}
	public void DisplayPauseMenu()
	{
		//Close existing menu if up
		CloseActiveMenuPanel();
		DisplayMenu(PauseMenu);
		//Ensure toggle of game menu is enabled
		canToggleGameMenu = true;
		//Enable bool for in-game menu
		gameMenuIsUp = true;
	}
	public void ClosePauseMenu()
	{
		//Only if the game menu is active
		if (gameMenuIsUp)
		{
			CloseActiveMenuPanel();
			//Ensure toggle of game menu is enabled
			canToggleGameMenu = true;
			//Disable bool for in-game menu
			gameMenuIsUp = false;
		}
	}
	public void Invert_X()
	{
		playerPreferences.instance.invertX = invertX.isOn;
	}
	#endregion

	#region Internal Menu Mehods
	private void DisplayMenu(GameObject menuPanel)
	{
		if (activeMenuPanel != null)
		{
			previousMenuPanel = activeMenuPanel;
			CloseActiveMenuPanel();
		}
		activeMenuPanel = menuPanel;
		activeMenuPanel.gameObject.SetActive(true);

	}
	private void CloseActiveMenuPanel(bool trackprevious = true)
	{
		if (activeMenuPanel == null)
			return;
		else
		{
			if (trackprevious)
			{
				previousMenuPanel = activeMenuPanel;
			}


			activeMenuPanel.gameObject.SetActive(false);
			activeMenuPanel = null;
		}
	}
	#endregion

	#region Specific Menu Methods
	public void DisplayMainMenu()
	{
		DisplayMenu(MainMenuPanel);
		//Ensure in-game menu toggling is disabled while main menu is up.
		canToggleGameMenu = false;
	}
	public void DisplayCreditsMenu()
	{
		DisplayMenu(CreditsPanel);
		//Ensure in-game menu toggling is disabled while main menu is up.
		canToggleGameMenu = false;
	}
	public void DisplayLoseMenu()
	{
		gameMenuIsUp = true;
		DisplayMenu(LoseMenu);

		//Ensure in-game menu toggling is disabled while menu is up.
		canToggleGameMenu = false;
	}
	public void DisplayWinMenu()
	{
		gameMenuIsUp = true;
		DisplayMenu(WinMenu);

		//Ensure in-game menu toggling is disabled while menu is up.
		canToggleGameMenu = false;
	}
	public void CloseWinMenu()
	{
		//Function specifically for continuing after the game reaches the end condition rather tha exit to a menu.
		//This can allow for continuation of palying with mechanics or further exploration of the level after it is done.
		gameMenuIsUp = false;
		CloseActiveMenuPanel();

		canToggleGameMenu = true;
	}
	public void DisplaySettingsMenu()
	{
		DisplayMenu(SettingsMenu);
		canToggleGameMenu = false;
		changesSaved = false;
	}
	#endregion
	#region SliderFunctions
	public void UpdateMasterVolumeSlider()
	{
		masterVolume_Value.text = masterVolume_Slider.value.ToString();
		MasterAudioMixer.SetFloat("masterVolume", Mathf.Log10(float.Parse(masterVolume_Value.text)) * 20);
	}
	public void UpdateMusicSlider()
	{
		musicVolume_Value.text = musicVolume_Slider.value.ToString();
		MasterAudioMixer.SetFloat("musicVolume", Mathf.Log10(float.Parse(musicVolume_Value.text)) * 20);
	}
	public void UpdateSFXSlider()
	{
		sfxVolume_Value.text = sfxVolume_Slider.value.ToString();
		MasterAudioMixer.SetFloat("sfxVolume", Mathf.Log10(float.Parse(sfxVolume_Value.text)) * 20);
	}
	public void UpdateHorizontalSlider()
	{
		cameraSensitivityHorizontal_Value.text = cameraSensitivityHorizontal_Slider.value.ToString("F0");
	}
	public void UpdateVerticalSlider()
	{
		cameraSensitivityVertical_Value.text = cameraSensitivityVertical_Slider.value.ToString("F0");
	}
	#endregion
	*/
}
