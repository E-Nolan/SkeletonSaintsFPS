using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{

    // Components used by this script
    [Header("-=- Components -=-")]
    [SerializeField] CharacterController controller;

    // Stats that determine player movement
    [Header("Player Stats")]
    [Range(5, 30)] [SerializeField] int playerSpeed;
    [Range(1, 5)] [SerializeField] int maxJumps;
    [Range(3, 50)] [SerializeField] int jumpSpeed;
    [Range(0, 50)] [SerializeField] int gravity;
    [Range(1, 100)] [SerializeField] int maxHealth;
    [Range(1, 10)] [SerializeField] int materialFlashSpeed;

    // Stats for player dashing
    [Header("Dash Stats")]
    [SerializeField] bool dashEnabled;
    [Range(15, 100)] [SerializeField] int dashSpeed;
    [Tooltip("This should be longer or equal to Dash Duration")]
    [Range(0.0f, 20.0f)] [SerializeField] float dashCooldown;
    [Tooltip("This should be shorter or equal to Dash Cooldown")]
    [Range(0.0f, 5.0f)] [SerializeField] float dashDuration;
    [Range(10,60)] [SerializeField] int sprintSpeed;

    // Stats used by the player's gun
    [Header("Weapon Stats")]
    [Range(5, 200)]     [SerializeField] int projectileSpeed;
    [Range(0.0f, 5.0f)] [SerializeField] float cooldown; // in seconds
    [Range(1, 200)]     [SerializeField] int range;
    [Range(1, 200)]     [SerializeField] int maxAmmo;
    [Tooltip("Will be rounded down if it exceeds maxAmmo.\nSet to 200 to always be equal to maxAmmo")]
    [Range(0, 200)] [SerializeField] int startingAmmo;
    [Range(1, 20)]      [SerializeField] int damage;

    [Header("Miscellaneous")]

    // Private variables used within the script to facilitate movement and actions
    Vector3 moveInput;
    Vector3 playerVelocity;
    int defaultSpeed;
    public bool isDashing;
    public bool isSprinting;

    public int jumpsCurrent = 0;
    public int currentHealth;
    public int currentAmmo;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        // Store the player's speed in another variable so that the player's speed can return to default after dashing
        defaultSpeed = playerSpeed;
        currentHealth = maxHealth;
        // If the starting ammo exceeds max ammo, bring it down to maxAmmo
        currentAmmo = startingAmmo;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);

        material = GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player releases the dash button while sprinting, return them to normal speed
        if (Input.GetButtonUp("Dash") && !isDashing && isSprinting)
        {
            playerSpeed = defaultSpeed;
            isSprinting = false;
        }

        // If the player presses the Dash button, and they aren't currently dashing, initiate a dash
        // If the player continues holding the dash button, they will start sprinting
        if (!isDashing && Input.GetButtonDown("Dash") && dashEnabled)
            StartCoroutine(startDash());

        movement();
    }

    // Tell the player where to move based on player input
    void movement()
    {
        // Set the player's vertical velocity to 0 if they are standing on ground
        if (controller.isGrounded && playerVelocity.y <= 0)
        {
            playerVelocity.y = 0;
            jumpsCurrent = 0;
        }

        // Move the character via arrow keys/WASD input
        moveInput = (transform.right * Input.GetAxis("Horizontal") + 
                transform.forward * Input.GetAxis("Vertical"));

        controller.Move(moveInput * Time.deltaTime * playerSpeed);

        // Allow the player to jump if they haven't exceeded their maximum amount of jumps
        // TODO: Prevent the player from being able to perform a mid-air jump after falling off a ledge
        if (Input.GetButtonDown("Jump") && jumpsCurrent < maxJumps)
        {
            jumpsCurrent++;
            playerVelocity.y = jumpSpeed;
        }

        // Accelerate the player downward via gravity
        playerVelocity.y -= (gravity * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Return true if the player's ammo is full
    /// </summary>
    public bool isAmmoFull()
    {
        return (currentAmmo == maxAmmo);
    }

    /// <summary>
    /// Return true if the player has no ammo
    /// </summary>
    public bool isAmmoEmpty()
    {
        return (currentAmmo == 0);
    }

    /// <summary>
    /// Give the player an amount of ammo (positive input), or take it away (negative input).
    /// Clamp it if it exceeds max ammo or becomes negative
    /// </summary>
    /// <param name="amount"></param>
    public void updateAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
    }

    /// <summary>
    /// Deal damage to the player, then kill them if their health reaches 0.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        updateHealth(-damage);
    }

    /// <summary>
    /// Heal the player by an amount. Their health can not exceed their max health
    /// </summary>
    /// <param name="amount"></param>
    public void HealPlayer(int amount)
    {
        updateHealth(amount);
    }

    /// <summary>
    /// Deal damage to the player (negative input) or heal the player (positive input)
    /// Clamp it if it exceeds max health
    /// </summary>
    /// <param name="amount"></param>
    void updateHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, int.MinValue, maxHealth);

        if (currentHealth == 0)
            killPlayer();
    }

    /// <summary>
    /// Kill the player. runs when the player's health reaches 0, but can be called for other reasons.
    /// </summary>
    public void killPlayer()
    {
        Debug.Log($"{gameObject.name} has died");
        Destroy(gameObject);
        // TODO: Implement this functionality
    }


    IEnumerator startDash()
    {
        // Start a dash, then reenable dashing when the cooldown expires
        isDashing = true;
        isSprinting = false;
        StartCoroutine(dash());

        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
    }

    IEnumerator dash()
    {
        // Increase the player's speed for the duration of the dash, then return it to normal
        playerSpeed = dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        // If the player is still holding the dash button down after the dash ends, they will continue sprinting at an increased speed
        if (Input.GetButton("Dash"))
        {
            playerSpeed = sprintSpeed;
            isSprinting = true;
        }
        else
        {
            playerSpeed = defaultSpeed;
        }
    }
}
