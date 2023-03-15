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
			if (MenusAudio == null)
				Debug.Log("No menu audio source found. Please ensure there is an AudioSource attached to the menuManager gameobject.");
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
}
