using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
	public static menuManager instance;

	[Header("UI Panels")]
    public GameObject MainMenuPanel;
	public GameObject DifficultyMenu;
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
    }
    private void Start()
    {
		//here temorarily till menu and loading is set up
		canToggleGameMenu = true;
    }
	private void LateUpdate()
	{
		if (gameManager.instance.PlayStarted())
			HandleInGameMenuInput();
	}
	#region Run-Time Methods
	void HandleInGameMenuInput()
	{
		//Using the menu manager here to toggle the menus, but in game manager because it affects the timescale and pausestate.
		if (canToggleGameMenu)
			if (Input.GetButtonDown(playerPreferences.instance.Button_Menu))
			{
				gameManager.instance.toggleGameMenu();
			}
	}
	#endregion

	#region Global Menu Methods
	public void DeactivateAllMenus()
    {
		if (MainMenuPanel.activeInHierarchy)
			MainMenuPanel.SetActive(false);
		if (DifficultyMenu.activeInHierarchy)
			DifficultyMenu.SetActive(false);
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
			//Temp object for active menu
			GameObject panelHolder;

			panelHolder = activeMenuPanel;
			activeMenuPanel.gameObject.SetActive(false);

			activeMenuPanel = previousMenuPanel;
			activeMenuPanel.gameObject.SetActive(true);

			previousMenuPanel = panelHolder;
		}
	}

    public void InitializeMenusText()
    {
		//placeholder for initializing menu settings
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
	private void CloseActiveMenuPanel()
	{
		if (activeMenuPanel == null)
			return;
		else
		{
			previousMenuPanel = activeMenuPanel;
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

	public void DisplayDifficultyMenu()
	{
		DisplayMenu(DifficultyMenu);
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

	#endregion

	#region In-Game Menu Methods
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
	#endregion
}
