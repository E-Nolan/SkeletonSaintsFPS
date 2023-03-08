using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

	public bool BUILDMODE = false;
	public bool QUICKPLAYMANAGMENT = false;

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
    bool isEnemyFiring;
	int enemiesRemaining;
	public GameDifficulty currentDifficulty;

	[Header("----- Game Goals -----")]
	[SerializeField] public bool[] keyCard = new bool[3];
	List<gameEvent> activeGameEvents;

	//Bool to determine when a scene with the player in it has started (I.E. Not in the main menu or level selection.
	//This lets the script know it can start tracking game events like winning or losing.
	[SerializeField]
	bool playStarted;
    #region Runtime Calls
    private void Awake()
	{
		instance = this;
		activeGameEvents = new List<gameEvent>();
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
		}
		FetchEvents();
		if (QUICKPLAYMANAGMENT)
			LevelSetup();
		//either way call hUDManager to start HUD elements and ensure checks to PlayStarted() return true now.
		hUDManager.instance.showHUD();
		
		playStarted = true;
	}

    public void FetchEvents()
    {
		gameEventManager.instance.FindEvents();
        if (gameEventManager.instance.HasEvents())
        {
            for (int i = 0; i < gameEventManager.instance.gameEvents.Count; i++)
            {
                activeGameEvents.Add(gameEventManager.instance.gameEvents[i]);
            }
        }
    }

    public void LevelSetup()
	{
		PlayerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn");
		if (PlayerSpawnPos == null)
        {
			Debug.Log ("Player Spawn not found on level setup");
        }

		//Load player in and assign script components
		playerInstance = Instantiate(PlayerPrefab, PlayerSpawnPos.transform.position, PlayerSpawnPos.transform.rotation);
		
		playerScript = playerInstance.GetComponent<playerController>();
		playerCamera = Camera.main.GetComponent<cameraControls>();

		//assign values from stored preferences
		AssertplayerPreferencesToScript();

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

    public void AssertplayerPreferencesToScript()
    {
        //Will take the active values from Player Preferences and assign those settings to the variables
        //used in the player and camera scripts
        //Should be called right before the player is dropped in and gains control of the player.
        //Script values should be assigned from preferences, controls should be enabled and cursor hidden
        
		playerScript.weaponInventory = playerPreferences.instance.SavedWeapons;

		playerScript.SetPlayerSpeed = playerPreferences.instance.playerSpeed;
		playerScript.SetGravity = playerPreferences.instance.gravity;

		playerScript.SetMaxStamina = playerPreferences.instance.maxStamina;
		playerScript.SetStaminaRegenSpeed = playerPreferences.instance.staminaRegenSpeed;
		playerScript.SetStaminaRegenCooldown = playerPreferences.instance.staminaRegenCooldown;

		playerScript.SetMaxHealth = playerPreferences.instance.maxHealth;

		playerScript.SetMaxArmor = playerPreferences.instance.maxArmor;
		playerScript.SetArmorRegenSpeed = playerPreferences.instance.armorRegenSpeed;
		playerScript.SetArmorRegenCooldown = playerPreferences.instance.armorRegenCooldown;

		playerScript.SetInvincibilityCooldown = playerPreferences.instance.invincibilityCooldown;

		playerScript.SetMaxJumpVel = playerPreferences.instance.maxJumpVel;
		playerScript.SetJumpAcceleration = playerPreferences.instance.jumpAcceleration;
		playerScript.SetMaxJumps = playerPreferences.instance.maxJumps;
		playerScript.SetJumpStaminaCost = playerPreferences.instance.jumpStaminaCost;
		playerScript.SetCoyoteTime = playerPreferences.instance.coyoteTime;
		playerScript.SetJumpInputCooldown = playerPreferences.instance.jumpInputCooldown;

		playerScript.SetDashSpeed = playerPreferences.instance.dashSpeed;
		playerScript.SetDashCooldown = playerPreferences.instance.dashCooldown;
		playerScript.SetDashDuration = playerPreferences.instance.dashDuration;
		playerScript.SetDashStaminaCost = playerPreferences.instance.dashStaminaCost;
		playerScript.SetDashInvincibilityTime = playerPreferences.instance.dashInvincibilityTime;

		playerScript.SetSprintSpeed = playerPreferences.instance.sprintSpeed;
		playerScript.SetSprintStaminaDrain = playerPreferences.instance.sprintStaminaDrain;
	}

    public void respawn()
	{
		unPause();
		//if we are respawning because the player died, just un-set the flash screen and toggle the menu
		if (playerScript.GetCurrentHealth() <= 0)
		{
			hUDManager.instance.damageFlashScreen.SetActive(false);
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
		if (playerScript.GetCurrentHealth() <= 0)
		{
			hUDManager.instance.damageFlashScreen.SetActive(false);
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
		killCondition killCon;
		if (gameEventManager.instance.HasKillCondition(out killCon))
        {
			if (amt < 0)
			{
				killCon.enemiesLeft += amt;
				killCon.CheckCompletion();
			}
        }
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

	public void updateEnemyCounter()
	{
		hUDManager.instance.enemiesCounter.text = $"{enemiesRemaining}";
	}
	#endregion
	public void setEasyMode()
	{
		currentDifficulty = GameDifficulty.Easy;
	}

	public void setNormalMode()
	{
		currentDifficulty = GameDifficulty.Normal;
	}

	public void setHardMode()
	{
		currentDifficulty = GameDifficulty.Hard;
	}

    public void setEnemyFiring(bool isFiring)
    {
        isEnemyFiring = isFiring;
    }

    public bool getEnemyFiring()
    {
        return isEnemyFiring;
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
		hUDManager.instance.closeHUD();
		isPaused = true;
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Confined;
	}
	private void unPause()
	{
		hUDManager.instance.showHUD();
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
			gameEventManager.instance.EventListComplete();
		}
	}

    private void clearLevel(bool restartingGame = false)
    {
		if (restartingGame)
			playerPreferences.instance.SavedWeapons.Clear();

		gameEventManager.instance.ClearEventListUI();
		gameEventManager.instance.ResetEvents();
		gameEventManager.instance.gameEvents.Clear();
		Destroy(playerInstance);
	}
	#endregion

	public enum GameDifficulty
	{
		Easy, Normal, Hard
	}
	public enum EventClass {
		Location, Interaction, Collection, Kill, Boss
	};

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
