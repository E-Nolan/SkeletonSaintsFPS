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
	public GameObject OffWeapon;

	[Header("----- Stats -----")]
	[Range(5, 30)]
	public int playerSpeed;
	[Range(0, 50)]
	public int gravity;

	[Header("----- Stamina -----")]
	[Range(10, 500)]
	public int maxStamina;
	[Range(0.0f, 100.0f)]
	public float staminaRegenSpeed;
	[Tooltip("This is how long (in seconds) the player will have to stop using stamina in order for their stamina to regenerate")]
	[Range(0.0f, 5.0f)]
	public float staminaRegenCooldown;

	[Header("----- Health and Armor -----")]
	[Range(1, 100)]
	public int maxHealth;
	[Range(0, 5)]
	public int maxArmor;
	[Range(0.0f, 10.0f)]
	public float armorRegenSpeed;
	[Range(0.0f, 5.0f)]
	public float armorRegenCooldown;
	[Range(0.0f, 2.0f)]
	public float invincibilityCooldown;

	[Header("----- Jump -----")]
	[Range(1.0f, 10.0f)]
	public float maxJumpVel;
	[Range(0.05f, 1000.0f)]
	public float jumpAcceleration;
	[Range(0, 3)]
	public int maxJumps;
	[Range(0, 100)]
	public int jumpStaminaCost;
	[Range(0.0f, 1.0f)]
	public float coyoteTime;
	[Range(0.0f, 1.0f)]
	public float jumpInputCooldown;

	// Stats for player dashing
	[Header("----- Dash -----")]
	[Range(15, 100)]
	public int dashSpeed;
	[Tooltip("This should be longer or equal to Dash Duration")]
	[Range(0.0f, 20.0f)]
	public float dashCooldown;
	[Tooltip("This should be shorter or equal to Dash Cooldown")]
	[Range(0.0f, 5.0f)]
	public float dashDuration;
	[Range(0, 100)]
	public int dashStaminaCost;
	[Range(0.0f, 1.0f)]
	public float dashInvincibilityTime;

	[Header("----- Sprint -----")]
	[Tooltip("This should ideally be between normal Speed and Dash Speed")]
	[Range(10, 60)] 
	public int sprintSpeed;
	[Range(0.0f, 50.0f)]
	public float sprintStaminaDrain;

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
	[Range(400, 700)]
	public float horizontal;
	[Range(300, 600)]
	public float vertical;
	public bool invertX;

	[Header("Sound Settings")]
	[Range(0.0001f, 1)]
	public float masterVolume;
	[Range(0.0001f, 1)]
	public float musicVolume;
	[Range(0.0001f, 1)]
	public float sfxVolume;
	//These could be assigned from a json file at the start of the program if we wanted to have the settigns persist
	//between closing and reopening
	#region Defaults
	//Sound
	public float masterVolumeDefault = 1f;
	public float musicVolumeDefault = 1f;
	public float sfxVolumeDefault = 1f;

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
