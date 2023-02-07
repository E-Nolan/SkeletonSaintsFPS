using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;
    [Range(1, 10)] [SerializeField] private int _turnSpeed;
    [Range(1, 10)] [SerializeField] private int _roamingDelay;

    // Public for the FieldOfViewEditor script
    [Range(0,360)] public float ViewAngle;
    public int ViewRadius;
    public GameObject PlayerGameObject;
    public bool CanSeePlayer = false;

    void Start()
    {
        if(_agent == null)
            _agent = GetComponent<NavMeshAgent>();
        if(PlayerGameObject == null)
            PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        
        _playerMask = LayerMask.GetMask("Player");
        _obstacleMask = LayerMask.GetMask("Obstacle");
    }

    void Update()
    {
        if (PlayerGameObject != null)
            StartCoroutine(CheckForPlayerWithDelay(.25f));
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
        Collider[] targetsInViewRange = Physics.OverlapSphere(transform.position, ViewRadius, _playerMask);

        if (targetsInViewRange.Length != 0)
        {
            Transform player = targetsInViewRange[0].transform;

            Vector3 playerDirection = (player.position - transform.position);

            if (Vector3.Angle(transform.forward, playerDirection) < ViewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, _obstacleMask))
                {
                    CanSeePlayer = true;
                    _agent.SetDestination(PlayerGameObject.transform.position);

                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        Quaternion enemyRotation = Quaternion.LookRotation(playerDirection);
                        transform.rotation = Quaternion.Lerp(transform.rotation, enemyRotation, Time.deltaTime * _turnSpeed);
                    }
                }
                else
                {
                    CanSeePlayer = false;
                }
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        else if (CanSeePlayer)
            CanSeePlayer = false;

        if (CanSeePlayer == false)
            StartCoroutine(RandomNavMeshLocationWithDelay(_roamingDelay));
    }

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

    private IEnumerator RandomNavMeshLocationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RandomNavMeshLocation(ViewRadius);
    }

    private IEnumerator CheckForPlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckForPlayer();
    }
}
