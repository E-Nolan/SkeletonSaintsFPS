using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    #region Member Fields
    // Components used by this script
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    public AudioSource audioSource;
    [SerializeField] Transform weaponFirePos;
    //[SerializeField] GameObject bullet;
    [SerializeField] List<GameObject> weaponInventory;
    [SerializeField] GameObject currentWeapon;
    [SerializeField] Camera playerCamera;

    // Stats that determine player movement
    [Header("----- Player Stats -----")]
    [Range(5, 30)] [SerializeField] int playerSpeed;
    [Range(0, 50)] [SerializeField] int gravity;
    [Range(1, 100)] [SerializeField] int maxHealth;
    [Range(10, 500)] [SerializeField] int maxStamina;
    [Range(0.0f, 100.0f)] [SerializeField] int staminaRegenSpeed;
    [Tooltip("This is how long (in seconds) the player will have to stop using stamina in order for their stamina to regenerate")]
    [Range(0.0f, 5.0f)] [SerializeField] float staminaRegenCooldown;

    [Header("----- Jump Stats -----")]
    [Range(3, 50)] [SerializeField] int jumpSpeed;
    [Range(0, 3)] [SerializeField] int maxJumps;
    [Range(0, 100)] [SerializeField] int jumpStaminaCost;

    // Stats for player dashing
    [Header("----- Dash Stats -----")]
    [Range(15, 100)] [SerializeField] int dashSpeed;
    [Tooltip("This should be longer or equal to Dash Duration")]
    [Range(0.0f, 20.0f)] [SerializeField] float dashCooldown;
    [Tooltip("This should be shorter or equal to Dash Cooldown")]
    [Range(0.0f, 5.0f)] [SerializeField] float dashDuration;
    [Range(0, 100)] [SerializeField] int dashStaminaCost;

    [Header("----- Sprint Stats -----")]
    [Tooltip("This should ideally be between normal Speed and Dash Speed")]
    [Range(10,60)] [SerializeField] int sprintSpeed;
    [Range(0.0f, 50.0f)] [SerializeField] int sprintStaminaDrain;

    [Header("----- Sound Effects -----")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip[] walkSoundsGroup;
    [SerializeField] AudioClip[] runSoundsGroup;
    [SerializeField] AudioClip gunshotSound;

    [Header("Miscellaneous")]
    [Range(1, 200)] [SerializeField] int raycastRange;
    // Private variables used within the script to facilitate movement and actions
    Vector3 moveInput;
    Vector3 playerVelocity;
    int defaultSpeed;
    float staminaRegenTimer;

    public bool isDashing = false;
    public bool isSprinting = false;
    public bool isShooting = false;

    public float currentStamina;
    public int jumpsCurrent = 0;
    public int currentHealth;

    bool canPlayFootsteps = true;
    public IWeapon weaponInterface;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Store the player's speed in another variable so that the player's speed can return to default after dashing
        defaultSpeed = playerSpeed;
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        updatePlayerHealthBar();
        updatePlayerStaminaBar();

        weaponInterface = currentWeapon.GetComponent<IWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        // Decrement the stamina regen timer. If any stamina is used this frame, the timer will be reset
        if (staminaRegenTimer > 0.0f)
            staminaRegenTimer -= Time.deltaTime;

        // Handle movement for the player
        movement();

        // If the player presses the Shoot Button, they will fire at whatever they are looking at
        // They can not fire if they do not have ammo
        if (Input.GetButton("Fire1") && !isShooting && !gameManager.instance.isPaused)
        {
            shoot();
        }

        // If the player hasn't used any stamina for the duration of the regen cooldown, regenerate their stamina over time
        if (staminaRegenTimer <= 0)
            giveStamina(staminaRegenSpeed * Time.deltaTime);
    }

    #region movement functions
    // Tell the player where to move based on player input
    void movement()
    {
        // Drain the player's stamina while they're sprinting
        // Stop their sprint if they run out of stamina
        if (isSprinting)
        {
            useStamina(sprintStaminaDrain * Time.deltaTime);
            if (currentStamina <= 0)
            {
                playerSpeed = defaultSpeed;
                isSprinting = false;
            }
        }

        // If the player releases the dash button while sprinting, return them to normal speed
        // Also stop their sprint if they ran out of stamina
        if (isSprinting && !isDashing && !Input.GetButton("Dash"))
        {
            playerSpeed = defaultSpeed;
            isSprinting = false;
        }

        // If the player presses the Dash button, and they aren't currently dashing, initiate a dash
        // If the player continues holding the dash button, they will start sprinting
        if (Input.GetButtonDown("Dash") && !isDashing && currentStamina >= dashStaminaCost)
            StartCoroutine(startDash());

        // Set the player's vertical velocity to 0 if they are standing on ground
        if (controller.isGrounded && playerVelocity.y <= 0)
        {
            playerVelocity.y = 0;
            jumpsCurrent = 0;
        }

        // Move the character via arrow keys/WASD input
        moveInput = (transform.right * Input.GetAxis("Horizontal") + 
                transform.forward * Input.GetAxis("Vertical"));
        
        // Play walk sound effects if the player is walking on ground
        // Play run sound effects if the player is sprinting on ground
        if (controller.isGrounded && moveInput.magnitude >= 0.1 && canPlayFootsteps)
            StartCoroutine(playFootstep());

        controller.Move(moveInput * Time.deltaTime * playerSpeed);

        // Allow the player to jump if they haven't exceeded their maximum amount of jumps
        // TODO: Prevent the player from being able to perform a mid-air jump after falling off a ledge
        if (Input.GetButtonDown("Jump") && jumpsCurrent < maxJumps)
        {
            jumpsCurrent++;
            playerVelocity.y = jumpSpeed;
            useStamina(jumpStaminaCost);
            audioSource.PlayOneShot(jumpSound);
        }

        // Accelerate the player downward via gravity
        playerVelocity.y -= (gravity * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator startDash()
    {
        // Start a dash, then reenable dashing when the cooldown expires
        isDashing = true;
        isSprinting = false;
        useStamina(dashStaminaCost);
        audioSource.PlayOneShot(dashSound);
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

    #endregion

    void shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, raycastRange))
        {
            // If the raycast hit something, instantiate a bullet and send it flying in that object's direction
            Vector3 directionToTarget = (hit.point - weaponFirePos.transform.position);
            Debug.DrawRay(transform.position, directionToTarget);
            weaponInterface.shoot(directionToTarget.normalized);
        }
        else
        {
            // If the raycast didn't hit anything, fire a bullet straight forwards
            weaponInterface.shootForward();
        }
    }

    /// <summary>
    /// Returns true if the player's health is full
    /// </summary>
    /// <returns></returns>
    public bool isHealthFull()
    {
        return (currentHealth >= maxHealth);
    }

    /// <summary>
    /// Deal damage to the player, then kill them if their health reaches 0.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        updateHealth(-damage);
        updatePlayerHealthBar();
        StartCoroutine(flashDamage());
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.damageFlashScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageFlashScreen.SetActive(false);
    }

    /// <summary>
    /// Heal the player by an amount. Their health can not exceed their max health
    /// </summary>
    /// <param name="amount"></param>
    public void GiveHealth(int amount)
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
        updatePlayerHealthBar();
        if (currentHealth == 0)
            gameManager.instance.playerDead();
    }

    public void updatePlayerHealthBar()
    {
        gameManager.instance.playerHealthBar.fillAmount = (float) currentHealth / (float) maxHealth;
    }

    /// <summary>
    /// Give stamina to the player. Their stamina can't exceed their max stamina
    /// </summary>
    /// <param name="amount"></param>
    public void giveStamina(float amount)
    {
        updateStamina(amount);
    }

    /// <summary>
    /// Take stamina away from the current player's stamina as a cost for abilities.
    /// Also puts their stamina regen on cooldown
    /// </summary>
    /// <param name="staminaCost"></param>
    public void useStamina(float staminaCost)
    {
        updateStamina(-staminaCost);
    
        if (staminaCost > 0.0f)
            staminaRegenTimer = staminaRegenCooldown;
    }

    // Add or subtract from the player's current stamina. Clamped between 0 and max stamina
    void updateStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0.0f, (float)maxStamina);
        updatePlayerStaminaBar();
    }

    public void updatePlayerStaminaBar()
    {
        gameManager.instance.playerStaminaBar.fillAmount = (float) currentStamina / (float) maxStamina;
    }

    
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetMaxStamina()
    {
        return maxStamina;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    IEnumerator playFootstep()
    {
        if (isSprinting)
            audioSource.PlayOneShot(runSoundsGroup[Random.Range(0, runSoundsGroup.Length)]);
        else
            audioSource.PlayOneShot(walkSoundsGroup[Random.Range(0, walkSoundsGroup.Length)]);

        canPlayFootsteps = false;
        yield return new WaitForSeconds(0.35f);
        canPlayFootsteps = true;
    }


}
