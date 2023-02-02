using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject _playerGameObject;
    NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(_playerGameObject != null)
            _agent.destination = _playerGameObject.transform.position;
        else
        {
            // If no player is detected for whatever reason, script destroys itself
            // on enemy so it doesnt continue checking (save minimal performance)
            Debug.Log($"{gameObject.name} did not detect player, destroying EnemyAI script");
            Destroy(GetComponent<EnemyAI>());
        }
    }
}
