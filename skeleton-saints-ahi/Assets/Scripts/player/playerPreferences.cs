using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class playerPreferences: MonoBehaviour
{
	public static playerPreferences instance;

    public void Awake()
    {
		instance = this;
		GetDefaultsFromFile();
    }
	public List<GameObject> MainWeapons = new List<GameObject>();
	public bool OffWeapon;
	public gameManager.Difficulty difficulty = gameManager.Difficulty.Normal;

	[Header("----- Stats -----")]
	[Range(5, 30)] public int playerSpeed = 10;
	[Range(0, 50)] public int gravity = 24;

	[Header("----- Stamina -----")]
	[Range(10, 500)] public int maxStamina = 100;
	[Range(0.0f, 100.0f)] public float staminaRegenSpeed = 50;
	[Tooltip("This is how long (in seconds) the player will have to stop using stamina in order for their stamina to regenerate")]
	[Range(0.0f, 5.0f)] public float staminaRegenCooldown = .5f;

	[Header("----- Health and Armor -----")]
	[Range(1, 100)] public int maxHealth = 10;
	[Range(0, 5)] public int maxArmor = 3;
	[Range(0.0f, 10.0f)] public float armorRegenSpeed = 1;
	[Range(0.0f, 5.0f)] public float armorRegenCooldown = 1;
	[Range(0.0f, 2.0f)] public float invincibilityCooldown = .25f;

	[Header("----- Jump -----")]
	[Range(1.0f, 10.0f)] public float maxJumpVel = 9;
	[Range(0.05f, 1000.0f)] public float jumpAcceleration = 45;
	[Range(0, 3)] public int maxJumps = 1;
	[Range(0, 100)] public int jumpStaminaCost = 1;
	[Range(0.0f, 1.0f)] public float coyoteTime = .2f;
	[Range(0.0f, 1.0f)] public float jumpInputCooldown = 0;

	// Stats for player dashing
	[Header("----- Dash -----")]
	[Range(15, 100)] public int dashSpeed = 25;
	[Tooltip("This should be longer or equal to Dash Duration")]
	[Range(0.0f, 20.0f)] public float dashCooldown = .75f;
	[Tooltip("This should be shorter or equal to Dash Cooldown")]
	[Range(0.0f, 5.0f)] public float dashDuration = .25f;
	[Range(0, 100)] public int dashStaminaCost = 30;
	[Range(0.0f, 1.0f)] public float dashInvincibilityTime = .05f;

	[Header("----- Sprint -----")]
	[Tooltip("This should ideally be between normal Speed and Dash Speed")]
	[Range(10, 60)] public int sprintSpeed = 15;
	[Range(0.0f, 50.0f)] public float sprintStaminaDrain = 25;

	[Header("Active Buttons")]
	#region Button Codes
	public string Button_Jump = "Jump";
	public string Button_Crouch = "Crouch";
	public string Button_Sprint = "Sprint";
	public string Button_Menu = "Cancel";
	public string Button_FirePrimary = "Fire1";
	public string Button_Interact = "Interact";
	#endregion

	[Header("----- Camera Stats -----")]
	[Range(400, 700)] public float horizontal = 400;
	[Range(300, 600)] public float vertical = 500;
	public bool invertX;

	[Header("Sound Settings")]
	[Range(0.0001f, 1)] public float masterVolume = 1f;
	[Range(0.0001f, 1)] public float musicVolume = .7f;
	[Range(0.0001f, 1)] public float sfxVolume = .7f;
	//These could be assigned from a json file at the start of the program if we wanted to have the settigns persist
	//between closing and reopening
	#region Defaults
	//Sound
	public float masterVolumeDefault = 1f;
	public float musicVolumeDefault = .7f;
	public float sfxVolumeDefault = .7f;

	//Camera
    public float horizontalDefault = 400;
	public float verticalDefault = 500;
	public bool invertXDefault= false;
    #endregion

    public void AssignActivetoDefault()
    {

		masterVolume = masterVolumeDefault;
		musicVolume = musicVolumeDefault;
		sfxVolume = sfxVolumeDefault;

		horizontal = horizontalDefault;
		vertical =  verticalDefault;
		invertX = invertXDefault;

	}
	public void GetDefaultsFromFile(/*string fileName*/)
    {
		AssignActivetoDefault();
	}
}
