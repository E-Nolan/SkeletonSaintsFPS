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

        if (_playerGameObject == null)
            Debug.Log($"{gameObject.name} could not find Player GameObject");
        else
            Debug.Log($"{gameObject.name} found the player");
    }

    void Update()
    {
        if(_playerGameObject != null)
            _agent.destination = _playerGameObject.transform.position;
    }
}
