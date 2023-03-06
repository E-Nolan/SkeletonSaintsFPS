using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{
    [Header("----- GameObjects -----")] 
    [SerializeField] private GameObject _rotationGameObject;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _rocket;
    [SerializeField] private GameObject _firePosition;

    [Header("----- Layers -----")]
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("----- Floats -----")]
    public float ViewRadius;
    public float WalkDetectRadius;
    public float SprintDetectRadius;
    public float ShootDetectRadius;
    public float ViewAngle;
    public float FireAngle;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _fireDelay;
    [SerializeField] private float _seekSpeed;
    [SerializeField] private float _rotateAngle;

    [Header("----- Bools -----")] 
    public bool CanDetectPlayer;
    public bool CanShootPlayer;
    public bool IsFiring;
    public bool IsRocketTurret;

    private Vector3 playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        IsFiring = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CheckForPlayerWithDelay(0.5f));
    }

    void CheckForPlayer()
    {
        Collider[] targetsInViewRange = Physics.OverlapSphere(transform.position, ViewRadius, _playerMask);

        // Run as long as the Player was detected within the OverlapSphere()
        if (targetsInViewRange.Length != 0)
        {
            // OverlapSphere() only returns an array of Colliders so only take the first array entry (should only be one player)
            Transform playerTransform = targetsInViewRange[0].transform;

            // Get the direction the player is from the Turret
            playerDirection = (playerTransform.position - _rotationGameObject.transform.position).normalized;

            // Get the distance between the Turret and the Player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // Enter the if when:
            // - the Player is in the viewing angle of the Turret
            // - the Player is within the SprintDetectRadius and is Sprinting
            // - the Player is within the ShootDetectRadius and is Shooting
            // - the Player is within the WalkDetectRadius
            if (Vector3.Angle(_rotationGameObject.transform.forward, playerDirection) < ViewAngle / 2 ||
                (distanceToPlayer <= SprintDetectRadius && IsPlayerSprinting()) || 
                (distanceToPlayer <= ShootDetectRadius && IsPlayerShooting()) ||
                distanceToPlayer <= WalkDetectRadius)
            {
                if (distanceToPlayer <= WalkDetectRadius)
                    FacePlayer(playerTransform);

                // Checks if an Obstacle with the Obstacle layer mask is between the Enemy and Player
                // If no Obstacle was detected, runs the if, else the Turret can't see the Player
                if (!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, _obstacleMask))
                {
                    CanDetectPlayer = true;
                    FacePlayer(playerTransform);
                    float angleToPlayer = Vector3.Angle(playerDirection, _rotationGameObject.transform.forward);

                    CanShootPlayer = angleToPlayer <= FireAngle;

                    if (CanShootPlayer && !IsFiring)
                    {
                        StartCoroutine(Fire());
                    }
                }
                else
                {
                    CanDetectPlayer = false;
                    CanShootPlayer = false;
                }
            }
            else
            {
                CanDetectPlayer = false;
                CanShootPlayer = false;
            }
        }

        if (!CanDetectPlayer)
        {
            _rotationGameObject.transform.parent.localRotation = Quaternion.Euler(
                0f, 0f, Mathf.PingPong(Time.time * _seekSpeed, _rotateAngle * 2) - _rotateAngle);
        }
        else
        {
            _rotationGameObject.transform.parent.localRotation = 
                Quaternion.Lerp(_rotationGameObject.transform.parent.localRotation, 
                    Quaternion.Euler(Vector3.zero), Time.deltaTime * 3f);
        }
    }

    /// <summary>
    /// Rotate the Turret to face the Player with _turnSpeed
    /// </summary>
    void FacePlayer(Transform playerTransform)
    {
        Quaternion q = Quaternion.LookRotation((playerTransform.position - _rotationGameObject.transform.position).normalized);
        _rotationGameObject.transform.rotation = Quaternion.Lerp(_rotationGameObject.transform.rotation, q, Time.deltaTime * _turnSpeed);
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

    private IEnumerator Fire()
    {
        if (!IsFiring)
        {
            IsFiring = true;
            yield return new WaitForSeconds(_fireDelay);
            if (!IsRocketTurret)
            {
                GameObject bulletClone =
                    Instantiate(_bullet, _firePosition.transform.position, _bullet.transform.rotation);
                bulletClone.GetComponent<Rigidbody>().velocity = playerDirection * _bulletSpeed;
            }
            else
            {
                GameObject rocketClone =
                    Instantiate(_rocket, _firePosition.transform.position, _rocket.transform.rotation);
            }
            IsFiring = false;
        }
    }

    private IEnumerator CheckForPlayerWithDelay(float delay)
    {
        CheckForPlayer();
        yield return new WaitForSeconds(delay);
    }
}
