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

    // Public for the FieldOfViewEditor script
    [Range(0,360)] public float viewAngle;
    public GameObject playerGameObject;
    public int radius;
    public bool CanSeePlayer = false;

    void Start()
    {
        if(_agent == null)
            _agent = GetComponent<NavMeshAgent>();
        if(playerGameObject == null)
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
        
        _playerMask = LayerMask.GetMask("Player");
        _obstacleMask = LayerMask.GetMask("Obstacle");
    }

    void Update()
    {
        if (playerGameObject != null)
            //CheckForPlayer();
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
        Collider[] targetsInViewRange = Physics.OverlapSphere(transform.position, 170f, _playerMask);

        if (targetsInViewRange.Length != 0)
        {
            Transform target = targetsInViewRange[0].transform;
            Vector3 playerDirection = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, playerDirection) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, _obstacleMask))
                {
                    CanSeePlayer = true;
                    _agent.SetDestination(playerGameObject.transform.position);

                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        Quaternion enemyRotation = Quaternion.LookRotation(playerDirection);
                        transform.rotation = Quaternion.Lerp(transform.rotation, enemyRotation, Time.deltaTime * 2f);
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
    }

    private IEnumerator CheckForPlayerWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            CheckForPlayer();
        }
    }
}
