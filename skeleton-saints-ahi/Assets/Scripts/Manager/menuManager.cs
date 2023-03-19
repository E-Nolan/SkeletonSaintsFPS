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
		if(instance == null)
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
	public void ActivateMenu()
	{
		mainMenu.SetActive(true);
		startMenu.SetActive(true);
		sharedMenu.SetActive(true);
	}

	public void DisplayStart()
	{
		if (activeMenu != startMenu)
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
		if (difficulty != easyMode)
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
		toggleGameMenu();
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
		gameManager.instance.clearLevel();
		sceneLoader.instance.RestartLevel();
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
		gameManager.instance.clearLevel();
		sceneLoader.instance.RestartGame();
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
		sceneLoader.instance.LoadMainMenu();
		hUDManager.instance.toggleCursorVisibility();
	}

	public void DeactivateMain()
	{
		mainMenu.SetActive(false);
		sharedMenu.SetActive(false);
		startMenu.SetActive(false);
		gameManager.instance.playStarted = true;
	}

	//defeat menu

}