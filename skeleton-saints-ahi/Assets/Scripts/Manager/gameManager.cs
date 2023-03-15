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
	public Difficulty currentDifficulty;
	public GameObject currentScene;

	[Header("----- Game Goals -----")]
	[SerializeField] public bool[] keyCard;
	List<gameEvent> activeGameEvents;
	[SerializeField]
	gateButton finalGateButton;

	//Bool to determine when a scene with the player in it has started (I.E. Not in the main menu or level selection.
	//This lets the script know it can start tracking game events like winning or losing.
	[SerializeField]
	bool playStarted;
    #region Runtime Calls
    private void Awake()
	{
		instance = this;
		activeGameEvents = new List<gameEvent>();
		keyCard = new bool[3];
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
			//menuManager.instance.DeactivateAllMenus
			mainMenuManager.instance.DeactivateMainMenu();
			sceneControl.instance.LoadMainLevel();
		}
		FetchEvents();
		if (QUICKPLAYMANAGMENT)
		{
			mainMenuManager.instance.DeactivateMainMenu();
			LevelSetup();
		}
            //either way call hUDManager to start HUD elements and ensure checks to PlayStarted() return true now.
            hUDManager.instance.showHUD();
		
		playStarted = true;
		isPaused = false;
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
		if (sceneControl.instance.CurrentScene().name == "Level One")
			finalGateButton = GameObject.FindGameObjectWithTag("FinalGateButton").GetComponent<gateButton>();
		//Load player in and assign script components
		playerInstance = Instantiate(PlayerPrefab, PlayerSpawnPos.transform.position, PlayerSpawnPos.transform.rotation);
		
		playerScript = playerInstance.GetComponent<playerController>();
		playerCamera = Camera.main.GetComponent<cameraControls>();

		//assign values from stored preferences
		AssertplayerPreferencesToScript();

		//if player has weapons saved, then equip the current weapon again
		if (playerScript.weaponInventory != null)
		{
			if (playerScript.weaponInventory.Count > 0)
				playerScript.extSwitchToWeapon(playerScript.weaponInventory.IndexOf(playerScript.currentWeapon.gameObject));
		}
		//if the cursor was up from previous menus or otherwise, lock it again befor the HUD comes up
		if (Cursor.visible)
			hUDManager.instance.toggleCursorVisibility();

		pauseMenuManager.instance.canToggleGameMenu = true;
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
	
		//if we are respawning because the player died, just un-set the flash screen and toggle the menu
		if (playerScript.GetCurrentHealth() <= 0)
		{
			hUDManager.instance.damageFlashScreen.SetActive(false);
	
        }
        else
        {
			//Otherwise, if we respawned from the menu it will still be up so close i
			
		}
		//And in either case, reset the player to the playerSpawnPos when they respawn
		//This can be updated when the player reaches a checkpoint

		playerInstance.transform.position = PlayerSpawnPos.transform.position;
		AssertplayerPreferencesToScript();

		//if player has weapons saved, then equip the current weapon again
		if (playerScript.weaponInventory.Count > 0)
			playerScript.extSwitchToWeapon(playerScript.weaponInventory.IndexOf(playerScript.currentWeapon.gameObject));

		if (Cursor.visible)
			hUDManager.instance.toggleCursorVisibility();

		menuManager.instance.CanToggleGameMenu = true;
	}
	public void restartLevel()
	{
		clearLevel();

		
		if (isPaused)
		{
			pauseMenuManager.instance.toggleGameMenu();
		}
		
		if (playerScript.GetCurrentHealth() <= 0)
		{
			hUDManager.instance.damageFlashScreen.SetActive(false);
		}
		//Restart a level without going all the way back to the main menu
		sceneControl.instance.SceneRestart();
		//reload player and variable settings

	}
	public void restartGame()
	{
		if (menuManager.instance.GameMenuIsUp())
			hUDManager.instance.toggleCursorVisibility();

		clearLevel(true);
		

		//Call to scene control to handle unloading anything we are currently in
		sceneControl.instance.SceneRestart_Game();

		//This call loads the main menu scene and menu
		
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
	
		menuManager.instance.DisplayLoseMenu();
	}
	public void winGame()
	{

		hUDManager.instance.toggleCursorVisibility();
		menuManager.instance.DisplayWinMenu();
	}
	public void continueGame()
	{
	
		hUDManager.instance.toggleCursorVisibility();

		menuManager.instance.CloseWinMenu();
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
		mainMenuManager.instance.easyMode();
	}

	public void setNormalMode()
	{
		mainMenuManager.instance.normalMode();
	}

	public void setHardMode()
	{
		mainMenuManager.instance.hardMode();
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
			//menuManager.instance.DisplayMainMenu();

		}
		else
		{
			InitializePlay();
		}

	}
	

	private void deactivateUI()
    {
		//menuManager.instance.DeactivateAllMenus();
		hUDManager.instance.closeHUD();
	}
	private void managePlayerTasks()
	{
		if (allKeysFound())
			finalGateButton.CanInteractYet = true;
		//could probably only check this when an interaction happens or something
		if (gameEventManager.instance.HasEvents())
		{
			//Track by highlighting active quest or event, remove or cross out when done, add new tasks as they appear.
			gameEventManager.instance.UpdateEvents();
			gameEventManager.instance.EventListComplete();
		}
	}
	private bool allKeysFound()
    {
		int keysFound = 0;
		if (keyCard[0])
			keysFound++;
		if (keyCard[1])
			keysFound++;
		if (keyCard[2])
			keysFound++;
		if (keysFound == 3)
		{
			return true;
		}
		else 
			return false;
	}
    private void clearLevel(bool respawning = false)
    {
		if (!respawning)
		{
			playerPreferences.instance.SavedWeapons.Clear();
			gameEventManager.instance.ClearEventListUI();
			gameEventManager.instance.ResetEvents();
			gameEventManager.instance.gameEvents.Clear();
		} else
        {

        }

		Destroy(playerInstance);
	}
	#endregion

	public enum Difficulty
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
