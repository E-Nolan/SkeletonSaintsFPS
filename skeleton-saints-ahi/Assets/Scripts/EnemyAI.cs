using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject _playerGameObject;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obsticleMask;
    [Range(0,360)] [SerializeField] private float viewAngle;

    public bool CanSeePlayer = false;

    void Start()
    {
        if(_agent == null)
            _agent = GetComponent<NavMeshAgent>();
        if(_playerGameObject == null)
            _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        
        _playerMask = LayerMask.GetMask("Player");
        _obsticleMask = LayerMask.GetMask("Obsticle");
    }

    void Update()
    {
        if (_playerGameObject != null)
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
            Vector3 playerDirection = (target.position - transform.position);

            if (Vector3.Angle(transform.forward, playerDirection) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, _obsticleMask))
                {
                    CanSeePlayer = true;
                    _agent.destination = target.position;

                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        Quaternion enemyRotation = Quaternion.LookRotation(playerDirection);
                        transform.rotation = Quaternion.Lerp(transform.rotation, enemyRotation, Time.deltaTime * 2f);
                    }

#if UNITY_EDITOR
                    Debug.DrawRay(transform.position, playerDirection, Color.green);
#endif
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
