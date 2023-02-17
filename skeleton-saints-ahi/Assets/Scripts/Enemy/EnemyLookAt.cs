using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class EnemyLookAt : MonoBehaviour {

    public Transform head = null;
    public Vector3 lookAtFuturePosition;
    public float lookAtCoolTime = 0.2f;
    public float lookAtHeatTime = 0.2f;
    public bool looking = true;

    private Vector3 lookAtCurrentPosition;
    private Animator animator;
    private float lookAtWeight = 0.0f;

    void Start ()
    {
        if (!head)
        {
            Debug.LogError("No head transform - EnemyLookAt disabled");
            enabled = false;
            return;
        }
        animator = GetComponent<Animator> ();
        lookAtFuturePosition = head.position + transform.forward;
        lookAtCurrentPosition = lookAtFuturePosition;
    }

    void OnAnimatorIK ()
    {
        lookAtFuturePosition.y = head.position.y;
        float lookAtTargetWeight = looking ? 1.0f : 0.0f;

        Vector3 currentDirection = lookAtCurrentPosition - head.position;
        Vector3 futureDirection = lookAtFuturePosition - head.position;

        currentDirection = Vector3.RotateTowards(currentDirection, futureDirection, 6.28f*Time.deltaTime, float.PositiveInfinity);
        lookAtCurrentPosition = head.position + currentDirection;

        float blendTime = lookAtTargetWeight > lookAtWeight ? lookAtHeatTime : lookAtCoolTime;

        lookAtWeight = Mathf.MoveTowards (lookAtWeight, lookAtTargetWeight, Time.deltaTime/blendTime);
        animator.SetLookAtWeight (lookAtWeight, 0.2f, 0.5f, 0.7f, 0.5f);
        animator.SetLookAtPosition (lookAtCurrentPosition);
    }
}
