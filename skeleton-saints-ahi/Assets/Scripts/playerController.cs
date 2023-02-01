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
    [Range(1, 5)]  [SerializeField] int maxJumps;
    [Range(3, 50)] [SerializeField] int jumpSpeed;
    [Range(0, 50)] [SerializeField] int gravity;

    // Stats used by the player's gun
    [Header("Weapon Stats")]
    [Range(5, 200)]     [SerializeField] int projectileSpeed;
    [Range(0.0f, 5.0f)] [SerializeField] float cooldown; // in seconds
    [Range(1, 200)]     [SerializeField] int range;
    [Range(1, 200)]     [SerializeField] int maxAmmo;
    [Range(1, 20)]      [SerializeField] int damage;


    // Debug bool. Enable to receive Debug messages in the console
    [Header("Miscellaneous")]
    [SerializeField] bool debugMessages;

    // Private variables used within the script to facilitate movement and actions
    private Vector3 moveInput;
    private Vector3 playerVelocity;
    private int jumpsCurrent = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
