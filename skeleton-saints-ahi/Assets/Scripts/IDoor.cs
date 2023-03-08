using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDoor : MonoBehaviour
{
    private Animator doorAnimator;
    private void Awake()
    {
        doorAnimator = GetComponentInParent<Animator>();
    }
    public void ActivateDoor()
    {
        if (doorAnimator.IsInTransition(0))
            return;
        AnimationReaction doorAction = ScriptableObject.CreateInstance<AnimationReaction>();
        doorAction.instruction = 1;
        doorAction.animator = doorAnimator;
        doorAction.text = "Activated";
        doorAction.React(this);
    }
}
