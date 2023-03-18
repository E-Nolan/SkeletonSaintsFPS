using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    #region Member Fields
    // Components used by this script
    [Header("----- Components -----")]
    [SerializeField] weaponStats startingWeapon;
    [SerializeField] CharacterController controller;
    public AudioSource audioSource;
    [SerializeField] Transform leftFirePos;
    [SerializeField] Transform rightFirePos;
    //[SerializeField] GameObject bullet;
    [SerializeField] public List<GameObject> weaponInventory;
    [SerializeField] public GameObject offHandWeapon;
    [SerializeField] Camera playerCamera;

    // Stats that determine player movement
    [Header("----- Stats -----")]
    [Range(5, 30)] [SerializeField] int playerSpeed;
    [Range(0, 50)] [SerializeField] int gravity;

    [Header("----- Stamina -----")]
    [Range(10, 500)] [SerializeField] int maxStamina;
    [Range(0.0f, 100.0f)] [SerializeField] float staminaRegenSpeed;
    [Tooltip("This is how long (in seconds) the player will have to stop using stamina in order for their stamina to regenerate")]
    [Range(0.0f, 5.0f)] [SerializeField] float staminaRegenCooldown;

    [Header("----- Health and Armor -----")]
    [Range(1, 100)] [SerializeField] int maxHealth;
    [Range(0, 5)] [SerializeField] int maxArmor;
    [Range(0.0f, 10.0f)] [SerializeField] float armorRegenSpeed;
    [Range(0.0f, 5.0f)] [SerializeField] float armorRegenCooldown;
    [Range(0.0f, 2.0f)] [SerializeField] float invincibilityCooldown;

    [Header("----- Jump -----")]
    [Range(1.0f, 10.0f)] [SerializeField] float maxJumpVel;
    [Range(0.05f, 1000.0f)] [SerializeField] float jumpAcceleration;
    [Range(0.0f, 10.0f)] [SerializeField] float initialJumpVelocity;
    [Range(0, 3)] [SerializeField] int maxJumps;
    [Range(0, 100)] [SerializeField] int jumpStaminaCost;
    [Range(0.0f, 1.0f)] [SerializeField] float coyoteTime;
    [Range(0.0f, 1.0f)] [SerializeField] float jumpInputCooldown;

    // Stats for player dashing
    [Header("----- Dash -----")]
    [Range(15, 100)] [SerializeField] int dashSpeed;
    [Tooltip("This should be longer or equal to Dash Duration")]
    [Range(0.0f, 20.0f)] [SerializeField] float dashCooldown;
    [Tooltip("This should be shorter or equal to Dash Cooldown")]
    [Range(0.0f, 5.0f)] [SerializeField] float dashDuration;
    [Range(0, 100)] [SerializeField] int dashStaminaCost;
    [Range(0.0f, 1.0f)] [SerializeField] float dashInvincibilityTime;

    [Header("----- Sprint -----")]
    [Tooltip("This should ideally be between normal Speed and Dash Speed")]
    [Range(10, 60)] [SerializeField] int sprintSpeed;
    [Range(0.0f, 50.0f)] [SerializeField] float sprintStaminaDrain;

    [Header("----- Sound Effects -----")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip[] walkSoundsGroup;
    [SerializeField] AudioClip[] runSoundsGroup;
    [SerializeField] AudioClip gunshotSound;

    [Header("Miscellaneous")]
    [Range(1, 200)] [SerializeField] int raycastRange;
    [Range(1, 20)] [SerializeField] int externalVelocityDecay;
    // Private variables used within the script to facilitate movement and actions
    Vector3 moveInput;
    [SerializeField] Vector3 playerVelocity;
    Vector3 externalVelocity = Vector3.zero;
    int defaultSpeed;
    float staminaRegenTimer = 0.0f;
    float armorRegenTimer = 0.0f;
    [SerializeField] float invincibilityTimer = 0.0f;
    [SerializeField] float currCoyoteTimer = 0.0f;
    bool canInputJump = false;
    [SerializeField] bool isJumping = false;

    [SerializeField] weaponStats backupGrapple;

    public rangedWeapon currentWeapon { get; private set; }
    public rangedWeapon currentSecondary { get; private set; }
    public List<rangedWeapon> inactiveWeapons { get; private set; }
    public bool isDashing { get; private set; } = false;
    public bool isSprinting { get; private set; } = false;
    public bool isPrimaryShooting = false;
    public bool isSecondaryShooting = false;
    public bool isGrappling = false;

    bool isSwitchingWeapons = false;
    float weaponSwitchCooldown = 0.1f;

    public float currentStamina { get; private set; }
    public int jumpsCurrent; // { get; private set; } = 0;
    public float currentHealth; //{ get; private set; }
    public float currentArmor; //{ get; private set; }

    bool canPlayFootsteps = true;
    public int currWepIndex { get; private set; } = 0;

    bool godModeEnabled = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Store the player's speed in another variable so that the player's speed can return to default after dashing
        defaultSpeed = playerSpeed;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentArmor = maxArmor;

        gameManager.instance.createUIBar();

        if (inactiveWeapons == null)
            inactiveWeapons = new List<rangedWeapon>();

        if (startingWeapon)
            rangedWeaponPickup(startingWeapon, startingWeapon.weaponType);
    }

    // Update is called once per frame
    void Update()
    {
        // Decrement any ongoing timers
        if (staminaRegenTimer > 0.0f)
            staminaRegenTimer -= Time.deltaTime;
        if (armorRegenTimer > 0.0f)
            armorRegenTimer -= Time.deltaTime;
        if (invincibilityTimer > 0.0f)
            invincibilityTimer -= Time.deltaTime;
        if (currCoyoteTimer > 0.0f)
            currCoyoteTimer -= Time.deltaTime;

        //Cheat Codes
        if (Input.GetKeyDown(KeyCode.I))
            godMode();


        // Handle movement for the player
        movement();

        // If the player presses the Shoot Button, they will fire at whatever they are looking at

        if (Input.GetButton("Fire1") && !isPrimaryShooting && !gameManager.instance.isPaused && currentWeapon)
        {
            // If auto reloading is enabled, simply try to shoot the weapon. otherwise show a message on screen
#if false
            if (currentWeapon.CurrentClip > 0)
                shoot(currentWeapon);
            else
                hUDManager.instance.displayReloadWeaponText();
#else
            shoot(currentWeapon);
#endif
        }

        if (Input.GetButtonDown("Reload") && !currentWeapon.isAmmoEmpty())
        {
            currentWeapon.startReload();
        }

        // If the player presses the secondary Shoot button (right click) Fire their secondary weapon
        if (Input.GetButtonDown("Fire2") && !isSecondaryShooting && !gameManager.instance.isPaused && currentSecondary)
        {
            shoot(currentSecondary);
        }

        // If the player scrolls the mouse wheel, switch to the next weapon for up, or the previous weapon for down.
        if (!isSwitchingWeapons)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                StartCoroutine(nextWeapon());
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                StartCoroutine(prevWeapon());
            }
        }


        // If the player hasn't used any stamina for the duration of the regen cooldown, regenerate their stamina over time
        if (staminaRegenTimer <= 0)
            giveStamina(staminaRegenSpeed * Time.deltaTime);
        // If the player hasn't taken damage for the duration of the regen cooldown, regenerate their armor over time
        if (armorRegenTimer <= 0)
            giveArmor(armorRegenSpeed * Time.deltaTime);
    }

#region movement functions
    // Tell the player where to move based on player input
    void movement()
    {
        // Drain the player's stamina while they're sprinting
        // Stop their sprint if they run out of stamina
        if (isSprinting && !isGrappling)
        {
            useStamina(sprintStaminaDrain * Time.deltaTime);
            if (currentStamina <= 0)
            {
                playerSpeed = defaultSpeed;
                isSprinting = false;
            }
        }

        // If the player releases the dash button while sprinting or stops moving while sprinting, return them to normal speed
        // Also stop their sprint if they ran out of stamina
        if (isSprinting && !isDashing && (!Input.GetButton("Dash") || moveInput.magnitude <= 0.05f))
        {
            playerSpeed = defaultSpeed;
            isSprinting = false;
        }

        // If the player presses the Dash button, and they aren't currently dashing, initiate a dash
        // If the player continues holding the dash button, they will start sprinting
        if (Input.GetButtonDown("Dash"))
        {
            if (!isDashing && currentStamina >= dashStaminaCost)
                StartCoroutine(startDash());
            else
                startSprint();
        }

        // Lerp external velocity back down to zero over time only if the player isn't actively grappling
        if (!isGrappling)
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, Time.deltaTime * externalVelocityDecay);

        // If the player isn't grounded, start the coyote time timer.
        // If the time has expired and they haven't used their first jump, take their first jump away
        if (!controller.isGrounded)
        {
            if (currCoyoteTimer <= 0.0f && jumpsCurrent == 0)
            {
                jumpsCurrent = 1;
            }
        }

        // Set the player's vertical velocity to 0 and reset their jumps if they are standing on ground
        if (isGrappling || (controller.isGrounded && playerVelocity.y <= 0))
        {
            playerVelocity.y = 0;
            if (!isGrappling)
            {
                currCoyoteTimer = coyoteTime;
                jumpsCurrent = 0;
            }
        }

        // Move the character via arrow keys/WASD input
        if (!isGrappling)
        {
            moveInput = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
            if (moveInput.magnitude > 1.0f)
                moveInput = moveInput.normalized;
        }

        // Play walk sound effects if the player is walking on ground
        // Play run sound effects if the player is sprinting on ground
        if (controller.isGrounded && moveInput.magnitude >= 0.1 && canPlayFootsteps)
            StartCoroutine(playFootstep());

        controller.Move(moveInput * Time.deltaTime * playerSpeed);

        // If the player presses the jump button, initiate a jump
        // Jump inputs have a cooldown
        // When the player releases the jump button, stop the jump
        if (Input.GetButtonDown("Jump") && jumpsCurrent < maxJumps && !canInputJump && jumpStaminaCost <= currentStamina)
        {
            playerVelocity.y = Mathf.Clamp((initialJumpVelocity + playerVelocity.y), initialJumpVelocity, float.MaxValue);
            isJumping = true;
            jumpsCurrent++;
            useStamina(jumpStaminaCost);
            audioSource.PlayOneShot(jumpSound);
            StartCoroutine(startJumpInputCooldown());
        }
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // Allow the player to jump if they haven't exceeded their maximum amount of jumps
        if (!isGrappling && Input.GetButton("Jump") && isJumping)
        {
            if (playerVelocity.y < 0.0f)
                playerVelocity.y = 0.0f;
            playerVelocity.y += jumpAcceleration * Time.deltaTime;
            if (playerVelocity.y >= maxJumpVel)
                isJumping = false;
        }

        // Accelerate the player downward via gravity
        if (!isJumping)
            playerVelocity.y -= (gravity * Time.deltaTime);
        controller.Move((playerVelocity + externalVelocity) * Time.deltaTime);
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
        // The player also gains a brief moment of invincibility upon dashing
        invincibilityTimer = Mathf.Clamp(dashInvincibilityTime, invincibilityTimer, invincibilityCooldown);

        yield return new WaitForSeconds(dashDuration);

        // If the player is still holding the dash button down after the dash ends, they will continue sprinting at an increased speed
        if (Input.GetButton("Dash"))
        {
            startSprint();
        }
        else
        {
            playerSpeed = defaultSpeed;
        }
    }

    void startSprint()
    {
        playerSpeed = sprintSpeed;
        isSprinting = true;
    }

    IEnumerator startJumpInputCooldown()
    {
        canInputJump = true;
        yield return new WaitForSeconds(jumpInputCooldown);
        canInputJump = false;
    }

#endregion

    void shoot(rangedWeapon _shotWeapon)
    {
        if (!_shotWeapon.isClipEmpty())
        {
            RaycastHit hit;
            if (GetCurrentReticleHit(out hit))
            {
                ////Debug.Log($"The player is shooting at {hit.point}");
                // If the raycast hit something, instantiate a bullet and send it flying in that object's direction
                Vector3 directionToTarget = (hit.point - leftFirePos.transform.position);
                Debug.DrawRay(transform.position, directionToTarget, Color.red, 1.0f);
                _shotWeapon.shoot(hit.point);
            }
            else
            {
                // If the raycast didn't hit anything, fire a bullet straight forwards
                _shotWeapon.shootForward();
            }
        }
        else
            _shotWeapon.startReload();
    }

    public bool GetCurrentReticleHit(out RaycastHit _hit)
    {
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out _hit, raycastRange) && _hit.distance >= 1.0f)
            return true;
        else
            return false;
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
    public void TakeDamage(float damage)
    {
        if (invincibilityTimer <= 0.0f && !godModeEnabled)
        {
            takeArmorDamage(ref damage);
            fudgeDamage(ref damage);
            updateHealth(-damage);
            hUDManager.instance.updatePlayerHealthBar();
            StartCoroutine(hUDManager.instance.flashDamage(damage));
            invincibilityTimer = invincibilityCooldown;
        }
    }

    void fudgeDamage(ref float damage)
    {
        // If the player is low on health, reduce the amount of damage being dealt to the player to help create tense moments
        // Also, if the player would be dealt a killing blow while they're above 1 hp, reduce the damage to put them at 1 hp instead
        if (damage >= (currentHealth * 0.5f))
        {
            damage = (damage + currentHealth) * 0.25f;

            if (damage >= currentHealth && currentHealth > 1.0f)
            {
                damage = currentHealth - 1.0f;
            }
        }
    }

    void takeArmorDamage(ref float damage)
    {
        
        // Deal damage to the player's armor, if the damage exceeds their armor, the excess will be dealt to their health.
        currentArmor -= damage;
        if (currentArmor < 0)
        {
            damage = -currentArmor;
            currentArmor = 0;
        }
        else
            damage = 0;

        hUDManager.instance.updatePlayerArmorBar();
        // Reset the armor regen cooldown
        armorRegenTimer = armorRegenCooldown;

    }

    public void rangedWeaponPickup(weaponStats _newWeaponStats, weaponStats.weaponStatsType _weaponType)
    {
        // turn isShooting off to prevent a bug where picking up a weapon while shooting could disable shooting entirely
        isPrimaryShooting = false;
        GameObject _newWeapon;
        GameObject _newFirePos;

        // If the picked up object is a grapple gun, add it to the player's off hand (right)
        // Otherwise, add it to their primary hand (left)
        switch (_weaponType)
        {
            case weaponStats.weaponStatsType.GrappleGun:
                _newWeapon = new GameObject(_newWeaponStats.name, typeof(grappleGun), typeof(AudioSource));
                _newWeapon.transform.parent = playerCamera.transform;
                _newWeapon.transform.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);

                _newFirePos = Instantiate(rightFirePos.gameObject, rightFirePos.position, rightFirePos.rotation, _newWeapon.transform);
                _newWeapon.GetComponent<grappleGun>().copyFromWeaponStats(_newWeaponStats, _newFirePos.transform, true);
                offHandWeapon = _newWeapon;
                currentSecondary = offHandWeapon.GetComponent<grappleGun>();
                break;

            default:
                _newWeapon = new GameObject(_newWeaponStats.name, typeof(rangedWeapon), typeof(AudioSource));
                _newWeapon.transform.parent = playerCamera.transform;
                _newWeapon.transform.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);

                _newFirePos = Instantiate(leftFirePos.gameObject, leftFirePos.position, leftFirePos.rotation, _newWeapon.transform);
                _newWeapon.GetComponent<rangedWeapon>().copyFromWeaponStats(_newWeaponStats, _newFirePos.transform, true);
                weaponInventory.Add(_newWeapon);
                switchToWeapon(weaponInventory.Count - 1);
                
                inactiveWeapons.Add(null);
                break;
        }
    }

    IEnumerator nextWeapon()
    {
        isSwitchingWeapons = true;
        // Switch to the weapon after the current one in the List
        // Switch to the first weapon if the index goes out of bounds
        if (currWepIndex + 1 >= weaponInventory.Count)
            switchToWeapon(0); 
        else
            switchToWeapon(currWepIndex + 1);
        
        yield return new WaitForSeconds(weaponSwitchCooldown);
        isSwitchingWeapons = false;
    }

    IEnumerator prevWeapon()
    {
        isSwitchingWeapons = true;
        // Switch to the weapon before the current one in the List
        // Switch to the last weapon if the index goes out of bounds
        if (currWepIndex - 1 < 0)
            switchToWeapon(weaponInventory.Count - 1);
        else
            switchToWeapon(currWepIndex - 1);

        yield return new WaitForSeconds(weaponSwitchCooldown);
        isSwitchingWeapons = false;
    }

    void switchToWeapon(int weaponIndex)
    {
        //weaponInventory = playerPreferences.instance.MainWeapons;
        //offHandWeapon = playerPreferences.instance.OffWeapon;
        if (currentWeapon != weaponInventory[weaponIndex])
        {
            // Switch to the weapon at the given index, update currWepIndex to the new weapon's index, and turn the old weapon off
            if (currentWeapon)
            {
                currentWeapon.offSwitch();
                currentWeapon.gameObject.SetActive(false);
            }

            currWepIndex = weaponIndex;
            currentWeapon = weaponInventory[currWepIndex].GetComponent<rangedWeapon>();
        
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.onSwitch();

            // For each weapon in the player's inventory beyond than the current weapon, update the inactive weapons array to include
            for (int i = 1; i < weaponInventory.Count; i++)
            {
                int _inactiveIndex = currWepIndex + i;
                if (_inactiveIndex >= weaponInventory.Count)
                    _inactiveIndex -= weaponInventory.Count;

                if ((i - 1) < inactiveWeapons.Count)
                    inactiveWeapons[i - 1] = weaponInventory[_inactiveIndex].GetComponent<rangedWeapon>();
                else
                    inactiveWeapons.Add(weaponInventory[_inactiveIndex].GetComponent<rangedWeapon>());
            }

            hUDManager.instance.updateWeaponDisplay();

        }
    }

    /// <summary>
    /// Switch the player's weapon to a chosen index in the player's inventory.
    /// </summary>
    /// <param name="weaponIndex"></param>
    public void extSwitchToWeapon(int weaponIndex)
    {
        if (gameManager.instance.PlayStarted())
        {
            //Debug.Log("Outside assignment of currentWeapon is invalid \n Only playerController and GM may call this");
            return;
        }
        else
        {
            switchToWeapon(weaponIndex);
        }
    }

    public void giveExternalVelocity(Vector3 _extraVelocity)
    {
        externalVelocity += _extraVelocity;
    }

    /// <summary>
    /// Heal the player by an amount. Their health can not exceed their max health
    /// </summary>
    /// <param name="amount"></param>
    public void GiveHealth(float amount)
    {
        updateHealth(amount);
    }

    /// <summary>
    /// Deal damage to the player (negative input) or heal the player (positive input)
    /// Clamp it if it exceeds max health
    /// </summary>
    /// <param name="amount"></param>
    void updateHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, int.MinValue, maxHealth);
        hUDManager.instance.updatePlayerHealthBar();
        if (currentHealth <= 0)
            gameManager.instance.playerDead();
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
        hUDManager.instance.updatePlayerStaminaBar();

    }

    public void giveArmor(float armorGain)
    {
        currentArmor += armorGain;
        currentArmor = Mathf.Clamp(currentArmor, 0.0f, maxArmor);
        hUDManager.instance.updatePlayerArmorBar();
    }

    public void SetInvincibilityState(bool _isInvincible)
    {
        switch (_isInvincible)
        {
            case true:
                invincibilityTimer = invincibilityCooldown;
                break;
            case false:
                invincibilityTimer = 0.0f;
                break;
        }
    }

    #region Public Member Accessors
    public int GetMaxHealth()
    { return maxHealth; }

    public float GetCurrentHealth()
    { return currentHealth; }

    public int GetMaxArmor()
    { return maxArmor; }

    public float GetCurrentArmor()
    { return currentArmor; }

    public int GetMaxStamina()
    { return maxStamina; }

    public float GetCurrentStamina()
    { return currentStamina; }

    public bool isAmmoInfinite()
    { return currentWeapon.isAmmoInfinite(); }

    public bool IsPlayerShooting()
    { return (isPrimaryShooting || isSecondaryShooting); }

    public bool IsPrimaryShooting()
    { return isPrimaryShooting; }

    public bool IsSecondaryShooting()
    { return isSecondaryShooting; }

    public Vector3 GetPlayerVelocity()
    { return controller.velocity + moveInput * playerSpeed + externalVelocity; }

    public int SetPlayerSpeed
    {
        set
        {
            playerSpeed = value;
        }
    }
    public int SetGravity
    {
        set
        {
            gravity = value;
        }
    }
    public int SetMaxStamina
    {
        set
        {
            maxStamina = value;
        }
    }
    public float SetStaminaRegenSpeed
    {
        set
        {
            staminaRegenSpeed = value;
        }
    }
    public float SetStaminaRegenCooldown
    {
        set
        {
            staminaRegenCooldown = value;
        }
    }
    public int SetMaxHealth
    {
        set
        {
            maxHealth = value;
        }
    }
    public int SetMaxArmor
    {
        set
        {
            maxArmor = value;
        }
    }
    public float SetArmorRegenSpeed
    {
        set
        {
            armorRegenSpeed = value;
        }
    }
    public float SetArmorRegenCooldown
    {
        set
        {
            armorRegenCooldown = value;
        }
    }
    public float SetInvincibilityCooldown
    {
        set
        {
            invincibilityCooldown = value;
        }
    }
    public float SetMaxJumpVel
    {
        set
        {
            maxJumpVel = value;
        }
    }
    public float SetJumpAcceleration
    {
        set
        {
            jumpAcceleration = value;
        }
    }
    public int SetMaxJumps
    {
        set
        {
            maxJumps = value;
        }
    }
    public int SetJumpStaminaCost
    {
        set
        {
            jumpStaminaCost = value;
        }
    }
    public float SetCoyoteTime
    {
        set
        {
            coyoteTime = value;
        }
    }
    public float SetJumpInputCooldown
    {
        set
        {
            jumpInputCooldown = value;
        }
    }
    public int SetDashSpeed
    {
        set
        {
            dashSpeed = value;
        }
    }
    public float SetDashCooldown
    {
        set
        {
            dashCooldown = value;
        }
    }
    public float SetDashDuration
    {
        set
        {
            dashDuration = value;
        }
    }
    public int SetDashStaminaCost
    {
        set
        {
            dashStaminaCost = value;
        }
    }
    public float SetDashInvincibilityTime
    {
        set
        {
            dashInvincibilityTime = value;
        }
    }
    public int SetSprintSpeed
    {
        set
        {
            sprintSpeed = value;
        }
    }
    public float SetSprintStaminaDrain
    {
        set
        {
            sprintStaminaDrain = value;
        }
    }
#endregion


    IEnumerator playFootstep()
    {
        if (isSprinting)
            audioSource.PlayOneShot(runSoundsGroup[Random.Range(0, runSoundsGroup.Length)]);
        else
            audioSource.PlayOneShot(walkSoundsGroup[Random.Range(0, walkSoundsGroup.Length)], 0.5f);

        canPlayFootsteps = false;
        yield return new WaitForSeconds(0.35f);
        canPlayFootsteps = true;
    }

    public void CopyWeaponFromPlayerPreferences(GameObject _copiedWeapon)
    {
        if (_copiedWeapon.GetComponent<rangedWeapon>().gunType == weaponStats.weaponStatsType.Gun)
        {
            //_copiedWeapon.transform.parent = playerCamera.transform;
            //_copiedWeapon.transform.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);
            GameObject _newWeapon = Instantiate(_copiedWeapon, playerCamera.transform.position, playerCamera.transform.rotation, playerCamera.transform);
            weaponInventory.Add(_newWeapon);
        }
    }

    public void CopyGrappleFromPlayerPreferences()
    {
        rangedWeaponPickup(backupGrapple, backupGrapple.weaponType);
    }

    public void godMode()
    {
        godModeEnabled = !godModeEnabled;
    }

    public void OnDeserialize()
    {
        gameManager.instance.SetPlayerController = this;
        gameManager.instance.SetCameraControls = Camera.main.gameObject.GetComponent<cameraControls>();
        gameManager.instance.playerInstance = gameObject;
    }
}