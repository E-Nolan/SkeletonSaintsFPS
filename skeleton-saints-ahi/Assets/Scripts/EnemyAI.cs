using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyAI : MonoBehaviour
{
    [Header("----- Objects/Transforms -----")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] public GameObject PlayerGameObject;
    [SerializeField] private Transform _headPosition;

    [Header("----- Masks -----")]
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("----- Animation -----")] 
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyLookAt _enemyLookAt;

    [Header("----- Misc -----")]
    [Range(1, 10)] [SerializeField] private int _turnSpeed;
    [Range(1, 10)] [SerializeField] private int _roamingDelay;
    [SerializeField] private Vector3 playerDirection;
    [SerializeField] private Enemy _enemyScript;

    [Header("----- FieldOfView Public -----")]
    // Public for the FieldOfViewEditor Editor script
     [Range(0,360)] public float ViewAngle;
     public int ViewRadius;
     public int SprintDetectRadius;
     public int WalkDetectRadius;
     public bool CanDetectPlayer = false;

    [Header("----- Fallback AI (buggy) -----")] 
    [SerializeField] private bool _useFallbackAi;


    private Vector2 smoothDeltaPosition;
    private Vector2 velocity;

    void Start()
    {
        // Component initializations
        if(_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        // Check if there is a Game Manager present
        if (gameManager.instance == null)
            Debug.Log("EnemyAI: There is no GameManager. Not using a GameManager will lead to unsupported behavior.");
        else
        {
            // Using PlayerGameObject so the FieldOfViewEditor script can access the player via reference with this script
            if(PlayerGameObject == null)
                PlayerGameObject = gameManager.instance.player;
        }

        _playerMask = LayerMask.GetMask("Player"); // Player layer mask for Enemy to check for Player check
        _obstacleMask = LayerMask.GetMask("Obstacle"); // Obstacle layer mask for Enemy to check if Obstacle is in the way for Player check

        if (_headPosition == null)
            GameObject.FindGameObjectWithTag("PlayerHeadPosition");

        if (_animator == null)
            GetComponent<Animator>();

        if (_enemyLookAt == null)
            GetComponent<EnemyLookAt>();

        _agent.updatePosition = false;
    }

    void Update()
    {
        // As long as the Player exists, Enemy will roam with specified delay
        if (PlayerGameObject != null)
            StartCoroutine(CheckForPlayerWithDelay(_roamingDelay));
        else
        {
            // If no player is detected for whatever reason, script destroys itself
            // on enemy so it doesnt continue checking (save minimal performance in edge cases)
            Debug.Log($"{gameObject.name} did not detect player, destroying EnemyAI script");
            Destroy(GetComponent<EnemyAI>());
        }

        // Get how far enemy is from its next position
        Vector3 worldDeltaPosition = _agent.nextPosition - transform.position;

        // Map worldDeltaPosition to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        // Use Vector2 because z isnt needed for our purpose (yet)
        Vector2 deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && _agent.remainingDistance > _agent.radius;

        // Update animation parameters
        _animator.SetBool("isWalking", shouldMove);
        _animator.SetFloat ("xVelocity", velocity.x);
        _animator.SetFloat ("yVelocity", velocity.y);

        if(_enemyLookAt != null)
            _enemyLookAt.lookAtTargetPosition = _agent.steeringTarget + transform.forward;
    }

    void OnAnimatorMove ()
    {
        // Update position to agent position
        transform.position = _agent.nextPosition;
    }

    private void CheckForPlayer()
    {
        if (!_useFallbackAi)
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
                playerDirection = playerTransform.position - transform.position;

                // Get the distance between the Enemy and the Player
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                // Enter the if when:
                // - the Player is in the viewing angle of the Enemy
                // - the player is within the SprintDetectRadius and is Sprinting
                // - the player is within the WalkDetectRadius
                if (Vector3.Angle(transform.forward, playerDirection) < ViewAngle / 2 ||
                    (distanceToPlayer <= SprintDetectRadius && gameManager.instance.playerScript.isSprinting) ||
                    distanceToPlayer <= WalkDetectRadius)
                {
                    // Checks if an Obstacle with the Obstacle layer mask is between the Enemy and Player
                    // If no Obstacle was detected, runs the if, else the Enemy can't see the Player
                    if (!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, _obstacleMask))
                    {
                        // Player detected, move Enemy towards Player
                        CanDetectPlayer = true;
                        _agent.SetDestination(playerTransform.position);

                        // If the Player is within the stopping distance of the Enemy,
                        // change rotation of the Enemy to face the Player
                        if (_agent.remainingDistance <= _agent.stoppingDistance)
                            FacePlayer();
                    }
                    else
                    {
                        // If Enemy detected the Player previously but can't currently detect the Player, go to last known location
                        if (CanDetectPlayer)
                            StartCoroutine(GoToLastKnownLocation(playerTransform.position, _roamingDelay));

                        CanDetectPlayer = false;
                    }
                }
                else
                {
                    CanDetectPlayer = false;
                }
            }
            else if (CanDetectPlayer)
                CanDetectPlayer = false;

            // If the Player was not detected, start Roaming with a delay
            if (CanDetectPlayer == false)
                StartCoroutine(RandomNavMeshLocationWithDelay(_roamingDelay));
            #endregion
        }
        else
        {
            #region FALLBACK_AI
            if (canSeePlayer() && !_enemyScript.isShooting)
            {
                _agent.SetDestination(gameManager.instance.player.transform.position);

                if (_agent.remainingDistance < _agent.stoppingDistance)
                    FacePlayer();

                if (!_enemyScript.isShooting)
                    StartCoroutine(_enemyScript.Shoot());
            }
            #endregion
        }
    }

    /// <summary>
    /// Fallback AI usage: Check if the Player is within viewing distance and angle of the Enemy
    /// </summary>
    /// <returns></returns>
    bool canSeePlayer()
    {
        playerDirection = gameManager.instance.player.transform.position - _headPosition.position;
        float angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(_headPosition.position, playerDirection);

        RaycastHit hit;
        if(Physics.Raycast(_headPosition.position, playerDirection, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= ViewAngle)
            {
                _agent.SetDestination(gameManager.instance.player.transform.position);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Rotate the Enemy to face the Player with _turnSpeed
    /// </summary>
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * _turnSpeed);
    }

    /// <summary>
    /// Gets a random location on the NavMesh within a given radius and sets the _agent's destination to that location for roaming
    /// </summary>
    public void RandomNavMeshLocation(float radius)
    {
        if (_agent.remainingDistance >= _agent.stoppingDistance)
            return;

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        Vector3 finalPosition = Vector3.zero;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, -1))
            finalPosition = hit.position;

        _agent.SetDestination(finalPosition);
    }

    /// <summary>
    /// Go to last known location of the location given with optional delay 
    /// </summary>
    private IEnumerator GoToLastKnownLocation(Vector3 location, float delay = 0f)
    {
        float originalStoppingDistance = _agent.stoppingDistance;
        _agent.stoppingDistance = 0f;
        _agent.SetDestination(PlayerGameObject.transform.position);
        yield return new WaitForSeconds(_roamingDelay);
        if (_agent.remainingDistance >= _agent.stoppingDistance)
            _agent.stoppingDistance = originalStoppingDistance;
    }

    /// <summary>
    /// RandomNavMeshLocation(ViewRadius) with delay
    /// </summary>
    private IEnumerator RandomNavMeshLocationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RandomNavMeshLocation(ViewRadius);
    }

    /// <summary>
    /// CheckForPlayer() with delay
    /// </summary>
    private IEnumerator CheckForPlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckForPlayer();
    }
}
