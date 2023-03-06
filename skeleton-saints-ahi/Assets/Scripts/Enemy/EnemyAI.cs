using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("----- Objects/Transforms -----")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _headPosition;
    [SerializeField] private Transform _weaponFirePosition;

    [Header("----- Masks -----")]
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("----- Animation -----")] 
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyLookAt _enemyLookAt;

    [Header("----- Misc -----")]
    [Range(1, 20)] [SerializeField] private float _turnSpeed;
    [Range(1, 20)] [SerializeField] private int _roamingDelay;
    [SerializeField] public Vector3 playerDirection;
    [SerializeField] private Enemy _enemyScript;

    [Header("----- Publics -----")]
    [Range(0,360)] public float ViewAngle;
    [Range(0,360)] public float FireAngle;
    public int ViewRadius;
    public int SprintDetectRadius;
    public int WalkDetectRadius;
    public int ShootDetectRadius;
    public bool CanDetectPlayer = false;
    public bool CanAttack = false;
    [Range(1,5)] public float chaseMultiplier;
    [Range(1,5)] public float turnMultiplier;
    public float originalTurnSpeed;
    public float originalStoppingDistance;
    public float originalSpeed;
    public float originalAcceleration;

    [Header("----- Mutant/Boss -----")]
    public bool IsMutant;
    public bool BossEnemy;
    public int AttackDamage;

    [Header("----- Fallback AI (buggy) -----")] 
    [SerializeField] private bool _useFallbackAi;

    private Vector2 smoothDeltaPosition;
    private Vector2 velocity;
    private bool destinationChosen;

    void Awake()
    {
        destinationChosen = false;
    }

    void Start()
    {
        if(_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        // Check if there is a Game Manager present
        if (gameManager.instance == null)
            Debug.Log("EnemyAI: There is no GameManager. Not using a GameManager will lead to unsupported behavior.");

        _playerMask = LayerMask.GetMask("Player"); // Player layer mask for Enemy to check for Player check
        _obstacleMask = LayerMask.GetMask("Obstacle"); // Obstacle layer mask for Enemy to check if Obstacle is in the way for Player check

        if (_headPosition == null)
            GameObject.FindGameObjectWithTag("PlayerHeadPosition");

        if (_animator == null)
            GetComponent<Animator>();

        if (_enemyLookAt == null)
            GetComponent<EnemyLookAt>();

        _animator.applyRootMotion = false;
        _agent.updatePosition = false;
        originalStoppingDistance = _agent.stoppingDistance;
        originalTurnSpeed = _turnSpeed;
        originalSpeed = _agent.speed;
        originalAcceleration = _agent.acceleration;
    }

    void Update()
    {
        if (_agent.enabled)
        {
            if (!CanDetectPlayer || _agent.destination != gameManager.instance.playerInstance.transform.position)
                StartCoroutine(CheckForPlayerWithDelay(_roamingDelay));
            else
            {
                // If no player is detected for whatever reason, script disables itself
                // on enemy so it doesnt continue checking (save minimal performance in edge cases)
                Debug.Log($"{gameObject.name} did not detect player, disabling EnemyAI");
                enabled = false;
            }

            // --- ANIMATION STUFF ---
            // Get how far enemy is from its next position
            Vector3 worldDeltaPosition = _agent.nextPosition - transform.position;

            // Map worldDeltaPosition to local space
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);

            // Use Vector2 because worldspace-y isnt needed for this
            Vector2 deltaPosition = new Vector2(dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            bool shouldMove = velocity.magnitude > 0.5f && _agent.remainingDistance > _agent.radius;

            // Update animation parameters
            if (_agent.velocity.magnitude >= 0.1f)
            {
                if (!IsMutant)
                {
                    _animator.SetFloat("xVelocity", velocity.normalized.x, 0.1f, Time.deltaTime);
                    _animator.SetFloat("yVelocity", velocity.normalized.y, 0.1f, Time.deltaTime);
                }
                else
                {
                    _animator.SetFloat("speed", velocity.normalized.magnitude, 0.1f, Time.deltaTime);
                }
            }
            else
            {
                // Enemy would continue current animation when should be idling, so if the agent has no velocity,
                // manually set the parameters
                if (!IsMutant)
                {
                    _animator.SetFloat("xVelocity", 0f, 0.1f, Time.deltaTime);
                    _animator.SetFloat("yVelocity", 0f, 0.1f, Time.deltaTime);
                }
                else
                {
                    _animator.SetFloat("speed", 0f, 0.1f, Time.deltaTime);
                }
            }

            if (_enemyLookAt != null)
                _enemyLookAt.lookAtFuturePosition = _agent.steeringTarget + transform.forward;
        }
    }

    // Having this on object sets animator's 
    void OnAnimatorMove ()
    {
        // Update object position to agent position
        transform.position = _agent.nextPosition;
    }

    private void CheckForPlayer()
    {
        #region CUSTOM_AI
        // Checks in a sphere around the Enemy's position in a given radius (ViewRadius)
        // Looks in only the Player layer mask so nothing other than Player is detected
        Collider[] targetsInViewRange = Physics.OverlapSphere(transform.position, ViewRadius, _playerMask);

        // Run as long as the Player was detected within the OverlapSphere()
        if (targetsInViewRange.Length != 0)
        {
            // OverlapSphere() only returns an array of Colliders so only take the first array entry (should only be one player)
            Transform playerTransform = targetsInViewRange[0].transform;

            // Get the direction the player is from the Enemy
            playerDirection = (playerTransform.position - transform.position).normalized;

            // Get the distance between the Enemy and the Player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // Enter the if when:
            // - the Player is in the viewing angle of the Enemy
            // - the Player is within the SprintDetectRadius and is Sprinting
            // - the Player is within the ShootDetectRadius and is Shooting
            // - the Player is within the WalkDetectRadius
            if (Vector3.Angle(transform.forward, playerDirection) < ViewAngle / 2 ||
                (distanceToPlayer <= SprintDetectRadius && IsPlayerSprinting()) || 
                (distanceToPlayer <= ShootDetectRadius && IsPlayerShooting()) ||
                distanceToPlayer <= WalkDetectRadius)
            {

                if (distanceToPlayer <= WalkDetectRadius)
                    FacePlayer();

                // Checks if an Obstacle with the Obstacle layer mask is between the Enemy and Player
                // If no Obstacle was detected, runs the if, else the Enemy can't see the Player
                if (!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, _obstacleMask))
                {
                    CanDetectPlayer = true;

                    if (NavMesh.SamplePosition(playerTransform.position, out NavMeshHit hit, ViewRadius, -1))
                    {
                        _agent.SetDestination(hit.position);
                        _agent.stoppingDistance = originalStoppingDistance;
                        SetupChase();
                    }

                    // If the Player is within the stopping distance of the Enemy,
                    // change rotation of the Enemy to face the Player
                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                        FacePlayer();

                    float angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0f, playerDirection.z), transform.forward);

                    // Set CanShoot bool to the result of (angleToPlayer <= FireAngle), if the Player is within the FireAngle
                    CanAttack = angleToPlayer <= FireAngle;
                }
                else
                {
                    // If Enemy detected the Player previously but can't currently detect the Player, go to last known location
                    if (CanDetectPlayer)
                        StartCoroutine(GoToLastKnownLocation(playerTransform.position, _roamingDelay));

                    CanDetectPlayer = false;
                    RevertChase();
                }
            }
            else
            {
                CanDetectPlayer = false;
                RevertChase();
            }
        }
        else if (CanDetectPlayer)
        {
            CanDetectPlayer = false;
            RevertChase();
        }

        // If the Player was not detected, start Roaming with a delay
        if (!CanDetectPlayer && !destinationChosen && _agent.remainingDistance < 0.1f)
        {
            StartCoroutine(RandomNavMeshLocation(ViewRadius, _roamingDelay));
            RevertChase();
        }
        #endregion
    }

    /// <summary>
    /// Rotate the Enemy to face the Player with _turnSpeed
    /// </summary>
    void FacePlayer()
    {
        playerDirection.y = 0f;
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * _turnSpeed);
    }

    /// <summary>
    /// Check if Player is Shooting
    /// </summary>
    private bool IsPlayerShooting()
    {
        return gameManager.instance.PlayerScript().IsPlayerShooting();
    }

    /// <summary>
    /// Check if Player is Sprinting
    /// </summary>
    private bool IsPlayerSprinting()
    {
        return gameManager.instance.PlayerScript().isSprinting;
    }

    private void IncreaseTurnSpeed()
    {
        if(Math.Abs(_turnSpeed - originalTurnSpeed) < 0.001f)
            _turnSpeed *= turnMultiplier;
    }

    private void DecreaseTurnSpeed()
    {
        if (Math.Abs(_turnSpeed - originalTurnSpeed) > 0.001f)
            _turnSpeed = originalTurnSpeed;
    }

    public void IncreaseAgentSpeed()
    {
        if(Math.Abs(_agent.speed - originalSpeed) < 0.001f)
            _agent.speed *= chaseMultiplier;
    }

    public void DecreaseAgentSpeed()
    {
        if(Math.Abs(_agent.speed - originalSpeed) > 0.001f)
            _agent.speed = originalSpeed;
    }

    public void IncreaseAgentAcceleration()
    {
        if (Math.Abs(_agent.acceleration - originalAcceleration) < 0.001f)
            _agent.acceleration = 100f;
    }

    public void DecreaseAgentAcceleration()
    {
        if(Math.Abs(_agent.acceleration - originalAcceleration) > 0.001f)
            _agent.acceleration = originalAcceleration;
    }

    public void RevertChase()
    {
        DecreaseTurnSpeed();
        DecreaseAgentSpeed();
        DecreaseAgentAcceleration();
    }

    public void SetupChase()
    {
        IncreaseTurnSpeed();
        IncreaseAgentSpeed();
        IncreaseAgentAcceleration();
    }

    public void SetAgentDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    public float GetAgentRemainingDistance()
    {
        return _agent.remainingDistance;
    }

    public float GetAgentStoppingDistance()
    {
        return _agent.stoppingDistance;
    }

    public NavMeshAgent GetAgent()
    {
        return _agent;
    }

    #region IENUMERATORS
    /// <summary>
    /// Go to last known location of the location given with optional delay 
    /// </summary>
    private IEnumerator GoToLastKnownLocation(Vector3 location, float delay = 0f)
    {
        if (_agent.isActiveAndEnabled)
        {
            RevertChase();
            _agent.stoppingDistance = 0f;

            if (NavMesh.SamplePosition(location, out NavMeshHit hit, ViewRadius, -1) && !CanDetectPlayer)
                _agent.SetDestination(hit.position);

            yield return new WaitForSeconds(delay);
        }

    }

    /// <summary>
    /// Move Enemy within radius from Enemy with optional delay
    /// </summary>
    private IEnumerator RandomNavMeshLocation(float radius, float delay = 0f)
    {
        if (!destinationChosen && _agent.remainingDistance <= 0.1f)
        {
            RevertChase();
            _agent.stoppingDistance = 0f;
            destinationChosen = true;
            yield return new WaitForSeconds(delay);
            destinationChosen = false;

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, -1) && _agent.isActiveAndEnabled)
                _agent.SetDestination(hit.position);
        }
    }


    /// <summary>
    /// CheckForPlayer() with delay
    /// </summary>
    private IEnumerator CheckForPlayerWithDelay(float delay)
    {
        CheckForPlayer();
        yield return new WaitForSeconds(delay);
    }
    #endregion
}
