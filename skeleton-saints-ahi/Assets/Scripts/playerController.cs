using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    // Components used by this script
    [Header("-=- Components -=-")]
    [SerializeField] CharacterController controller;

    // Stats that determine player movement
    [Header("Player Stats")]
    [Range(1, 30)] [SerializeField] int playerSpeed;
    [Range(1, 5)] [SerializeField] int maxJumps;
    [Range(3, 50)] [SerializeField] int jumpSpeed;
    [Range(0, 50)] [SerializeField] int gravity;
    [Range(1, 100)] [SerializeField] int maxHealth;

    // Stats for player dashing
    [Header("Dash Stats")]
    [SerializeField] bool dashEnabled;
    [Range(2, 100)] [SerializeField] int dashSpeed;
    [Tooltip("This should be longer or equal to Dash Duration")]
    [Range(0.0f, 20.0f)] [SerializeField] float dashCooldown;
    [Tooltip("This should be shorter or equal to Dash Cooldown")]
    [Range(0.0f, 5.0f)] [SerializeField] float dashDuration;

    // Stats used by the player's gun
    [Header("Weapon Stats")]
    [Range(5, 200)]     [SerializeField] int projectileSpeed;
    [Range(0.0f, 5.0f)] [SerializeField] float cooldown; // in seconds
    [Range(1, 200)]     [SerializeField] int range;
    [Range(1, 200)]     [SerializeField] int maxAmmo;
    [Tooltip("Will be rounded down if it exceeds maxAmmo.\nSet to 200 to always be equal to maxAmmo")]
    [Range(0, 200)] [SerializeField] int startingAmmo;
    [Range(1, 20)]      [SerializeField] int damage;


    // Debug bool. Enable to receive Debug messages in the console
    [Header("Miscellaneous")]
    [SerializeField] bool debugMessages;

    // Private variables used within the script to facilitate movement and actions
    Vector3 moveInput;
    Vector3 playerVelocity;
    int defaultSpeed;
    bool isDashing;

    public int jumpsCurrent = 0;
    public int currentHealth;
    public int currentAmmo;

    // Start is called before the first frame update
    void Start()
    {
        // Store the player's speed in another variable so that the player's speed can return to default after dashing
        defaultSpeed = playerSpeed;
        currentHealth = maxHealth;
        // If the starting ammo exceeds max ammo, bring it down to maxAmmo
        currentAmmo = startingAmmo;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        // If the player presses the Dash button, and they aren't currently dashing, initiate a dash
        if (!isDashing && Input.GetButtonDown("Dash") && dashEnabled)
            StartCoroutine(startDash());
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

    IEnumerator startDash()
    {
        // Start a dash, then reenable dashing when the cooldown expires
        if (debugMessages)
            Debug.Log("Starting a dash!");

        isDashing = true;
        StartCoroutine(dash());
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;

        if (debugMessages)
            Debug.Log("Ending dash");
    }

    IEnumerator dash()
    {
        // Increase the player's speed for the duration of the dash, then return it to normal
        playerSpeed = dashSpeed;
        if (debugMessages)
            Debug.Log($"Player's speed is now {playerSpeed}");

        yield return new WaitForSeconds(dashDuration);
        playerSpeed = defaultSpeed;
        if (debugMessages)
            Debug.Log($"Player's speed is now {playerSpeed}");
    }
}
