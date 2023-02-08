using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyAI : MonoBehaviour
{
    [Header("----- Agent -----")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] public GameObject PlayerGameObject;

    [Header("----- Masks -----")]
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("----- Misc Vars -----")]
    [Range(1, 10)] [SerializeField] private int _turnSpeed;
    [Range(1, 10)] [SerializeField] private int _roamingDelay;
    [SerializeField] private Vector3 playerDirection;
    [SerializeField] private Enemy _enemyScript;


    // Public for the FieldOfViewEditor Editor script
    [Range(0,360)] public float ViewAngle;
    public int ViewRadius;
    public int SprintDetectRadius;
    public int WalkDetectRadius;
    public bool CanDetectPlayer = false;

    [Header("----- Fallback AI -----")] 
    [SerializeField] private bool _useFallbackAi;
    [SerializeField] private Transform _headPosition;
    [SerializeField] private bool isShooting;

    void Start()
    {
        // Component initializations
        if(_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        // Doing this so the FieldOfViewEditor script can access the player via reference with this script
        if(PlayerGameObject == null)
            PlayerGameObject = gameManager.instance.player;
        
        _playerMask = LayerMask.GetMask("Player"); // Player layer mask for Enemy to check for Player check
        _obstacleMask = LayerMask.GetMask("Obstacle"); // Obstacle layer mask for Enemy to check if Obstacle is in the way for Player check

        // FALLBACK AI SETUP
        if (_useFallbackAi)
        {
            _headPosition.gameObject.SetActive(true);

            if (_headPosition == null)
                GameObject.FindGameObjectWithTag("PlayerHeadPosition");
        }
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
                // the Player is in the viewing angle of the Enemy OR
                // the player is within the SprintDetectRadius and is Sprinting OR
                // the player is within the WalkDetectRadius
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

                if (!isShooting)
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
