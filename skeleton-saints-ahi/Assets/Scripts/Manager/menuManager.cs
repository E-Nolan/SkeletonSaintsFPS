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
	public GameObject credits;

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
		if (instance == null)
			instance = this;
	}

    private void Start()
    {
		ActivateMenu();
		SetSettings();
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
		if (activeMenu != startMenu && activeMenu != controlMenu && activeMenu != settingsMenu && activeMenu != exitGame || activeMenu == null)
        {
			activeMenu = startMenu;
			switch (playerPreferences.instance.difficulty)
			{
				case gameManager.Difficulty.Easy:
					difficulty = easyMode;
					gameManager.instance.currentDifficulty = gameManager.Difficulty.Easy;
					break;
				case gameManager.Difficulty.Normal:
					difficulty = normalMode;
					gameManager.instance.currentDifficulty = gameManager.Difficulty.Normal;
					break;
				case gameManager.Difficulty.Hard:
					difficulty = hardMode;
					gameManager.instance.currentDifficulty = gameManager.Difficulty.Hard;
					break;
				default:
					difficulty = normalMode;
					gameManager.instance.currentDifficulty = gameManager.Difficulty.Normal;
					playerPreferences.instance.difficulty = gameManager.Difficulty.Normal;
					break;
			}
			
		}
		activeMenu.SetActive(true);
		difficulty.SetActive(true);
		sharedMenu.SetActive(true);
	}

	public void DisplayStart()
	{
		if (activeMenu != startMenu)
		{
			activeMenu.SetActive(false);
			activeMenu = startMenu;
			activeMenu.SetActive(true);
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

	public void ExitGame()
	{
		Application.Quit();
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
		hUDManager.instance.closeDialogue();
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
		sceneLoader.instance.RestartLevel();
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

	public void UpdateSettings()
	{
		MAM.SetFloat("masterVolume", Mathf.Log10(playerPreferences.instance.masterVolume) * 20);
		MAM.SetFloat("musicVolume", Mathf.Log10(playerPreferences.instance.musicVolume) * 20);
		MAM.SetFloat("sfxVolume", Mathf.Log10(playerPreferences.instance.sfxVolume) * 20);
		if (gameManager.instance.CameraControls() != null)
			gameManager.instance.CameraControls().Invert_X = playerPreferences.instance.invertX;
	}

	//setting menu
	public void SetSettings()
    {
		masterSlider.value = playerPreferences.instance.masterVolume;
		musicSlider.value = playerPreferences.instance.masterVolume;
		sfxSlider.value = playerPreferences.instance.masterVolume;
		horizontalSlider.value = playerPreferences.instance.masterVolume;
		verticalSlider.value = playerPreferences.instance.masterVolume;
		invertX.isOn = playerPreferences.instance.invertX;

	}

	public void SaveSettingsToPlayer()
	{
		playerPreferences.instance.masterVolume = masterSlider.value;
		playerPreferences.instance.musicVolume = musicSlider.value;
		playerPreferences.instance.sfxVolume = sfxSlider.value;
		playerPreferences.instance.horizontal = horizontalSlider.value;
		playerPreferences.instance.vertical = verticalSlider.value;
		playerPreferences.instance.invertX = invertX.isOn;

		UpdateSettings();
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

		SaveSettingsToPlayer();
	}

	public void UpdateSliderText()
	{
		master.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(masterSlider.value * 100)}";
		music.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(musicSlider.value * 100)}";
		sfx.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(sfxSlider.value * 100)}";
		horizontal.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(horizontalSlider.value)}";
		vertical.GetComponent<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(verticalSlider.value)}";

		SaveSettingsToPlayer();
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
		activeMenu.SetActive(false);
		sharedMenu.SetActive(false);
		startMenu.SetActive(false);
        gameManager.instance.playStarted = true;
	}

	public void MainCredit()
    {
        StartCoroutine(mainCredit());
    }

	public void VictoryCredit()
    {
		StartCoroutine(victoryCredit());
    }

    private IEnumerator mainCredit()
    {
        credits.GetComponent<creditsRoll>().isScrolling = true;
        DeactivateMain();
        credits.SetActive(true);
        GameObject tempSideMenu = GameObject.Find("Game Manager/Menu Manager/Side Menu");
        GameObject tempCinematicManager = GameObject.Find("CinematicManager");
        tempCinematicManager.SetActive(false);
		tempSideMenu.SetActive(false);
        yield return new WaitUntil(() => credits.GetComponent<creditsRoll>().isScrolling == false);
        credits.SetActive(false);
        tempCinematicManager.SetActive(true);
        tempSideMenu.SetActive(true);
        ActivateMenu();
    }

	private IEnumerator victoryCredit()
	{
		credits.GetComponent<creditsRoll>().isScrolling = true;
		victoryMenu.SetActive(false);
		credits.SetActive(true);
		yield return new WaitUntil(() => credits.GetComponent<creditsRoll>().isScrolling == false);
		credits.SetActive(false);
		victoryMenu.SetActive(true);
	}
}