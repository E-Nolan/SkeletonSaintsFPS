using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

	public bool BUILDMODE = false;

	[Header("----- Player Information -----")]
	[SerializeField]
	GameObject PlayerPrefab;
	public GameObject PlayerSpawnPos;

	public GameObject playerInstance;

    [Header("Game Components")]
    [SerializeField]
    playerController playerScript;
	[SerializeField]
	cameraControls playerCamera;

    [Header("Game State Variables")]
    public bool isPaused;
	int enemiesRemaining;
	public GameDifficulty currentDifficulty;

	[Header("----- Game Goals -----")]
	[SerializeField] public bool[] keyCard = new bool[3];
	//List<GameEvent> activeGameEvents;

	//Bool to determine when a scene with the player in it has started (I.E. Not in the main menu or level selection.
	//This lets the script know it can start tracking game events like winning or losing.
	[SerializeField]
	bool playStarted;
    #region Runtime Calls
    private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		//Begin the game at start so things can initialize and awake.
		/*beginGame() will start GM functionality and if(BUILDMODE) will expect to have only the persistent scene loaded
		  If not, this will start functionality but with expectation that it has the play level already loaded
		 */
		beginGame();
		
	}
    private void LateUpdate()
    {
        if (playStarted)
        {
			/*manage events and tasks, this needs not check every frame because it doesn't need to be as precise
			  as something like movement or detecting things
			*/
			managePlayerTasks();
        }
		
    }
    #endregion

    #region Public Methods

    public void InitializePlay()
	{
		/* This method is called by either the button in the start menu 
		 * or by beginGame if BUILDMODE == false
		 */
		if (BUILDMODE)
		{
			menuManager.instance.DeactivateAllMenus();
			sceneControl.instance.LoadMainLevel();
		} else
        {
			instance.currentDifficulty = difficultyStorer.instance.currentGameDifficulty;
			FetchEvents();
			LevelSetup();
		}
		//either way call hUDManager to start HUD elements and ensure checks to PlayStarted() return true now.
		hUDManager.instance.showHUD();
		playStarted = true;
	}

    public void FetchEvents()
    {
		gameEventManager.instance.FindEvents();
		//if (gameEventManager.instance.HasEvents())
		//{
		//	for (int i = 0; i < gameEventManager.instance.GameEvents.Count; i++)
  //          {
		//		activeGameEvents.Add(gameEventManager.instance.GameEvents[i]);
		//	}
		//}
	}

    public void LevelSetup()
	{
		//Load player in and assign script components
		playerInstance = Instantiate(PlayerPrefab, PlayerSpawnPos.transform.position, PlayerSpawnPos.transform.rotation);
		
		playerScript = playerInstance.GetComponent<playerController>();
		playerCamera = Camera.main.GetComponent<cameraControls>();

		//assign values from stored preferences
		//AssertplayerPreferencesToScript();

		//if player has weapons saved, then equip the current weapon again
		if (playerScript.weaponInventory.Count > 0)
			playerScript.extSwitchToWeapon(playerScript.weaponInventory.IndexOf(playerScript.currentWeapon.gameObject));

		//if the cursor was up from previous menus or otherwise, lock it again befor the HUD comes up
		if (Cursor.visible)
			hUDManager.instance.toggleCursorVisibility();

		menuManager.instance.CanToggleGameMenu = true;

		gameEventManager.instance.GenerateEventsUI();
	}
	//Commented out until player loads properly to start with.
	/*
    public void AssertplayerPreferencesToScript()
	{
		//Will take the active values from Player Preferences and assign those settings to the variables
		//used in the player and camera scripts
		//Should be called right before the player is dropped in and gains control of the player.
		//Script values should be assigned from preferences, controls should be enabled and cursor hidden
		playerScript.weaponInventory = playerPreferences.instance.SavedWeapons;

		playerScript.HP = playerPreferences.instance.HP;
		playerScript.MoveSpeed = playerPreferences.instance.MoveSpeed;
		playerScript.moveSpeedOrig = playerScript.MoveSpeed;
		playerScript.SprintMod = playerPreferences.instance.SprintMod;
		playerScript.JumpTimes = playerPreferences.instance.JumpTimes;
		playerScript.JumpSpeed = playerPreferences.instance.JumpSpeed;
		playerScript.PlayerGravity = playerPreferences.instance.PlayerGravityStrength;
		playerScript.PlayerForce = playerPreferences.instance.PlayerForceStrength;

		playerScript.ShootRate = playerPreferences.instance.ShootRate;
		playerScript.ShootDist = playerPreferences.instance.ShootDistance;
		playerScript.ShotDamage = playerPreferences.instance.ShotDamage;

		playerCamera.HorizontalSensitivity = playerPreferences.instance.SensitivityHorizontal;
		playerCamera.VeritcalSensitivity = playerPreferences.instance.SensitivityVertical;
		playerCamera.VeticalLockMin = playerPreferences.instance.VerticalLockMin;
		playerCamera.VeticalLockMax = playerPreferences.instance.VerticalLockMax;
		playerCamera.InvertX = playerPreferences.instance.InvertX;

	}
	*/
	public void respawn()
	{
		unPause();
		//if we are respawning because the player died, just un-set the flash screen and toggle the menu
		if (playerScript.GetCurrentStamina() <= 0)
		{
			hUDManager.instance.DamageFlashScreen().SetActive(false);
			toggleGameMenu();
        }
        else
        {
			//Otherwise, if we respawned from the menu it will still be up so close it
			if (menuManager.instance.GameMenuIsUp())
				toggleGameMenu();
		}
		//And in either case, reset the player to the playerSpawnPos when they respawn
		//This can be updated when the player reaches a checkpoint
		playerInstance.transform.position = PlayerSpawnPos.transform.position;
		//AssertplayerPreferencesToScript();

		//if player has weapons saved, then equip the current weapon again
		if (playerScript.weaponInventory.Count > 0)
			playerScript.extSwitchToWeapon(playerScript.weaponInventory.IndexOf(playerScript.currentWeapon.gameObject));

		if (Cursor.visible)
			hUDManager.instance.toggleCursorVisibility();

		menuManager.instance.CanToggleGameMenu = true;
	}
	public void restartLevel()
	{
		unPause();
		clearLevel();
		//Direct call here instead of on ClearLevel be
		playerPreferences.instance.SavedWeapons.Clear();


		if (playerScript.GetCurrentStamina() <= 0)
		{
			hUDManager.instance.DamageFlashScreen().SetActive(false);
			toggleGameMenu();
		}
		//Restart a level without going all the way back to the main menu
		sceneControl.instance.SceneRestart("Level One");
		//reload player and variable settings
		LevelSetup();
	}
	public void restartGame()
	{
		if (menuManager.instance.GameMenuIsUp())
			hUDManager.instance.toggleCursorVisibility();

		clearLevel(true);
		

		//Call to scene control to handle unloading anything we are currently in
		sceneControl.instance.SceneRestart_Game();

		//This call loads the main menu scene and menus
		beginGame();
		//if the menu wasn't up, then the cursor is still locked at this point
		if (!Cursor.visible)
			hUDManager.instance.toggleCursorVisibility();
	}
	public void playerDead()
    {
		loseGame();
    }
	public void loseGame()
	{
		pause();

		menuManager.instance.DisplayLoseMenu();
		hUDManager.instance.toggleCursorVisibility();
	}
	public void winGame()
	{
		pause();
		hUDManager.instance.toggleCursorVisibility();
		menuManager.instance.DisplayWinMenu();
	}
	public void continueGame()
	{
		unPause();
		hUDManager.instance.toggleCursorVisibility();

		menuManager.instance.CloseWinMenu();
	}
	public void toggleGameMenu()
	{
		if (menuManager.instance.GameMenuIsUp())
		{
			menuManager.instance.ClosePauseMenu();
			unPause();
			hUDManager.instance.toggleCursorVisibility();
		}
		else
		{
			hUDManager.instance.toggleCursorVisibility();
			pause();
			menuManager.instance.DisplayPauseMenu();
		}
	}
	public void updateGameGoal (int amt)
    {
		enemiesRemaining += amt;
    }
	public void queuePlayerVictory(float timer)
	{
		StartCoroutine(playerVictoryTimer(timer));
	}

	public IEnumerator playerVictoryTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
		instance.winGame();
	}
    #region Merged Functions
	public void createUIBar()
    {
		hUDManager.instance.createPlayerHealthBar();
		hUDManager.instance.createPlayerStaminaBar();
		hUDManager.instance.createPlayerArmorBar();
	}
    public void updatePlayerHealthBar()
    {
		hUDManager.instance.updatePlayerHealthBar();

	}
	public void updatePlayerStaminaBar()
	{
		hUDManager.instance.updatePlayerStaminaBar();
	}

	public void updatePlayerArmorBar()
	{
		hUDManager.instance.updatePlayerArmorBar();
	}

	public void updateEnemyCounter()
	{
		hUDManager.instance.enemiesCounter.text = $"{enemiesRemaining}";
	}
	public void updateActiveGun()
	{
		hUDManager.instance.updateActiveGun();
	}
	#endregion
	public void setEasyMode()
	{
		currentDifficulty = GameDifficulty.Easy;
		difficultyStorer.instance.currentGameDifficulty = GameDifficulty.Easy;
	}

	public void setNormalMode()
	{
		currentDifficulty = GameDifficulty.Normal;
		difficultyStorer.instance.currentGameDifficulty = GameDifficulty.Normal;
	}

	public void setHardMode()
	{
		currentDifficulty = GameDifficulty.Hard;
		difficultyStorer.instance.currentGameDifficulty = GameDifficulty.Hard;
	}
	#endregion

	#region Private Methods
	void beginGame()
	{
		if (BUILDMODE)
		{
			isPaused = true;
			playStarted = false;

			//Deactivate any menus up from a possible last play
			deactivateUI();

			sceneControl.instance.LoadMainMenuScene();

			menuManager.instance.InitializeMenusText();

			menuManager.instance.DisplayMainMenu();
		}
		else
		{
			InitializePlay();
			isPaused = false;
		}
	}
	private void pause()
    {
		isPaused = true;
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Confined;
	}
	private void unPause()
	{
		isPaused = false;
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void deactivateUI()
    {
		menuManager.instance.DeactivateAllMenus();
		hUDManager.instance.closeHUD();
	}
	private void managePlayerTasks()
	{
		//could probably only check this when an interaction happens or something
		if (gameEventManager.instance.HasEvents())
		{
			//Track by highlighting active quest or event, remove or cross out when done, add new tasks as they appear.
			gameEventManager.instance.UpdateEvents();


			//if (gameEventManager.instance.EventsCompleted == 2)
			//{
			//	WinGame();
			//}
		}
	}

    private void clearLevel(bool restarting = false)
    {
		if (!restarting)
			playerPreferences.instance.SavedWeapons.Clear();

		//gameEventManager.instance.ClearEventListUI();
		//gameEventManager.instance.GameEvents.Clear();
		Destroy(playerInstance);
	}
	#endregion

	public enum GameDifficulty
	{
		Easy, Normal, Hard
	}

	#region Accessors
	public playerController PlayerScript()
	{
		return playerScript;
	}

	public cameraControls CameraControls()
	{
		return playerCamera;
	}
	public bool PlayStarted()
	{
		return playStarted;
	}
	#endregion

}
