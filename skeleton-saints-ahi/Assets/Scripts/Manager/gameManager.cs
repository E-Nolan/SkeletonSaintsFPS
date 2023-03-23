using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class gameManager : MonoBehaviour
{
	public static gameManager instance;

	public bool Game = false;
	public bool Testing = false;

	[Header("----- Player Information -----")]
	[SerializeField] GameObject PlayerPrefab;
	public GameObject PlayerSpawnPos;
	public GameObject playerInstance;

	[Header("Game Components")]
	[SerializeField] playerController playerScript;
	[SerializeField] cameraControls playerCamera;

	[Header("----- Audio -----")]
	public AudioMixer masterMixer;
	[Header("Game State Variables")]
	public bool isPaused;
	bool isEnemyFiring;
	int enemiesRemaining;
	public Difficulty currentDifficulty;
	public GameObject currentScene;

	[Header("----- Game Goals -----")]
	[SerializeField] public bool[] keyCard = new bool[3];
	List<gameEvent> activeGameEvents;
	[SerializeField] gateButton finalGateButton;

	public string activeScene;
	//Bool to determine when a scene with the player in it has started (I.E. Not in the main menu or level selection.
	//This lets the script know it can start tracking game events like winning or losing.
	public bool playStarted;
	public bool confirmed;
	#region Runtime Calls
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
		activeGameEvents = new List<gameEvent>();
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
		if (Game)
		{
			//menuManager.instance.DeactivateAllMenus
			menuManager.instance.DeactivateMain();
			sceneLoader.instance.LoadNextScene();
			isPaused = false;
		}

		if (Testing)
		{
			menuManager.instance.DeactivateMain();
			LevelSetup();
		}

		//either way call hUDManager to start HUD elements and ensure checks to PlayStarted() return true now.
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
		Debug.Log("Setting up level: Spawning New Player");
		PlayerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn");
		if (PlayerSpawnPos == null)
		{
			//Debug.Log ("Player Spawn not found on level setup");
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
		{/*
			if (playerScript.weaponInventory.Count > 0)
				playerScript.extSwitchToWeapon(playerScript.weaponInventory.IndexOf(playerScript.currentWeapon.gameObject));
			*/
		}
		//if the cursor was up from previous menus or otherwise, lock it again befor the HUD comes up
		if (Cursor.visible)
			hUDManager.instance.toggleCursorVisibility();

		hUDManager.instance.showHUD();
		menuManager.instance.canToggleGameMenu = true;
		FetchEvents();
	}
	//Commented out until player loads properly to start with.

	public void AssertplayerPreferencesToScript()
	{
		//Will take the active values from Player Preferences and assign those settings to the variables
		//used in the player and camera scripts
		//Should be called right before the player is dropped in and gains control of the player.
		//Script values should be assigned from preferences, controls should be enabled and cursor hidden

		if (playerScript.startingWeapon)
			playerScript.rangedWeaponPickup(playerScript.startingWeapon, playerScript.startingWeapon.weaponType);

		for (int i = 0; i < playerPreferences.instance.MainWeapons.Count; i++)
			playerScript.CopyWeaponFromPlayerPreferences(playerPreferences.instance.MainWeapons[i]);

		if (playerScript.startingOffHand)
			playerScript.rangedWeaponPickup(playerScript.startingOffHand, playerScript.startingOffHand.weaponType);

		playerScript.PlayerSpeed = playerPreferences.instance.playerSpeed;
		playerScript.SetGravity = playerPreferences.instance.gravity;

		playerScript.MaxStamina = playerPreferences.instance.maxStamina;
		playerScript.SetStaminaRegenSpeed = playerPreferences.instance.staminaRegenSpeed;
		playerScript.SetStaminaRegenCooldown = playerPreferences.instance.staminaRegenCooldown;

		playerScript.MaxHealth = playerPreferences.instance.maxHealth;

		playerScript.MaxArmor = playerPreferences.instance.maxArmor;
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
		if (playerScript.GetCurrentHealth <= 0)
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

		//menuManager.instance.CanToggleGameMenu = true;
	}

	public void restartLevel()
	{
		clearLevel();
		if (isPaused)
		{
			menuManager.instance.toggleGameMenu();
		}

		if (playerScript.GetCurrentHealth <= 0)
		{
			hUDManager.instance.damageFlashScreen.SetActive(false);
		}
		//Restart a level without going all the way back to the main menu
		sceneControl.instance.SceneRestart();
		//reload player and variable settings
	}

	public void restartGame()
	{
		clearLevel();
		if (isPaused)
		{
			menuManager.instance.toggleGameMenu();
		}

		if (playerScript.GetCurrentHealth <= 0)
		{
			hUDManager.instance.damageFlashScreen.SetActive(false);
		}
		//Restart a level without going all the way back to the main menu
		sceneControl.instance.LoadMainLevel();
		//reload player and variable settings
	}

	public void GoToMain()
	{
		/*
		if (menuManager.instance.GameMenuIsUp())
			hUDManager.instance.toggleCursorVisibility();
		*/
		clearLevel(true);

		//Call to scene control to handle unloading anything we are currently in
		sceneControl.instance.SceneRestart_Game();

		//This call loads the main menu scene and menu

		beginGame();
		//if the menu wasn't up, then the cursor is still locked at this point
	}
	public void playerDead()
	{
		loseGame();
	}

	public void loseGame()
	{
		menuManager.instance.canToggleGameMenu = false;
		menuManager.instance.pause();
		menuManager.instance.activeMenu = menuManager.instance.defeatMenu;
		menuManager.instance.activeMenu.SetActive(true);
	}

	public void winGame()
	{
		menuManager.instance.pause();
		menuManager.instance.canToggleGameMenu = false;
		menuManager.instance.activeMenu = menuManager.instance.victoryMenu;
		menuManager.instance.activeMenu.SetActive(true);
	}

	public void updateGameGoal(int amt)
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

	#endregion
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
	public void beginGame()
	{
		if (Game)
		{
			isPaused = true;
			playStarted = false;

			//Deactivate any menus up from a possible last play
			hUDManager.instance.closeHUD();

			sceneControl.instance.LoadMainMenuScene();
			menuManager.instance.ActivateMenu();

		}
		else
		{
			InitializePlay();
		}

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

	public void clearLevel(bool respawning = false)
	{
		if (!respawning)
		{
			//playerPreferences.instance.MainWeapons.Clear();
			gameEventManager.instance.ClearEventListUI();
			gameEventManager.instance.ResetEvents();
			gameEventManager.instance.gameEvents.Clear();
		}
		else
		{

		}

		Destroy(playerInstance);
	}
	#endregion

	public enum Difficulty
	{
		Easy, Normal, Hard
	}
	public enum EventClass
	{
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
	public cameraControls SetCameraControls
	{
		set
		{
			playerCamera = value;
		}
	}
	public playerController SetPlayerController
	{
		set
		{
			playerScript = value;
		}
	}
	public bool PlayStarted()
	{
		return playStarted;
	}
	#endregion
}